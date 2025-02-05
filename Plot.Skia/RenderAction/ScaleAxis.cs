using System;
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

        private void AdjustLimit(RangeMutable limit, double defaultMin, double defaultMax)
        {
            if (!limit.HasBeenSet)
                limit.Set(defaultMin, defaultMax);
        }

        private void ApplyPadding(RangeMutable limit, double paddingRatio)
        {
            double padding = limit.Span * paddingRatio / 2.0;
            limit.Set(limit.Low - padding, limit.High + padding);
        }

        private void ProcessAxis(AxisManager axisManager,
            IAxis axis, double paddingRatio, Func<RangeMutable> getLimit)
        {
            RangeMutable limit = getLimit();

            AdjustLimit(limit, -10, 10);
            ApplyPadding(limit, paddingRatio);
            axisManager.SetLimits(limit.ToRange, axis);
        }

        private void AutoScaleSeries(IEnumerable<ISeries> series,
            AxisManager axisManager)
        {
            foreach (ISeries item in series)
            {
                if (item.X.RangeMutable.HasBeenSet && item.Y.RangeMutable.HasBeenSet)
                    continue;

                ProcessAxis(axisManager, item.X, 0.1, item.GetXLimit);
                ProcessAxis(axisManager, item.Y, 0.15, item.GetYLimit);
            }

            ApplyDefaultLimits(axisManager);
        }

        private void ApplyDefaultLimits(AxisManager axisManager)
        {
            foreach (IAxis axis in axisManager.Axes)
            {
                if (!axis.RangeMutable.HasBeenSet)
                    axisManager.SetLimits(Range.Default, axis);
            }
        }
    }
}
