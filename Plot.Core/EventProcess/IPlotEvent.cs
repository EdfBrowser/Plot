using Plot.Core.Renderables.Axes;

namespace Plot.Core.EventProcess
{
    public interface IPlotEvent
    {
        void Process(AxisManager axisManager);
    }
}
