using System;

namespace Plot.Skia
{
    public interface IAxis : IRenderable, IMeasureable, IDisposable
    {
        float AxisSpace { get; set; }
        RangeMutable RangeMutable { get; }
        ITickGenerator TickGenerator { get; }
        Edge Direction { get; }

        double Min { get; }
        double Max { get; }

        LabelStyle Label { get; }
        LineStyle MajorTickStyle { get; }
        LineStyle MinorTickStyle { get; }
        LabelStyle TickLabelStyle { get; }
        LineStyle TickLineStyle { get; }

        float GetPixel(double position, Rect dataRect);
        double GetWorld(float pixel, Rect dataRect);

        void GenerateTicks(float axisLength);
    }
}
