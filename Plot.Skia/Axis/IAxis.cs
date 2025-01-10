using System;

namespace Plot.Skia
{
    internal interface IAxis : IDisposable
    {
        PixelRangeMutable Range { get; }
        ITickGenerator TickGenerator { get; }
        Edge Direction { get; }

        double Min { get; set; }
        double Max { get; set; }

        LabelStyle Label { get; set; }
        TickStyle MajorTickStyle { get; set; }
        TickStyle MinorTickStyle { get; set; }
        LabelStyle TickLabelStyle { get; set; }
        LineStyle TickLineStyle { get; set; }

        float GetPixel(double position, PixelPanel dataPanel);
        double GetWorld(float pixel, PixelPanel dataPanel);

        void GenerateTicks(float axisLength);
        float Measure();
        void Render(RenderContext rc);
    }
}
