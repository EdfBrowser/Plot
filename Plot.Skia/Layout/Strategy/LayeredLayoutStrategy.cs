using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class LayeredLayoutStrategy : BaseLayoutStrategy
    {
        internal LayeredLayoutStrategy(LayoutOptions options) : base(options)
        {

        }

        public override LayoutResult CalculateLayout(Figure figure, Rect figureRect)
        {
            GenerateTick(figure.AxisManager.Axes, figureRect);

            Dictionary<IAxis, float> axesMetrics = MeasureAxes(figure.AxisManager.Axes);
            Dictionary<IPanel, float> panelsMetrics = MeasurePanels(figure.PanelManager.Panels);

            var axesEdge = CalculateAxesEdge(figure.AxisManager.Axes);
            var panelsEdge = CalculatePanelsEdge(figure.PanelManager.Panels);

            var axesOffset = CalculateAxisOffsets(figure.AxisManager.Axes, axesMetrics, axesEdge);
            var panelsOffset = CalculatePanelOffsets(figure.PanelManager.Panels, panelsMetrics, panelsEdge);
            var dataRect = CalculateDataRect(figureRect, axesOffset, panelsOffset);

            return new LayoutResult(
                dataRect,
                ArrangeAxes(figure.AxisManager.Axes, dataRect, axesMetrics),
                ArrangePanels(figure.PanelManager.Panels, dataRect, panelsMetrics, axesOffset)
            );
        }

        private EdgeOffsets CalculateAxisOffsets(IEnumerable<IAxis> axes,
           IReadOnlyDictionary<IAxis, float> axesMetrics,
           EdgeOffsets edges)
        {
            var leftAxes = axes.Where(x => x.Direction == Edge.Left).ToList();
            var rightAxes = axes.Where(x => x.Direction == Edge.Right).ToList();
            var bottomAxes = axes.Where(x => x.Direction == Edge.Bottom).ToList();
            var topAxes = axes.Where(x => x.Direction == Edge.Top).ToList();

            return new EdgeOffsets(
                left: leftAxes.Count > 0 ? leftAxes.Sum(x => axesMetrics[x]) : edges.Left,
                right: rightAxes.Count > 0 ? rightAxes.Sum(x => axesMetrics[x]) : edges.Right,
                bottom: bottomAxes.Count > 0 ? bottomAxes.Sum(x => axesMetrics[x]) : edges.Bottom,
                top: topAxes.Count > 0 ? topAxes.Sum(x => axesMetrics[x]) : edges.Top
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
               left: leftPanels.Count > 0 ? leftPanels.Sum(x => panelsMetrics[x]) : edges.Left,
               right: rightPanels.Count > 0 ? rightPanels.Sum(x => panelsMetrics[x]) : edges.Right,
               bottom: bottomPanels.Count > 0 ? bottomPanels.Sum(x => panelsMetrics[x]) : edges.Bottom,
               top: topPanels.Count > 0 ? topPanels.Sum(x => panelsMetrics[x]) : edges.Top
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

        private Dictionary<IAxis, Rect> ArrangeAxes(IEnumerable<IAxis> axes,
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
            float startedDelta = 0f;
            foreach (IAxis axis in axes)
            {
                switch (axis.Direction)
                {
                    case Edge.Left:
                        result[axis] = new Rect(
                            left: dataRect.Left - startedDelta - axesMetrics[axis],
                            right: dataRect.Left - startedDelta,
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Right:
                        result[axis] = new Rect(
                            left: dataRect.Right + startedDelta,
                            right: dataRect.Right + startedDelta + axesMetrics[axis],
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Top:
                        result[axis] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Top - startedDelta - axesMetrics[axis],
                            bottom: dataRect.Top - startedDelta);
                        break;
                    case Edge.Bottom:
                        result[axis] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Bottom + startedDelta,
                            bottom: dataRect.Bottom + startedDelta + axesMetrics[axis]);
                        break;
                }

                startedDelta += axesMetrics[axis];
            }
        }

        private Dictionary<IPanel, Rect> ArrangePanels(
            IEnumerable<IPanel> panels,
            Rect dataRect,
            IReadOnlyDictionary<IPanel, float> panelMetrics,
            EdgeOffsets axesOffset)
        {
            var result = new Dictionary<IPanel, Rect>(GetCollectionCapacity(panels));

            var group = panels.GroupBy(x => x.Direction).Select(g => (g.Key, g.AsEnumerable()));
            foreach (var (direction, many) in group)
            {
                float startedMargin = GetInitialOffset(direction, axesOffset);
                CalculatePanelLayout(many, dataRect, startedMargin, panelMetrics, result);
            }

            return result;
        }

        private void CalculatePanelLayout(
           IEnumerable<IPanel> panels,
           Rect dataRect,
           float startedMargin,
           IReadOnlyDictionary<IPanel, float> panelMetrics,
           Dictionary<IPanel, Rect> result)
        {
            float startedDelta = 0f;
            foreach (IPanel panel in panels)
            {
                switch (panel.Direction)
                {
                    case Edge.Left:
                        result[panel] = new Rect(
                            left: dataRect.Left - startedMargin - startedDelta - _options.ColorBarWidth,
                            right: dataRect.Left - startedMargin - startedDelta,
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Right:
                        result[panel] = new Rect(
                            left: dataRect.Right + startedMargin + startedDelta,
                            right: dataRect.Right + startedMargin + startedDelta + _options.ColorBarWidth,
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Top:
                        result[panel] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Top - startedMargin - startedDelta - _options.ColorBarWidth,
                            bottom: dataRect.Top - startedMargin - startedDelta);
                        break;
                    case Edge.Bottom:
                        result[panel] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Bottom + startedMargin + startedDelta,
                            bottom: dataRect.Bottom + startedMargin + startedDelta + _options.ColorBarWidth);
                        break;
                }

                startedDelta += panelMetrics[panel];
            }
        }
    }
}
