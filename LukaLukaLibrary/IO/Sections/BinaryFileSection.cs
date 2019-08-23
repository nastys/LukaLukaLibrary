﻿using LukaLukaLibrary.IO.Common;

namespace LukaLukaLibrary.IO.Sections
{
    public abstract class BinaryFileSection<T> : Section<T> where T : IBinaryFile, new()
    {
        protected override void Read( T dataObject, EndianBinaryReader reader, long length )
        {
            dataObject.Format = Format;
            dataObject.Endianness = Endianness;
            {
                dataObject.Read( reader, this );
            }
        }

        protected override void Write( T dataObject, EndianBinaryWriter writer ) => dataObject.Write( writer, this );

        protected BinaryFileSection( SectionMode mode, T dataObject = default( T ) ) : base( mode, dataObject )
        {
            if ( mode != SectionMode.Write )
                return;

            Endianness = dataObject.Endianness;
            AddressSpace = BinaryFormatUtilities.GetAddressSpace( dataObject.Format );
        }
    }
}
