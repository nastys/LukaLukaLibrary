﻿using System.ComponentModel;
using System.Numerics;
using System.Windows.Forms;
using LukaLukaLibrary.Maths;
using LukaLukaLibrary.Misc;
using LukaLukaLibrary.Models;
using LukaLukaModel.GUI.Controls;
using LukaLukaModel.Nodes.Misc;

namespace LukaLukaModel.Nodes.Models
{
    public class SubMeshNode : Node<SubMesh>
    {
        public override NodeFlags Flags => NodeFlags.Add | NodeFlags.Rename;

        public override Control Control
        {
            get
            {
                var modelParent = FindParent<ModelNode>();
                var meshParent = FindParent<MeshNode>();

                if ( modelParent == null || meshParent == null )
                    return null;

                ModelViewControl.Instance.SetModel( Data, meshParent.Data, modelParent.Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        public Vector3[] Vertices
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        public Vector3[] Normals
        {
            get => GetProperty<Vector3[]>();
            set => SetProperty( value );
        }

        public Vector4[] Tangents
        {
            get => GetProperty<Vector4[]>();
            set => SetProperty( value );
        }

        [DisplayName( "UV channel 1" )]
        public Vector2[] UVChannel1
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        [DisplayName( "UV channel 2" )]
        public Vector2[] UVChannel2
        {
            get => GetProperty<Vector2[]>();
            set => SetProperty( value );
        }

        public Color[] Colors
        {
            get => GetProperty<Color[]>();
            set => SetProperty( value );
        }

        [DisplayName( "Bone weights" )]
        public BoneWeight[] BoneWeights
        {
            get => GetProperty<BoneWeight[]>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<IndexTable>( "Index tables", Data.IndexTables ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public SubMeshNode( string name, SubMesh data ) : base( name, data )
        {
        }
    }
}