using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class StackedLayoutStrategy : BaseLayoutStrategy
    {
        internal StackedLayoutStrategy(LayoutOptions options) : base(options)
        {

        }

        public override LayoutResult CalculateLayout(Figure figure, Rect figureRect)
        {
            GenerateTick(figure.AxisManager.Axes, figureRect);

            Dictionary<IAxis, float> axesMetrics = MeasureAxes(figure.AxisManager.Axes);
            Dictionary<IPanel, float> panelsMetrics = MeasurePanels(figure.PanelManager.Panels);

            var axesEdge = CalculateAxesEdge(figure.AxisManager.Axes);
            var panelsEdge = CalculatePanelsEdge(figure.PanelManager.Panels);
            SetAxisSpace(figure.AxisManager.Axes, axesEdge);
            SetPanelSpace(figure.PanelManager.Panels, panelsEdge);

            var axesOffset = CalculateAxisOffsets(figure.AxisManager.Axes, axesMetrics, axesEdge);
            var panelsOffset = CalculatePanelOffsets(figure.PanelManager.Panels, panelsMetrics, panelsEdge);
            var dataRect = CalculateDataRect(figureRect, axesOffset, panelsOffset);

            return new LayoutResult(
                dataRect,
                ArrangeAxes(figure.AxisManager.Axes, dataRect, axesMetrics),
                ArrangePanels(figure.PanelManager.Panels, dataRect, axesOffset)
            );
        }


        private void SetAxisSpace(IEnumerable<IAxis> axes, EdgeOffsets edges)
        {
            foreach (var axis in axes)
            {
                axis.AxisSpace = axis.Direction.Horizontal()
                    ? Math.Max(edges.Left, edges.Right)
                    : Math.Max(edges.Top, edges.Bottom);
            }
        }

        private void SetPanelSpace(IEnumerable<IPanel> panels, EdgeOffsets edges)
        {
            foreach (var panel in panels)
            {
                panel.PanelSpace = panel.Direction.Horizontal()
                    ? Math.Max(edges.Left, edges.Right)
                    : Math.Max(edges.Top, edges.Bottom);
            }
        }


        private EdgeOffsets CalculateAxisOffsets(IEnumerable<IAxis> axes,
            IReadOnlyDictionary<IAxis, float> axesMetrics,
            EdgeOffsets edgeOffsets)
        {
            var leftAxes = axes.Where(x => x.Direction == Edge.Left).ToList();
            var rightAxes = axes.Where(x => x.Direction == Edge.Right).ToList();
            var bottomAxes = axes.Where(x => x.Direction == Edge.Bottom).ToList();
            var topAxes = axes.Where(x => x.Direction == Edge.Top).ToList();

            return new EdgeOffsets(
                left: leftAxes.Count > 0 ? leftAxes.Max(x => axesMetrics[x]) : edgeOffsets.Left,
                right: rightAxes.Count > 0 ? rightAxes.Max(x => axesMetrics[x]) : edgeOffsets.Right,
                bottom: bottomAxes.Count > 0 ? bottomAxes.Max(x => axesMetrics[x]) : edgeOffsets.Bottom,
                top: topAxes.Count > 0 ? topAxes.Max(x => axesMetrics[x]) : edgeOffsets.Top
            );
        }

        private EdgeOffsets CalculatePanelOffsets(IEnumerable<IPanel> panels,
            IReadOnlyDictionary<IPanel, float> panelsMetrics,
            EdgeOffsets edges)
        {
            var leftPanels = panels.Where(x => x.Direction == Edge.Left).ToList();
            var rightPanels = panels.Where(x => x.Direction == Edge.Right).ToList();
            var bottomPanels = panels.Where(x => x.Direction == Edge.Bottom).ToList();
            var topPanels = panels.Where(x => x.Direction == Edge.Top).ToList();

            return new EdgeOffsets(
               left: leftPanels.Count > 0 ? leftPanels.Max(x => panelsMetrics[x]) : edges.Left,
               right: rightPanels.Count > 0 ? rightPanels.Max(x => panelsMetrics[x]) : edges.Right,
               bottom: bottomPanels.Count > 0 ? bottomPanels.Max(x => panelsMetrics[x]) : edges.Bottom,
               top: topPanels.Count > 0 ? topPanels.Max(x => panelsMetrics[x]) : edges.Top
           );
        }

        private Rect CalculateDataRect(Rect figureRect,
            EdgeOffsets axesOffset,
            EdgeOffsets panelsOffset)
        {
            float wOffset = axesOffset.Left + panelsOffset.Left + axesOffset.Right + panelsOffset.Right;
            float hOffset = axesOffset.Top + panelsOffset.Top + axesOffset.Bottom + panelsOffset.Bottom;

            float dataRectWidth
                = Math.Max(0, figureRect.Width - wOffset);
            float dataRectHeight
                = Math.Max(0, figureRect.Height - hOffset);

            SizeF dataSize = new SizeF(dataRectWidth, dataRectHeight);
            PointF location = new PointF(axesOffset.Left + panelsOffset.Left, axesOffset.Top + panelsOffset.Top);

            return new Rect(location, dataSize)
                .WithPan(figureRect.Left, figureRect.Top);
        }

        private Dictionary<IAxis, Rect> ArrangeAxes(
            IEnumerable<IAxis> axes,
            Rect dataRect,
            IReadOnlyDictionary<IAxis, float> axesMetrics)
        {
            var result = new Dictionary<IAxis, Rect>(GetCollectionCapacity(axes));
            foreach (var group in axes.GroupBy(x => x.Direction))
            {
                CalculateAxisLayout(group, dataRect, axesMetrics, result);
            }

            return result;
        }

        private void CalculateAxisLayout(
           IEnumerable<IAxis> axes,
           Rect dataRect,
           IReadOnlyDictionary<IAxis, float> axesMetrics,
           Dictionary<IAxis, Rect> result)
        {
            int axisCount = axes.Count();
            if (axisCount == 0) return;

            float totalSpace = axes.Sum(x => x.AxisSpace);

            float availableHorizontalSize = dataRect.Width - totalSpace;
            float horizontalAxisLength = availableHorizontalSize / axisCount;

            float availableVerticalSize = dataRect.Height - totalSpace;
            float verticalAxisLength = availableVerticalSize / axisCount;

            float startedDelta = 0f;

            foreach (IAxis axis in axes)
            {
                bool horizontal = axis.Direction.Horizontal();
                float length = horizontal ? horizontalAxisLength : verticalAxisLength;

                if (horizontal)
                {
                    result[axis] = axis.Direction == Edge.Bottom
                        ? new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Bottom + 0f,
                            bottom: dataRect.Bottom + axesMetrics[axis])
                        : new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Top - axesMetrics[axis],
                            bottom: dataRect.Top - 0f);
                }
                else
                {
                    result[axis] = axis.Direction == Edge.Left
                       ? new Rect(
                            left: dataRect.Left - axesMetrics[axis],
                            right: dataRect.Left - 0f,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length)
                       : new Rect(
                            left: dataRect.Right + 0f,
                            right: dataRect.Right + axesMetrics[axis],
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                }

                startedDelta += (length + axis.AxisSpace);
            }
        }

        private Dictionary<IPanel, Rect> ArrangePanels(
            IEnumerable<IPanel> panels,
            Rect dataRect,
            EdgeOffsets axesOffset)
        {
            var result = new Dictionary<IPanel, Rect>(GetCollectionCapacity(panels));

            var group = panels.GroupBy(x => x.Direction).Select(g => (g.Key, g.AsEnumerable()));
            foreach (var (direction, many) in group)
            {
                float offset = GetInitialOffset(direction, axesOffset);
                CalculatePanelLayout(many, dataRect, offset, result);
            }

            return result;
        }

        private void CalculatePanelLayout(
           IEnumerable<IPanel> panels,
           Rect dataRect,
           float startedMargin,
           Dictionary<IPanel, Rect> result)
        {
            int panelCount = panels.Count();
            if (panelCount == 0) return;

            float totalSpace = panels.Sum(x => x.PanelSpace);

            float availableHorizontalSize = dataRect.Width - totalSpace;
            float horizontalAxisLength = availableHorizontalSize / panelCount;

            float availableVerticalSize = dataRect.Height - totalSpace;
            float verticalAxisLength = availableVerticalSize / panelCount;

            float startedDelta = 0f;

            foreach (IPanel panel in panels)
            {
                bool horizontal = panel.Direction.Horizontal();
                float length = horizontal ? horizontalAxisLength : verticalAxisLength;

                if (horizontal)
                {
                    result[panel] = panel.Direction == Edge.Bottom
                        ? new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Bottom + startedMargin,
                            bottom: dataRect.Bottom + startedMargin + _options.ColorBarWidth)
                        : new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Top - startedMargin - _options.ColorBarWidth,
                            bottom: dataRect.Top - startedMargin);
                }
                else
                {
                    result[panel] = panel.Direction == Edge.Left
                       ? new Rect(
                            left: dataRect.Left - startedMargin - _options.ColorBarWidth,
                            right: dataRect.Left - startedMargin,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length)
                       : new Rect(
                            left: dataRect.Right + startedMargin,
                            right: dataRect.Right + startedMargin + _options.ColorBarWidth,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                }

                startedDelta += (length + panel.PanelSpace);
            }
        }
    }
}
