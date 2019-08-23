﻿using System;
using System.Collections.Generic;
using System.IO;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.IO;
using LukaLukaLibrary.IO.Common;
using LukaLukaLibrary.IO.Sections;

namespace LukaLukaLibrary.Motions
{
    public class Motion : BinaryFile
    {
        private MotionController mController;

        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public int Id { get; set; }
        public string Name { get; set; }
        public int HighBits { get; set; }
        public int FrameCount { get; set; }
        public List<KeySet> KeySets { get; }
        public List<BoneInfo> BoneInfos { get; }

        public bool HasController => mController != null;

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            if ( section != null )
            {
                if ( section.Format == BinaryFormat.F2nd )
                    reader.SeekCurrent( 4 );

                Id = reader.ReadInt32();
                Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            }

            long keySetCountOffset = reader.ReadOffset();
            long keySetTypesOffset = reader.ReadOffset();
            long keySetsOffset = reader.ReadOffset();
            long boneInfosOffset = reader.ReadOffset();

            if ( section != null )
            {
                long boneIdsOffset = reader.ReadOffset();
                int boneInfoCount = reader.ReadInt32();

                BoneInfos.Capacity = boneInfoCount;
                reader.ReadAtOffset( boneInfosOffset, () =>
                {
                    for ( int i = 0; i < boneInfoCount; i++ )
                        BoneInfos.Add( new BoneInfo
                        { Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated ) } );
                } );

                reader.ReadAtOffset( boneIdsOffset, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        boneInfo.Id = ( int ) reader.ReadUInt64();
                } );
            }
            else
            {
                reader.ReadAtOffset( boneInfosOffset, () =>
                {
                    int index = reader.ReadUInt16();
                    do
                    {
                        BoneInfos.Add( new BoneInfo { Id = index } );
                        index = reader.ReadUInt16();
                    } while ( index != 0 );
                } );
            }

            reader.ReadAtOffset( keySetCountOffset, () =>
            {
                int info = reader.ReadUInt16();
                int keySetCount = info & 0x3FFF;
                HighBits = info >> 14;
                FrameCount = reader.ReadUInt16();

                KeySets.Capacity = keySetCount;
                reader.ReadAtOffset( keySetTypesOffset, () =>
                {
                    for ( int i = 0, b = 0; i < keySetCount; i++ )
                    {
                        if ( i % 8 == 0 )
                            b = reader.ReadUInt16();

                        KeySets.Add( new KeySet { Type = ( KeySetType ) ( ( b >> ( i % 8 * 2 ) ) & 3 ) } );
                    }

                    reader.ReadAtOffset( keySetsOffset, () =>
                    {
                        foreach ( var keySet in KeySets )
                            keySet.Read( reader, section != null );
                    } );
                } );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            if ( section != null )
            {
                if ( section.Format == BinaryFormat.F2nd )
                    writer.Write( 0 );

                writer.Write( Id );
                writer.AddStringToStringTable( Name );
            }
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                writer.Write( ( ushort ) ( ( HighBits << 14 ) | KeySets.Count ) );
                writer.Write( ( ushort ) FrameCount );
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                for ( int i = 0, b = 0; i < KeySets.Count; i++ )
                {
                    var keySet = KeySets[ i ];

                    if ( keySet.Keys.Count == 1 )
                        keySet.Type = KeySetType.Static;

                    else if ( keySet.Keys.Count > 1 )
                        keySet.Type = keySet.IsInterpolated
                            ? KeySetType.Interpolated
                            : KeySetType.Linear;

                    else
                        keySet.Type = KeySetType.None;

                    b |= ( int ) keySet.Type << ( i % 8 * 2 );

                    if ( i != KeySets.Count - 1 && i % 8 != 7 )
                        continue;

                    writer.Write( ( ushort ) b );
                    b = 0;
                }
            } );
            writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
            {
                foreach ( var keySet in KeySets )
                    keySet.Write( writer, section != null );
            } );
            if ( section != null )
            {
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.AddStringToStringTable( boneInfo.Name );
                } );
                writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.Write( ( long ) boneInfo.Id );
                } );
                writer.Write( BoneInfos.Count );
                writer.WriteAlignmentPadding( 16 );
            }
            else
            {
                writer.ScheduleWriteOffset( 4, AlignmentMode.Left, () =>
                {
                    foreach ( var boneInfo in BoneInfos )
                        writer.Write( ( ushort ) boneInfo.Id );

                    writer.Write( ( ushort ) 0 );
                } );
            }
        }

        public MotionController GetController( SkeletonEntry skeletonEntry = null,
            MotionDatabase motionDatabase = null )
        {
            if ( mController != null )
                return mController;

            if ( skeletonEntry == null )
                throw new ArgumentNullException( nameof( skeletonEntry ) );

            var controller = new MotionController( this );

            int index = 0;
            foreach ( var boneInfo in BoneInfos )
            {
                if ( motionDatabase != null && boneInfo.Id >= motionDatabase.BoneNames.Count )
                    break;
            
                boneInfo.Name = boneInfo.Name ?? motionDatabase?.BoneNames[ boneInfo.Id ] ??
                                throw new ArgumentNullException( nameof( motionDatabase ) );

                var boneEntry = skeletonEntry.GetBoneEntry( boneInfo.Name );
                var keyController = new KeyController { Name = boneInfo.Name };

                if ( boneEntry != null )
                {
                    if ( boneEntry.Type != BoneType.Rotation )
                        keyController.Position = new KeySetVector
                        {
                            X = KeySets[ index++ ],
                            Y = KeySets[ index++ ],
                            Z = KeySets[ index++ ],
                        };

                    if ( boneEntry.Type != BoneType.Position )
                        keyController.Rotation = new KeySetVector
                        {
                            X = KeySets[ index++ ],
                            Y = KeySets[ index++ ],
                            Z = KeySets[ index++ ],
                        };
                }
                else if ( !skeletonEntry.BoneNames2.Contains( boneInfo.Name ) )
                    keyController.Position = new KeySetVector
                    {
                        X = KeySets[ index++ ],
                        Y = KeySets[ index++ ],
                        Z = KeySets[ index++ ],
                    };


                controller.KeyControllers.Add( keyController );
            }

            return mController = controller;
        }

        public void Load( Stream source, SkeletonEntry skeletonEntry, bool leaveOpen = false )
        {
            Load( source, leaveOpen );

            if ( skeletonEntry != null )
                GetController( skeletonEntry );
        }

        public void Load( string filePath, SkeletonEntry skeletonEntry )
        {
            using ( var stream = File.OpenRead( filePath ) )
                Load( stream, skeletonEntry );
        }

        public void Save( Stream destination, SkeletonEntry skeletonEntry, bool leaveOpen = false )
        {
            if ( skeletonEntry != null )
                mController?.Update( skeletonEntry );

            Save( destination, leaveOpen );
        }

        public void Save( string filePath, SkeletonEntry skeletonEntry )
        {
            using ( var stream = File.Create( filePath ) )
                Save( stream, skeletonEntry );
        }

        public Motion()
        {
            KeySets = new List<KeySet>();
            BoneInfos = new List<BoneInfo>();
        }
    }

    public class BoneInfo
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}