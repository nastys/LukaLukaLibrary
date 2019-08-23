﻿using System;
using System.IO;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.IO;

namespace LukaLukaModel.Modules.Databases
{
    public class TextureDatabaseModule : FormatModule<TextureDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Texture Database";
        public override string[] Extensions => new[] { "bin", "txi" };

        public override bool Match( string fileName )
        {
            if ( fileName.EndsWith( ".bin", StringComparison.OrdinalIgnoreCase ) )
            {
                if ( fileName.StartsWith( "mdata_", StringComparison.OrdinalIgnoreCase ) )
                    fileName = fileName.Remove( 0, 6 );

                return Path.GetFileNameWithoutExtension( fileName )
                    .Equals( "tex_db", StringComparison.OrdinalIgnoreCase );
            }

            return base.Match( fileName );
        }

        protected override TextureDatabase ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<TextureDatabase>( source, true );

        protected override void ExportCore( TextureDatabase model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}