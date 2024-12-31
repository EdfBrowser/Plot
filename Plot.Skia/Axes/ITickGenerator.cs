using Plot.Skia.Enums;
using Plot.Skia.Structs;

namespace Plot.Skia.Axes
{
    internal interface ITickGenerator
    {
        Tick[] Tick { get; set; }

        void GenerateTick(Edge edge, double range, float size);
    }
}
