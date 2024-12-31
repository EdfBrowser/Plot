using Plot.Skia.Enums;
using Plot.Skia.Structs;

namespace Plot.Skia.Axes
{
    internal class BottomAxis : XAxisBase
    {
        public BottomAxis()
        {
            Direction = Edge.Bottom;
        }

        public override Edge Direction { get; set; }
    }
}
