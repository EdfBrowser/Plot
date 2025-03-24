using System;

namespace Plot.Skia
{
    public interface IAxis : IDisposable
    {
        float AxisSpacing { get; set; }
        RangeMutable RangeMutable { get; }
        ITickGenerator TickGenerator { get; }
        Edge Direction { get; }

        double Min { get; }
        double Max { get; }

        LabelStyle Label { get; }
        TickStyle MajorTickStyle { get; }
        TickStyle MinorTickStyle { get; }
        LabelStyle TickLabelStyle { get; }
        LineStyle TickLineStyle { get; }

        float GetPixel(double position, Rect dataRect);
        double GetWorld(float pixel, Rect dataRect);

        void GenerateTicks(float axisLength);
        float Measure();
        void Render(RenderContext rc);
        void Render(RenderContext rc, Rect dataRect);


        Rect GetDataRect(Rect dataRect, float delta, float size);
    }
}
