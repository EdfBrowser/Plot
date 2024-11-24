using Plot.Core.Renderables.Axes;

namespace Plot.Core.Series.AxesMangers
{
    public interface IAxisManager
    {
        AxisLimits GetAxisLimits(AxisLimits viewLimits, AxisLimits dataLimits);
    }
}
