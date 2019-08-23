﻿using System;
using System.IO;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.IO;

namespace LukaLukaModel.Modules.Databases
{
    public class MotionDatabaseModule : FormatModule<MotionDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Motion Database";
        public override string[] Extensions => new[] { "bin" };

        public override bool Match( string fileName ) =>
            base.Match( fileName ) && Path.GetFileNameWithoutExtension( fileName )
                .Equals( "mot_db", StringComparison.OrdinalIgnoreCase );

        protected override MotionDatabase ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<MotionDatabase>( source, true );

        protected override void ExportCore( MotionDatabase model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}