﻿using System;
using System.IO;
using LukaLukaLibrary.IO.Common;
using LukaLukaLibrary.IO.Sections;

namespace LukaLukaLibrary.IO
{
    public interface IBinaryFile : IDisposable
    {
        BinaryFileFlags Flags { get; }
        BinaryFormat Format { get; set; }
        Endianness Endianness { get; set; }

        void Read( EndianBinaryReader reader, ISection section = null );
        void Write( EndianBinaryWriter writer, ISection section = null );

        void Load( string filePath );
        void Load( Stream source, bool leaveOpen = false );
        void Save( string filePath );
        void Save( Stream destination, bool leaveOpen = false );
    }

    [Flags]
    public enum BinaryFileFlags
    {
        Load = 1,
        Save = 2,
        HasSectionFormat = 4,
        UsesSourceStream = 8,
    }
}
