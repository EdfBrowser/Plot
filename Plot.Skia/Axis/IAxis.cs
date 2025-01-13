using System;

namespace Plot.Skia
{
    public interface IAxis : IDisposable
    {
        PixelRangeMutable Range { get; }
        ITickGenerator TickGenerator { get; }
        Edge Direction { get; }

        double Min { get; }
        double Max { get; }

        LabelStyle Label { get; }
        TickStyle MajorTickStyle { get; }
        TickStyle MinorTickStyle { get; }
        LabelStyle TickLabelStyle { get; }
        LineStyle TickLineStyle { get; }

        float GetPixel(double position, PixelPanel dataPanel);
        double GetWorld(float pixel, PixelPanel dataPanel);

        void GenerateTicks(float axisLength);
        float Measure();
        void Render(RenderContext rc);
    }
}
