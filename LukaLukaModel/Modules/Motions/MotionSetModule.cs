﻿using System;
using System.IO;
using LukaLukaLibrary.Motions;
using LukaLukaModel.Configurations;

namespace LukaLukaModel.Modules.Motions
{
    public class MotionSetModule : FormatModule<MotionSet>
    {
        public override FormatModuleFlags Flags => FormatModuleFlags.Import | FormatModuleFlags.Export;
        public override string Name => "Motion Set";
        public override string[] Extensions => new[] { "bin" };

        public override bool Match( string fileName ) =>
            Path.GetFileNameWithoutExtension( fileName ).StartsWith( "mot_", StringComparison.OrdinalIgnoreCase ) &&
            base.Match( fileName );

        public override MotionSet Import( string filePath )
        {
            var motion = new MotionSet();
            {
                motion.Load( filePath,
                    ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                    ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase );
            }
            return motion;
        }

        protected override MotionSet ImportCore( Stream source, string fileName )
        {
            var motion = new MotionSet();
            {
                motion.Load( source,
                    ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                    ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase, true );
            }
            return motion;
        }

        protected override void ExportCore( MotionSet model, Stream destination, string fileName )
        {
            model.Save( destination,
                ConfigurationList.Instance.CurrentConfiguration?.BoneDatabase?.Skeletons?[ 0 ],
                ConfigurationList.Instance.CurrentConfiguration?.MotionDatabase, true );
        }
    }
}