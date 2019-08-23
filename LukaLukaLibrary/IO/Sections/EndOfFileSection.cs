using LukaLukaLibrary.IO.Common;

namespace LukaLukaLibrary.IO.Sections
{
    [Section( "EOFC" )]
    public class EndOfFileSection : Section<object>
    {
        public override SectionFlags Flags => SectionFlags.None;
        public override Endianness Endianness => Endianness.LittleEndian;
        public override AddressSpace AddressSpace => AddressSpace.Int32;

        protected override void Read( object dataObject, EndianBinaryReader reader, long length )
        {
        }

        protected override void Write( object dataObject, EndianBinaryWriter writer )
        {
        }

        public EndOfFileSection( SectionMode mode, object dataObject = null ) : base( mode, dataObject )
        {
        }
    }
}
