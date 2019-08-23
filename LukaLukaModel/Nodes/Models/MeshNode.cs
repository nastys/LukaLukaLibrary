using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using LukaLukaLibrary.Materials;
using LukaLukaLibrary.Maths;
using LukaLukaLibrary.Models;
using LukaLukaModel.GUI.Controls;
using LukaLukaModel.Nodes.Misc;
using LukaLukaModel.Resources;

namespace LukaLukaModel.Nodes.Models
{
    public class MeshNode : Node<Mesh>
    {
        public override NodeFlags Flags =>
            NodeFlags.Import | NodeFlags.Add | NodeFlags.Export | NodeFlags.Replace | NodeFlags.Rename;

        public override Bitmap Image => 
            ResourceStore.LoadBitmap( "Icons/Mesh.png" );

        public override Control Control
        {
            get
            {
                var modelParent = FindParent<ModelNode>();
                if ( modelParent == null )
                    return null;

                ModelViewControl.Instance.SetModel( Data, modelParent.Data.TextureSet );
                return ModelViewControl.Instance;
            }
        }

        public int Id
        {
            get => GetProperty<int>();
            set => SetProperty( value );
        }

        [DisplayName( "Bounding sphere" )]
        public BoundingSphere BoundingSphere
        {
            get => GetProperty<BoundingSphere>();
            set => SetProperty( value );
        }

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<SubMesh>( "Submeshes", Data.SubMeshes, x => x.Name ) );
            Nodes.Add( new ListNode<Material>( "Materials", Data.Materials, x => x.Name ) );

            if ( Data.Skin != null )
                Nodes.Add( new SkinNode( "Skin", Data.Skin ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MeshNode( string name, Mesh data ) : base( name, data )
        {
        }
    }   
}