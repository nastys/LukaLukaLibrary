﻿using LukaLukaLibrary.Motions;
using LukaLukaModel.Nodes.Misc;

namespace LukaLukaModel.Nodes.Motions
{
    public class MotionControllerNode : Node<MotionController>
    {
        public override NodeFlags Flags => NodeFlags.Add;

        protected override void Initialize()
        {
        }

        protected override void PopulateCore()
        {
            Nodes.Add( new ListNode<KeyController>( "Key controllers", Data.KeyControllers, x => x.Name ) );
        }

        protected override void SynchronizeCore()
        {
        }

        public MotionControllerNode( string name, MotionController data ) : base( name, data )
        {
        }
    }
}