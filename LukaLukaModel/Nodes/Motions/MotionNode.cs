﻿using System;
using System.Drawing;
using System.IO;
using LukaLukaLibrary.Motions;
using LukaLukaModel.Configurations;
using LukaLukaModel.Nodes.IO;
using LukaLukaModel.Nodes.Misc;
using LukaLukaModel.Resources;

namespace LukaLukaModel.Nodes.Motions
{
    public class MotionNode : BinaryFileNode<Motion>
    {
        public override NodeFlags Flags =>
            NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image => ResourceStore.LoadBitmap( "Icons/Motion.png" );

        protected override void Initialize()
        {
            RegisterReplaceHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                var motion = new Motion();
                {
                    motion.Load( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
                return motion;
            } );
            RegisterExportHandler<Motion>( filePath =>
            {
                var configuration = ConfigurationList.Instance.CurrentConfiguration;
                {
                    Data.Save( filePath, configuration?.BoneDatabase?.Skeletons?[ 0 ] );
                }
            } );
            RegisterExportHandler<MotionSet>( filePath =>
            {
                using ( var motionSet = new MotionSet() )
                {
                    motionSet.Motions.Add( Data );

                    var configuration = ConfigurationList.Instance.CurrentConfiguration;
                    {
                        motionSet.Save( filePath,
                            configuration?.BoneDatabase?.Skeletons?[ 0 ], configuration?.MotionDatabase );
                    }
                }
            } );
        }

        protected override void Load( Motion data, Stream source ) =>
            data.Load( source, SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ] );

        protected override void PopulateCore()
        {
            if ( !Data.HasController )
            {
                var skeletonEntry = SourceConfiguration?.BoneDatabase?.Skeletons?[ 0 ];
                var motionDatabase = SourceConfiguration?.MotionDatabase;

                try
                {
                    Data.GetController( skeletonEntry, motionDatabase );
                }
                catch ( ArgumentNullException )
                {

                }
            }

            if ( Data.HasController )
            {
                Nodes.Add( new MotionControllerNode( "Controller", Data.GetController() ) );
            }
            else
            {
                Nodes.Add( new ListNode<KeySet>( "Key sets", Data.KeySets ) );
                Nodes.Add( new ListNode<BoneInfo>( "Bones", Data.BoneInfos, x => x.Name ) );
            }
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionNode( string name, Motion data ) : base( name, data )
        {
        }

        public MotionNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }

    public class BoneInfoNode : Node<BoneInfo>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        public int Id
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneInfoNode( string name, BoneInfo data ) : base( name, data )
        {
        }
    }
}