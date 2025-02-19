using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class AutomaticLayout : ILayout
    {
        public (Rect,
            Dictionary<IAxis, (float, float)>,
            Dictionary<IPanel, (float, float)>)
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


            Dictionary<IAxis, float> measuredAxes
                = MeasureAxes(figure.AxisManager.Axes);
            Dictionary<IPanel, float> measuredPanels
                = MeasurePanels(figure.PanelManager.Panels);

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

            // TODO: 是否修改成最后一个刻度字符串和measured方法的值来进行比较？
            float leftOffsetAxis = leftAxes.Any()
                ? leftAxes.Select(x => measuredAxes[x]).Max() : l;
            float rightOffsetAxis = rightAxes.Any()
                ? rightAxes.Select(x => measuredAxes[x]).Max() : r;
            float bottomOffsetAxis = bottomAxes.Any()
                ? bottomAxes.Select(x => measuredAxes[x]).Max() : b;
            float topOffsetAxis = topAxes.Any()
                ? topAxes.Select(x => measuredAxes[x]).Max() : t;

            // TODO: 优化加入Panel后的计算
            IEnumerable<IPanel> leftPanels
              = figure.PanelManager.Panels.Where(x => x.Direction == Edge.Left);
            IEnumerable<IPanel> rightPanels
                = figure.PanelManager.Panels.Where(x => x.Direction == Edge.Right);
            IEnumerable<IPanel> bottomPanels
                = figure.PanelManager.Panels.Where(x => x.Direction == Edge.Bottom);
            IEnumerable<IPanel> topPanels
                = figure.PanelManager.Panels.Where(x => x.Direction == Edge.Top);

            float leftOffsetPanel = leftPanels.Any()
                ? leftPanels.Select(x => measuredPanels[x]).Sum() : 0;
            float rightOffsetPanel = rightPanels.Any()
                ? rightPanels.Select(x => measuredPanels[x]).Sum() : 0;
            float bottomOffsetPanel = bottomPanels.Any()
                ? bottomPanels.Select(x => measuredPanels[x]).Sum() : 0;
            float topOffsetPanel = topPanels.Any()
                ? topPanels.Select(x => measuredPanels[x]).Sum() : 0;

            float wOffset = leftOffsetAxis + leftOffsetPanel + rightOffsetAxis + rightOffsetPanel;
            float hOffset = topOffsetAxis + topOffsetPanel + bottomOffsetAxis + bottomOffsetPanel;

            float dataRectWidth
                = Math.Max(0, figureRect.Width - wOffset);
            float dataRectHeight
                = Math.Max(0, figureRect.Height - hOffset);

            SizeF dataSize = new SizeF(dataRectWidth, dataRectHeight);
            PointF location = new PointF((leftOffsetAxis + leftOffsetPanel), (topOffsetAxis + topOffsetPanel));
            Rect dataRect = new Rect(location, dataSize)
                // TODO: 移动到0，0还是-1，-1？
                .WithPan(figureRect.Left, figureRect.Top);


            Dictionary<IAxis, (float, float)> axesInfo
                = ArrangeAxes(figure.AxisManager.Axes, dataRect);

            Dictionary<IPanel, (float, float)> panelsInfo
                = ArrangePanels(
                    figure.PanelManager.Panels,
                    measuredPanels,
                    (leftOffsetAxis, rightOffsetAxis, bottomOffsetAxis, topOffsetAxis));

            return (dataRect, axesInfo, panelsInfo);
        }

        private static void CalculateAxisOffsets(IEnumerable<IAxis> axes,
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

        private static void CalculatePanelOffsets(
           IEnumerable<IPanel> panels,
           float offset,
           Dictionary<IPanel, float> measuredPanels,
           Dictionary<IPanel, (float, float)> panelsInfo)
        {
            float delta = offset;
            foreach (IPanel panel in panels)
            {
                panelsInfo[panel] = (delta, measuredPanels[panel]);
                delta += measuredPanels[panel];
            }
        }


        private static Dictionary<IAxis, (float, float)> ArrangeAxes(
            IEnumerable<IAxis> axes, Rect dataRect)
        {
            Dictionary<IAxis, (float, float)> axesInfo = new Dictionary<IAxis, (float, float)>();

            CalculateAxisOffsets(axes.Where(x => x.Direction == Edge.Left),
                dataRect, axesInfo);
            CalculateAxisOffsets(axes.Where(x => x.Direction == Edge.Right),
                dataRect, axesInfo);
            CalculateAxisOffsets(axes.Where(x => x.Direction == Edge.Bottom),
                dataRect, axesInfo);
            CalculateAxisOffsets(axes.Where(x => x.Direction == Edge.Top),
                dataRect, axesInfo);

            return axesInfo;
        }

        private static Dictionary<IPanel, (float, float)> ArrangePanels(
           IEnumerable<IPanel> panels,
           Dictionary<IPanel, float> measuredPanels,
           (float l, float r, float b, float t) axisOffset)
        {
            Dictionary<IPanel, (float, float)> panelsInfo = new Dictionary<IPanel, (float, float)>();

            CalculatePanelOffsets(panels.Where(x => x.Direction == Edge.Left),
               axisOffset.l, measuredPanels, panelsInfo);
            CalculatePanelOffsets(panels.Where(x => x.Direction == Edge.Right),
               axisOffset.r, measuredPanels, panelsInfo);
            CalculatePanelOffsets(panels.Where(x => x.Direction == Edge.Bottom),
               axisOffset.b, measuredPanels, panelsInfo);
            CalculatePanelOffsets(panels.Where(x => x.Direction == Edge.Top),
               axisOffset.t, measuredPanels, panelsInfo);

            return panelsInfo;
        }

        private static Dictionary<IAxis, float> MeasureAxes(IEnumerable<IAxis> axes)
        {
            Dictionary<IAxis, float> measuredAxes = new Dictionary<IAxis, float>();

            foreach (IAxis axis in axes)
            {
                measuredAxes[axis] = axis.Measure();
            }

            return measuredAxes;
        }

        private static Dictionary<IPanel, float> MeasurePanels(IEnumerable<IPanel> panels)
        {
            Dictionary<IPanel, float> measuredPanels = new Dictionary<IPanel, float>();

            foreach (IPanel panel in panels)
            {
                measuredPanels[panel] = panel.Measure();
            }

            return measuredPanels;
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
