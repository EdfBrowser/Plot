using Plot.Core.Renderables.Axes;
using System.Drawing;

namespace Plot.Core.Series
{
    public interface IPlotSeries
    {
        Axis XAxis { get; }
        Axis YAxis { get; }

        Color Color { get; set; }
        float LineWidth { get; set; }
        string Label { get; set; }

        AxisLimits GetAxisLimits();

        void ValidateData();

        void Plot(Bitmap bmp, bool lowQuality, float scale);
    }
}
