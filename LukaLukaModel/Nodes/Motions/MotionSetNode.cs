﻿using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.Motions;
using LukaLukaModel.Configurations;
using LukaLukaModel.Nodes.IO;
using LukaLukaModel.Nodes.Misc;
using Ookii.Dialogs.WinForms;

namespace LukaLukaModel.Nodes.Motions
{
    public class MotionSetNode : BinaryFileNode<MotionSet>
    {
        private static readonly XmlSerializer sMotionSetEntrySerializer = new XmlSerializer( typeof( MotionSetEntry ) );

        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Import | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            RegisterImportHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
                Data.Motions.Add( motion );
            } );
            RegisterImportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                Data.Motions.AddRange( motionSet.Motions );
            } );
            RegisterReplaceHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motionSet = new MotionSet();
                {
                    motionSet.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ],
                        configuration?.MotionDatabase );
                }
                return motionSet;
            } );
            RegisterReplaceHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                var motionSet = new MotionSet();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                    motionSet.Motions.Add( motion );
                }
                return motionSet;
            } );
            RegisterExportHandler<MotionSet>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                {
                    Data.Save( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ], configuration?.MotionDatabase );
                }
            } );
            RegisterCustomHandler( "Copy motion database info to clipboard", () =>
            {
                int id = -1;

                using ( var inputDialog = new InputDialog
                {
                    WindowTitle = "Enter base id for motions",
                    Input = Math.Max( 0, Data.Motions.Max( x => x.Id ) + 1 ).ToString()
                } )
                {
                    while ( inputDialog.ShowDialog() == DialogResult.OK )
                    {
                        bool result = int.TryParse( inputDialog.Input, out id );

                        if ( !result || id < 0 )
                        {
                            MessageBox.Show( "Please enter a correct id number.", "Luka Luka Model",
                                MessageBoxButtons.OK, MessageBoxIcon.Error );
                        }
                        else
                        {
                            break;
                        }
                    }
                }

                if ( id < 0 )
                    return;

                var motionSetEntry = new MotionSetEntry
                {
                    Id = 39,
                    Name = Path.GetFileNameWithoutExtension( Name ),
                };

                if ( motionSetEntry.Name.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetEntry.Name = motionSetEntry.Name.Remove( 0, 4 );

                foreach ( var motion in Data.Motions )
                {
                    motionSetEntry.Motions.Add( new MotionEntry
                    {
                        Name = motion.Name,
                        Id = id++,
                    } );
                }

                using ( var stringWriter = new StringWriter() )
                using ( var xmlWriter =
                    XmlWriter.Create( stringWriter,
                        new XmlWriterSettings { Indent = true, OmitXmlDeclaration = true } ) )
                {
                    sMotionSetEntrySerializer.Serialize( xmlWriter, motionSetEntry,
                        new XmlSerializerNamespaces( new[] { XmlQualifiedName.Empty } ) );

                    Clipboard.SetText( stringWriter.ToString() );
                }
            } );

            base.Initialize();
        }

        protected override void Load( MotionSet data, Stream source ) =>
            data.Load( source,
                SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                SourceConfiguration?.MotionDatabase );

        protected override void PopulateCore()
        {
            var motionDatabase = SourceConfiguration?.MotionDatabase;
            if ( motionDatabase != null )
            {
                string motionSetName = Path.GetFileNameWithoutExtension( Name );
                if ( motionSetName.StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) )
                    motionSetName = motionSetName.Substring( 4 );

                var motionSetEntry = motionDatabase.GetMotionSet( motionSetName );
                if ( motionSetEntry != null && Data.Motions.Count == motionSetEntry.Motions.Count )
                    for ( int i = 0; i < motionSetEntry.Motions.Count; i++ )
                    {
                        Data.Motions[ i ].Name = motionSetEntry.Motions[ i ].Name;
                        Data.Motions[ i ].Id = motionSetEntry.Motions[ i ].Id;
                    }
            }

            Nodes.Add( new ListNode<Motion>( "Motions", Data.Motions, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionSetNode( string name, MotionSet data ) : base( name, data )
        {
        }

        public MotionSetNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}