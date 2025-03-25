using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    internal class ScaleAxis : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;
            IEnumerable<ISeries> series = rc.Figure.SeriesManager.Series;

            if (rc.Figure.RenderManager.FittedY)
            {
                FitAxis(series, axisManager, item => item.Y, item => item.GetYLimit(), 0.15);
                rc.Figure.RenderManager.FittedY = false;
            }

            if (rc.Figure.RenderManager.FittedX)
            {
                FitAxis(series, axisManager, item => item.X, item => item.GetXLimit(), 0.1);
                rc.Figure.RenderManager.FittedX = false;
            }

            // 如果没有上面的设置，进行默认初始化
            ApplyDefaultLimits(axisManager);
        }

        private void FitAxis<TAxis>(IEnumerable<ISeries> series, AxisManager axisManager,
                            Func<ISeries, TAxis> getAxis, Func<ISeries, RangeMutable> getLimit, 
                            double expandFactor) where TAxis : IAxis
        {
            Dictionary<TAxis, RangeMutable> axisLimits = new Dictionary<TAxis, RangeMutable>();

            foreach (ISeries item in series)
            {
                TAxis axis = getAxis(item);
                if (axis == null) continue; // 添加空引用检查

                RangeMutable range = getLimit(item);
                if (!range.Valid) continue;

                if (axisLimits.ContainsKey(axis))
                {
                    RangeMutable currentLimit = axisLimits[axis];
                    currentLimit.Low = Math.Min(currentLimit.Low, range.Low);
                    currentLimit.High = Math.Max(currentLimit.High, range.High);
                }
                else
                {
                    axisLimits[axis] = new RangeMutable(range.Low, range.High);
                }
            }

            foreach (var axisLimit in axisLimits)
            {
                RangeMutable limit = axisLimit.Value;
                limit.Expand(expandFactor);
                axisManager.SetLimits(limit.ToRange, axisLimit.Key);
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
