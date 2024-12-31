using Plot.Skia.Enums;
using Plot.Skia.Structs;

namespace Plot.Skia.Axes
{
    internal interface IAxis
    {
        double Min { get; set; }
        double Max { get; set; }

        Edge Direction { get; set; }

        LabelStyle Label { get; set; }

        TickStyle MajorTickStyle { get; set; }
        TickStyle MinorTickStyle { get; set; }

        LabelStyle TickLabelStyle { get; set; }
        LineStyle TickLineStyle { get; set; }

        float GetPixel(double position, PixelPanel dataPanel);
        double GetWorld(float pixel, PixelPanel dataPanel);
    }
}
