﻿using System;
using System.IO;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.IO;

namespace LukaLukaModel.Modules.Databases
{
    public class BoneDatabaseModule : FormatModule<BoneDatabase>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Bone Database";
        public override string[] Extensions => new[] { "bin", "bon" };

        public override bool Match( string fileName ) =>
            base.Match( fileName ) && Path.GetFileNameWithoutExtension( fileName )
                .Equals( "bone_data", StringComparison.OrdinalIgnoreCase );

        protected override BoneDatabase ImportCore( Stream source, string fileName ) =>
            BinaryFile.Load<BoneDatabase>( source, true );

        protected override void ExportCore( BoneDatabase model, Stream destination, string fileName ) =>
            model.Save( destination, true );
    }
}