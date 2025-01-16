using System;

namespace Plot.Skia
{
    internal class AxisLimits : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;

            foreach (IAxis axis in axisManager.Axes)
            {
                if (!axis.RangeMutable.HasBeenSet)
                {
                    if (axis is IXAxis xAxis
                        && xAxis.LabelFormat == TickLabelFormat.DateTime)

                        axisManager.SetLimits(
                            Range.DefaultDateTime(DateTime.Now, 10), axis);
                    else
                        axisManager.SetLimits(Range.DefaultNumeric, axis);
                }
            }
        }
    }
}
