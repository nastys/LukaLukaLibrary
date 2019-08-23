using System;
using System.IO;

namespace LukaLukaModel.Nodes.IO
{
    public interface IDirtyNode : INode
    {
        event EventHandler DirtyStateChanged;
        bool IsDirty { get; }
        Stream GetStream();
    }
}