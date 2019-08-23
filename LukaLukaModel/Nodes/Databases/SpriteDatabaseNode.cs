﻿using System;
using System.IO;
using LukaLukaLibrary.Databases;
using LukaLukaLibrary.IO;
using LukaLukaModel.Nodes.IO;

namespace LukaLukaModel.Nodes.Databases
{
    public class SpriteDatabaseNode : BinaryFileNode<SpriteDatabase>
    {
        public override NodeFlags Flags =>
            NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        protected override void Initialize()
        {
            RegisterExportHandler<SpriteDatabase>( filePath => Data.Save( filePath ) );
            RegisterReplaceHandler<SpriteDatabase>( BinaryFile.Load<SpriteDatabase> );

            base.Initialize();
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public SpriteDatabaseNode( string name, SpriteDatabase data ) : base( name, data )
        {
        }

        public SpriteDatabaseNode( string name, Func<Stream> streamGetter ) : base( name, streamGetter )
        {
        }
    }
}