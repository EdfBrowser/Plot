using System.Collections.Generic;

namespace Plot.Skia
{
    internal class LayeredLayoutStrategy : BaseLayoutStrategy
    {
        internal LayeredLayoutStrategy(Figure figure, LayoutOptions options) : base(figure, options)
        {

        }

        public override LayoutResult CalculateLayout(Rect figureRect)
        {
            CalculateCore(figureRect);

            return new LayoutResult(
                _dataRect,
                ArrangeAxes(_dataRect),
                ArrangePanels(_dataRect)
            );
        }

        protected override void CalculateAxisLayout(
           IEnumerable<IAxis> axes,
           Rect dataRect,
           Dictionary<IAxis, Rect> result)
        {
            float startedDelta = 0f;
            foreach (IAxis axis in axes)
            {
                float measuredValue = _axesMetrics[axis];

                switch (axis.Direction)
                {
                    case Edge.Left:
                        result[axis] = new Rect(
                            left: dataRect.Left - startedDelta - measuredValue,
                            right: dataRect.Left - startedDelta,
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Right:
                        result[axis] = new Rect(
                            left: dataRect.Right + startedDelta,
                            right: dataRect.Right + startedDelta + measuredValue,
                            top: dataRect.Top,
                            bottom: dataRect.Bottom);
                        break;
                    case Edge.Top:
                        result[axis] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Top - startedDelta - measuredValue,
                            bottom: dataRect.Top - startedDelta);
                        break;
                    case Edge.Bottom:
                        result[axis] = new Rect(
                            left: dataRect.Left,
                            right: dataRect.Right,
                            top: dataRect.Bottom + startedDelta,
                            bottom: dataRect.Bottom + startedDelta + measuredValue);
                        break;
                }

                startedDelta += measuredValue;
            }
        }

        protected override void CalculatePanelLayout(
           IEnumerable<IPanel> panels,
           Rect dataRect,
           float startedMargin,
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

                startedDelta += _panelsMetrics[panel] + _options.ColorBarWidth;
            }
        }
    }
}
