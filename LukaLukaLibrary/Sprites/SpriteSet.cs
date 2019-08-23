﻿using System;
using LukaLukaLibrary.IO;
using LukaLukaLibrary.IO.Common;
using LukaLukaLibrary.IO.Sections;
using LukaLukaLibrary.Textures;
using System.Collections.Generic;
using System.IO;
using LukaLukaLibrary.Databases;

namespace LukaLukaLibrary.Sprites
{
    public class SpriteSet : BinaryFile
    {
        public override BinaryFileFlags Flags =>
            BinaryFileFlags.Load | BinaryFileFlags.Save | BinaryFileFlags.HasSectionFormat;

        public List<Sprite> Sprites { get; }
        public TextureSet TextureSet { get; internal set; }

        public override void Read( EndianBinaryReader reader, ISection section = null )
        {
            int signature = reader.ReadInt32();
            uint texturesOffset = reader.ReadUInt32();
            int textureCount = reader.ReadInt32();
            int spriteCount = reader.ReadInt32();
            long spritesOffset = reader.ReadOffset();
            long textureNamesOffset = reader.ReadOffset();
            long spriteNamesOffset = reader.ReadOffset();
            long spriteUnknownsOffset = reader.ReadOffset();

            reader.ReadAtOffsetIf( section == null, texturesOffset, () =>
            {
                TextureSet.Load( reader.BaseStream, true );
            } );

            Sprites.Capacity = spriteCount;
            reader.ReadAtOffset( spritesOffset, () =>
            {
                for ( int i = 0; i < spriteCount; i++ )
                {
                    var sprite = new Sprite();
                    sprite.ReadFirst( reader );
                    Sprites.Add( sprite );
                }
            } );

            reader.ReadAtOffsetIf( section?.Endianness != Endianness.BigEndian, textureNamesOffset, () =>
            {
                foreach ( var texture in TextureSet.Textures )
                    texture.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffsetIf( section?.Endianness != Endianness.BigEndian, spriteNamesOffset, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.Name = reader.ReadStringOffset( StringBinaryFormat.NullTerminated );
            } );

            reader.ReadAtOffset( spriteUnknownsOffset, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.ReadSecond( reader );
            } );
        }

        public override void Write( EndianBinaryWriter writer, ISection section = null )
        {
            writer.Write( 0 );
            writer.ScheduleWriteOffsetIf( section == null, 1, 16, AlignmentMode.Left, () => TextureSet.Save( writer.BaseStream, true ) );
            writer.Write( TextureSet.Textures.Count );
            writer.Write( Sprites.Count );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.WriteFirst( writer );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var texture in TextureSet.Textures )
                    writer.AddStringToStringTable( texture.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    writer.AddStringToStringTable( sprite.Name );
            } );
            writer.ScheduleWriteOffset( 16, AlignmentMode.Left, () =>
            {
                foreach ( var sprite in Sprites )
                    sprite.WriteSecond( writer );
                    
                writer.PopStringTablesReversed();
            } );
        }

        public override void Load( string filePath )
        {
            base.Load( filePath );

            if ( !filePath.EndsWith( ".spr", StringComparison.OrdinalIgnoreCase ) )
                return;

            string spriteDatabaseFilePath = Path.ChangeExtension( filePath, "spi" );
            if ( !File.Exists( spriteDatabaseFilePath ) )
                return;

            var spriteDatabase = BinaryFile.Load<SpriteDatabase>( spriteDatabaseFilePath );
            var spriteSetEntry = spriteDatabase.SpriteSets[ 0 ];

            foreach ( var spriteEntry in spriteSetEntry.Sprites )
                Sprites[ spriteEntry.Index ].Name = spriteEntry.Name;

            foreach ( var textureEntry in spriteSetEntry.Textures )
                TextureSet.Textures[ textureEntry.Index ].Name = textureEntry.Name;
        }

        public SpriteSet()
        {
            Sprites = new List<Sprite>();
            TextureSet = new TextureSet();
        }
    }
}
