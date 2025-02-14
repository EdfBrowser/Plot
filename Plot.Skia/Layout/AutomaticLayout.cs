using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class AutomaticLayout : ILayout
    {
        public (Rect, Dictionary<IAxis, (float, float)>)
            GetLayout(Figure figure, Rect figureRect)
        {
            AxisManager axisManager = figure.AxisManager;
            foreach (IAxis axis in axisManager.Axes)
            {
                if (axis.Direction.Horizontal())
                    axisManager.GenerateTicks(figureRect.Width, axis);
                else
                    axisManager.GenerateTicks(figureRect.Height, axis);
            }


            Dictionary<IAxis, float> measuredAxes = Measure(figure.AxisManager.Axes);

            (float l, float r, float t, float b) =
                CalculateEdgeTickLabel(figure.AxisManager.Axes);

            SetAxisSpacing(figure.AxisManager.Axes,
                l, r, t, b);

            IEnumerable<IAxis> leftAxes
                = figure.AxisManager.Axes.Where(x => x.Direction == Edge.Left);
            IEnumerable<IAxis> rightAxes
                = figure.AxisManager.Axes.Where(x => x.Direction == Edge.Right);
            IEnumerable<IAxis> bottomAxes
                = figure.AxisManager.Axes.Where(x => x.Direction == Edge.Bottom);
            IEnumerable<IAxis> topAxes
                = figure.AxisManager.Axes.Where(x => x.Direction == Edge.Top);


            float left = leftAxes.Any()
                ? leftAxes.Select(x => measuredAxes[x]).Max() : l;
            float right = rightAxes.Any()
                ? rightAxes.Select(x => measuredAxes[x]).Max() : r;
            float bottom = bottomAxes.Any()
                ? bottomAxes.Select(x => measuredAxes[x]).Max() : b;
            float top = topAxes.Any()
                ? topAxes.Select(x => measuredAxes[x]).Max() : t;


            float dataRectWidth
                = Math.Max(0, figureRect.Width - (left + right));
            float dataRectHeight
                = Math.Max(0, figureRect.Height - (bottom + top));

            SizeF dataSize = new SizeF(dataRectWidth, dataRectHeight);
            PointF location = new PointF(left, top);
            Rect dataRect = new Rect(location, dataSize)
                .WithPan(figureRect.Left, figureRect.Top);


            Dictionary<IAxis, (float, float)> axesInfo
                = ArrangeAxes(figure.AxisManager.Axes, dataRect);

            return (dataRect, axesInfo);
        }

        private static void CalculateOffsets(IEnumerable<IAxis> axes,
            Rect dataRect, Dictionary<IAxis, (float, float)> axesInfo)
        {
            int axisCount = axes.Count();
            if (axisCount == 0) return;

            float availableSize, plotSize, totalSpacing, lastOffset = 0;

            foreach (IAxis axis in axes)
            {
                totalSpacing = axis.AxisSpacing * (axisCount - 1);

                if (axis.Direction.Horizontal())
                {
                    availableSize = dataRect.Width - totalSpacing;
                    plotSize = availableSize / axisCount;
                }
                else
                {
                    availableSize = dataRect.Height - totalSpacing;
                    plotSize = availableSize / axisCount;
                }

                axesInfo[axis] = (lastOffset, plotSize);
                lastOffset += plotSize + axis.AxisSpacing;
            }
        }

        private static Dictionary<IAxis, (float, float)> ArrangeAxes(
            IEnumerable<IAxis> axes, Rect dataRect)
        {
            Dictionary<IAxis, (float, float)> axesInfo = new Dictionary<IAxis, (float, float)>();

            CalculateOffsets(axes.Where(x => x.Direction == Edge.Left),
                dataRect, axesInfo);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Right),
                dataRect, axesInfo);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Bottom),
                dataRect, axesInfo);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Top),
                dataRect, axesInfo);

            return axesInfo;
        }

        private static Dictionary<IAxis, float> Measure(IEnumerable<IAxis> axes)
        {
            Dictionary<IAxis, float> measuredAxes = new Dictionary<IAxis, float>();

            foreach (IAxis axis in axes)
            {
                measuredAxes[axis] = axis.Measure();
            }

            return measuredAxes;
        }

        private static (float, float, float, float)
            CalculateEdgeTickLabel(IEnumerable<IAxis> axes)
        {

            float left = 0f, right = 0f, top = 0f, bottom = 0f;
            foreach (IAxis axis in axes)
            {
                Tick first = axis.TickGenerator.Ticks.First(t => t.MajorPos);
                Tick last = axis.TickGenerator.Ticks.Last(t => t.MajorPos);

                SizeF firstLabelSize = axis.TickLabelStyle.Measure(first.Label);
                SizeF LastLabelSize = axis.TickLabelStyle.Measure(last.Label);

                if (axis.Direction.Horizontal())
                {
                    left = Math.Max(left, firstLabelSize.Width);
                    right = Math.Max(right, LastLabelSize.Width);
                }
                else
                {
                    bottom = Math.Max(bottom, firstLabelSize.Height);
                    top = Math.Max(top, LastLabelSize.Height);
                }
            }

            return (left, right, top, bottom);
        }

        private static void SetAxisSpacing(IEnumerable<IAxis> axes,
            float l, float r, float t, float b)
        {
            foreach (IAxis axis in axes)
            {
                if (axis.Direction.Horizontal())
                    axis.AxisSpacing = Math.Max(l, r);
                else
                    axis.AxisSpacing = Math.Max(t, b);
            }
        }
    }
}
