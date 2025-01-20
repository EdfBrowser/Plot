using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class ScaleAxis : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;
            IEnumerable<ISeries> series = rc.Figure.SeriesManager.Series;
            if (series.Any())
                AutoScaleSeries(series, axisManager);
            else
                ApplyDefaultLimits(axisManager);

        }

        private void AutoScaleSeries(IEnumerable<ISeries> series,
            AxisManager axisManager)
        {
            foreach (ISeries item in series)
            {
                if (!item.X.RangeMutable.HasBeenSet)
                {
                    RangeMutable xLimit = item.GetXLimit();

                    if (xLimit.Low == xLimit.High)
                        xLimit.Set(xLimit.Low - 1, xLimit.High + 1);

                    double left = xLimit.Low - (xLimit.Span * .1 / 2.0);
                    double right = xLimit.High + (xLimit.Span * .1 / 2.0);

                    xLimit.Set(left, right);

                    axisManager.SetLimits(xLimit.ToRange, item.X);
                }

                if (!item.Y.RangeMutable.HasBeenSet)
                {
                    RangeMutable yLimit = item.GetYLimit();

                    if (yLimit.Low == yLimit.High)
                        yLimit.Set(yLimit.Low - 1, yLimit.High + 1);

                    double bottom = yLimit.Low - (yLimit.Span * .15 / 2.0);
                    double top = yLimit.High + (yLimit.Span * .15 / 2.0);

                    yLimit.Set(bottom, top);

                    axisManager.SetLimits(yLimit.ToRange, item.Y);
                }
            }
        }

        private void ApplyDefaultLimits(AxisManager axisManager)
        {
            foreach (IAxis axis in axisManager.Axes)
            {
                if (!axis.RangeMutable.HasBeenSet)
                    axisManager.SetLimits(Range.DefaultNumeric, axis);
            }
        }
    }
}
