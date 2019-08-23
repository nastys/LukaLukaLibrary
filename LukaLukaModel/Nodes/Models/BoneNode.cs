﻿using System.ComponentModel;
using System.Numerics;
using LukaLukaLibrary.Models;

namespace LukaLukaModel.Nodes.Models
{
    public class BoneNode : Node<Bone>
    {
        public override NodeFlags Flags => NodeFlags.Rename;

        [DisplayName( "Parent id" )]
        public int ParentId
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public int Id
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        public Matrix4x4 Matrix
        {
            get => GetProperty<Matrix4x4>();
            set => SetProperty( value );
        }

        [DisplayName( "Belongs in ex data" )]
        public bool IsEx => GetProperty<bool>();

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
        }

        protected override void SynchronizeCore()
        {
        }

        public BoneNode( string name, Bone data ) : base( name, data )
        {
        }
    }
}