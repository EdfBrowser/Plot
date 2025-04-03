using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class StackedLayoutStrategy : BaseLayoutStrategy
    {
        internal StackedLayoutStrategy(Figure figure, LayoutOptions options) : base(figure, options)
        {

        }

        public override LayoutResult CalculateLayout(Rect figureRect)
        {
            CalculateCore(figureRect);

            SetAxisSpace();
            SetPanelSpace();

            return new LayoutResult(
                _dataRect,
                ArrangeAxes(_dataRect),
                ArrangePanels(_dataRect)
            );
        }


        private void SetAxisSpace()
        {
            foreach (var axis in _figure.AxisManager.Axes)
            {
                axis.Space = axis.Direction.Horizontal()
                    ? _axesEdge.Right
                    : _axesEdge.Bottom;
            }
        }

        private void SetPanelSpace()
        {
            foreach (var panel in _figure.PanelManager.Panels)
            {
                panel.Space = panel.Direction.Horizontal()
                    ? _panelsEdge.Right
                    : _panelsEdge.Bottom;
            }
        }

        protected override void CalculateAxisLayout(
           IEnumerable<IAxis> axes,
           Rect dataRect,
           Dictionary<IAxis, Rect> result)
        {
            int axisCount = axes.Count();
            if (axisCount == 0) return;

            float totalSpace = axes.Sum(x => x.Space);

            float availableHorizontalSize = dataRect.Width - totalSpace;
            float horizontalAxisLength = availableHorizontalSize / axisCount;

            float availableVerticalSize = dataRect.Height - totalSpace;
            float verticalAxisLength = availableVerticalSize / axisCount;

            float startedDelta = 0f;

            foreach (IAxis axis in axes)
            {
                bool horizontal = axis.Direction.Horizontal();
                float length = horizontal ? horizontalAxisLength : verticalAxisLength;

                float measuredValue = _axesMetrics[axis];

                switch (axis.Direction)
                {
                    case Edge.Left:
                        result[axis] = new Rect(
                            left: dataRect.Left - measuredValue,
                            right: dataRect.Left - 0f,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                        break;
                    case Edge.Right:
                        result[axis] = new Rect(
                            left: dataRect.Right + 0f,
                            right: dataRect.Right + measuredValue,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                        break;
                    case Edge.Top:
                        result[axis] = new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Top - measuredValue,
                            bottom: dataRect.Top - 0f);
                        break;
                    case Edge.Bottom:
                        result[axis] = new Rect(
                           left: dataRect.Left + startedDelta,
                           right: dataRect.Left + startedDelta + length,
                           top: dataRect.Bottom + 0f,
                           bottom: dataRect.Bottom + measuredValue);
                        break;
                    default:
                        throw new NotImplementedException(nameof(axis.Direction));
                }

                startedDelta += (length + axis.Space);
            }
        }

        protected override void CalculatePanelLayout(
           IEnumerable<IPanel> panels,
           Rect dataRect,
           float startedMargin,
           Dictionary<IPanel, Rect> result)
        {
            int panelCount = panels.Count();
            if (panelCount == 0) return;

            float totalSpace = panels.Sum(x => x.Space);

            float availableHorizontalSize = dataRect.Width - totalSpace;
            float horizontalAxisLength = availableHorizontalSize / panelCount;

            float availableVerticalSize = dataRect.Height - totalSpace;
            float verticalAxisLength = availableVerticalSize / panelCount;

            float startedDelta = 0f;

            foreach (IPanel panel in panels)
            {
                bool horizontal = panel.Direction.Horizontal();
                float length = horizontal ? horizontalAxisLength : verticalAxisLength;

                switch (panel.Direction)
                {
                    case Edge.Left:
                        result[panel] = new Rect(
                            left: dataRect.Left - startedMargin - _options.ColorBarWidth,
                            right: dataRect.Left - startedMargin,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                        break;
                    case Edge.Right:
                        result[panel] = new Rect(
                            left: dataRect.Right + startedMargin,
                            right: dataRect.Right + startedMargin + _options.ColorBarWidth,
                            top: dataRect.Top + startedDelta,
                            bottom: dataRect.Top + startedDelta + length);
                        break;
                    case Edge.Top:
                        result[panel] = new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Top - startedMargin - _options.ColorBarWidth,
                            bottom: dataRect.Top - startedMargin);
                        break;
                    case Edge.Bottom:
                        result[panel] = new Rect(
                            left: dataRect.Left + startedDelta,
                            right: dataRect.Left + startedDelta + length,
                            top: dataRect.Bottom + startedMargin,
                            bottom: dataRect.Bottom + startedMargin + _options.ColorBarWidth);
                        break;
                    default:
                        throw new NotImplementedException(nameof(panel.Direction));
                }

                startedDelta += (length + panel.Space);
            }
        }
    }
}
