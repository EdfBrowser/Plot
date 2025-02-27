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
            // 初次初始化X轴，后续继续初始化Y轴
            if (rc.Figure.RenderManager.FitY)
            {
                AutoScaleSeries(series, axisManager);
                rc.Figure.RenderManager.FitY = false;
            }
            else
                ApplyDefaultLimits(axisManager);
        }

        private void ProcessAxis(AxisManager axisManager,
            IAxis axis, double paddingRatio, Func<RangeMutable> getLimit)
        {
            RangeMutable limit = getLimit();

            if (!limit.Valid)
                limit.Set(0, 10);
            else
                limit.Expand(paddingRatio);

            axisManager.SetLimits(limit.ToRange, axis);
        }

        private void AutoScaleSeries(IEnumerable<ISeries> series,
            AxisManager axisManager)
        {
            foreach (ISeries item in series)
            {
                if (!item.X.RangeMutable.HasBeenSet)
                    ProcessAxis(axisManager, item.X, 0.1, item.GetXLimit);

                ProcessAxis(axisManager, item.Y, 0.15, item.GetYLimit);
            }
        }

        private void ApplyDefaultLimits(AxisManager axisManager)
        {
            foreach (IAxis axis in axisManager.Axes)
            {
                if (!axis.RangeMutable.HasBeenSet)
                    axisManager.SetLimits(new Range(0, 10), axis);
            }
        }
    }
}
