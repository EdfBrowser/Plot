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
                    if (axis is IXAxis
                        && ((IXAxis)axis).LabelFormat == TickLabelFormat.DateTime)

                        axisManager.SetLimits(
                            Range.DefaultDateTime(DateTime.Now, 10), axis);
                    else
                        axisManager.SetLimits(Range.DefaultNumeric, axis);
                }

                if (axis is IXAxis && ((IXAxis)axis).ScrollPosition > axis.Max)
                    ScrollMode(((IXAxis)axis), rc.Figure.AxisManager);
            }
        }

        private void ScrollMode(IXAxis axis, AxisManager axisManager)
        {
            double min = 0, max = 0;

            switch (axis.ScrollMode)
            {
                case AxisScrollMode.Stepping:
                    max = axis.ScrollPosition + axis.Width * 0.15;
                    min = max - axis.Width;
                    break;
                case AxisScrollMode.Scrolling:
                    max = axis.ScrollPosition;
                    min = max - axis.Width;
                    break;
                case AxisScrollMode.Sweeping:
                    max = axis.ScrollPosition + axis.Width;
                    min = axis.ScrollPosition;
                    axis.Animate = true;
                    break;
            }

            Range pixelRange = new Range(min, max);
            axisManager.SetLimits(pixelRange, axis);
        }
    }
}
