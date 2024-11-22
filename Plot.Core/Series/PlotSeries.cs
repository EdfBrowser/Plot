using Plot.Core.Renderables.Axes;
using System.Drawing;

namespace Plot.Core.Series
{
    public abstract class PlotSeries
    {
        internal abstract AxisLimits GetAxisLimits();

        internal abstract void ValidateData();

        internal abstract void Plot(Bitmap bmp, PlotDimensions dims, bool lowQuailty);
    }
}
