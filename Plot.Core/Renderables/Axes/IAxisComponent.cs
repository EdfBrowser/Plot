using Plot.Core.Enum;

namespace Plot.Core.Renderables.Axes
{
    public interface IAxisComponent
    {
        Edge Edge { get; set; }
        bool RulerMode { get; set; }
        float PixelOffset { get; set; }
    }
}