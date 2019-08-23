using LukaLukaLibrary.Motions;

namespace LukaLukaLibrary.IO.Sections
{
    [Section( "MOTC" )]
    public class MotionSection : BinaryFileSection<Motion>
    {
        public override SectionFlags Flags => SectionFlags.HasRelocationTable;

        public MotionSection( SectionMode mode, Motion dataObject = null ) : base( mode, dataObject )
        {
        }
    }
}