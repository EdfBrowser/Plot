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
                if (!axis.Range.HasBeenSet)
                {
                    if (axis is IXAxis xAxis
                        && xAxis.LabelFormat == TickLabelFormat.DateTime)

                        axisManager.SetLimits(
                            PixelRange.DefaultDateTime(DateTime.Now), axis);
                    else
                        axisManager.SetLimits(PixelRange.DefaultNumeric, axis);
                }

                if (axis is IXAxis)
                {
                    ScrollMode((IXAxis)axis, rc.Figure.AxisManager);
                }
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
                    break;
            }

            if (axis.ScrollPosition > axis.Max)
            {
                PixelRange pixelRange = new PixelRange(min, max);
                axisManager.SetLimits(pixelRange, axis);

                if (axis.ScrollMode == AxisScrollMode.Sweeping)
                    axis.Animate = true;
            }
        }
    }
}
