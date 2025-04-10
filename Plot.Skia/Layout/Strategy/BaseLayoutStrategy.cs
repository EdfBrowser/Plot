using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Plot.Skia
{
    internal abstract class BaseLayoutStrategy : ILayoutStrategy
    {
        protected readonly Figure _figure;
        protected readonly LayoutOptions _options;
        protected Dictionary<IAxis, float> _axesMetrics;
        protected Dictionary<IPanel, float> _panelsMetrics;
        protected Rect _axesEdge;
        protected Rect _panelsEdge;
        protected Rect _axesMargin;
        protected Rect _panelsMargin;
        protected Rect _dataRect;

        protected BaseLayoutStrategy(Figure figure, LayoutOptions options)
        {
            _figure = figure;
            _options = options;
        }

        public abstract LayoutResult CalculateLayout(Rect figureRect);

        protected void CalculateCore(Rect figureRect)
        {
            _figure.AxisManager.GenerateTicks(figureRect);

            _axesMetrics = Measured(_figure.AxisManager.Axes);
            _panelsMetrics = Measured(_figure.PanelManager.Panels);

            // 测量边缘的Tick Label
            _axesEdge = CalculateAxesEdge();
            _panelsEdge = CalculatePanelsEdge();

            // 测量总的Margin
            _axesMargin = CalculateEdgeMargin(_figure.AxisManager.Axes, _axesMetrics, _axesEdge);
            _panelsMargin = CalculateEdgeMargin(_figure.PanelManager.Panels, _panelsMetrics, _panelsEdge);
            _dataRect = CalculateDataRect(figureRect, _axesMargin, _panelsMargin);
        }

        #region private methods
        private Dictionary<T, float> Measured<T>(IEnumerable<T> elements) where T : IMeasureable
        {
            return elements.ToDictionary(e=>e, e=> e.Measure(force: true));
        }

        private Rect CalculateAxesEdge()
        {
            IEnumerable<IAxis> axes = _figure.AxisManager.Axes;
            float l = _options.MinAxisSpace,
                r = _options.MinAxisSpace,
                t = _options.MinAxisSpace,
                b = _options.MinAxisSpace;

            foreach (var axis in axes)
            {
                Tick first = axis.TickGenerator.Ticks.First(x => x.MajorPos);
                Tick last = axis.TickGenerator.Ticks.Last(x => x.MajorPos);

                Size<float> firstSize = axis.TickLabelStyle.Measure(first.Label);
                Size<float> lastSize = axis.TickLabelStyle.Measure(last.Label);

                if (axis.Direction.Horizontal())
                {
                    ApplyHorizontalEdgeCalculation(axis, firstSize, lastSize, ref l, ref r);
                }
                else
                {
                    ApplyVerticalEdgeCalculation(axis, firstSize, lastSize, ref t, ref b);
                }
            }

            return new Rect(l, r, t, b);
        }

        private Rect CalculatePanelsEdge()
        {
            IEnumerable<IPanel> panels = _figure.PanelManager.Panels;
            float l = _options.MinPanelSpace,
                r = _options.MinPanelSpace,
                t = _options.MinPanelSpace,
                b = _options.MinPanelSpace;

            foreach (IPanel panel in panels)
            {
                if (!(panel is ColorBarPanel colorPanel)) continue;

                var first = colorPanel.Axis.TickGenerator.Ticks.First(x => x.MajorPos);
                var last = colorPanel.Axis.TickGenerator.Ticks.Last(x => x.MajorPos);

                var firstSize = colorPanel.Axis.TickLabelStyle.Measure(first.Label);
                var lastSize = colorPanel.Axis.TickLabelStyle.Measure(last.Label);

                if (colorPanel.Axis.Direction.Horizontal())
                {
                    ApplyHorizontalEdgeCalculation(colorPanel.Axis, firstSize, lastSize, ref l, ref r);
                }
                else
                {
                    ApplyVerticalEdgeCalculation(colorPanel.Axis, firstSize, lastSize, ref t, ref b);
                }
            }

            return new Rect(l, r, t, b);
        }

        private Rect CalculateEdgeMargin<T>(IEnumerable<T> many,
        IReadOnlyDictionary<T, float> metrics,
        Rect edges) where T : IRenderable
        {
            float left = edges.Left, right = edges.Right, bottom = edges.Bottom, top = edges.Top;

            float value;
            foreach (var item in many)
            {
                value = metrics[item];
                if (item is ColorBarPanel) value += _options.ColorBarWidth;

                switch (item.Direction)
                {
                    case Edge.Left:
                        left = Math.Max(left, value);
                        break;
                    case Edge.Right:
                        right = Math.Max(right, value);
                        break;
                    case Edge.Bottom:
                        bottom = Math.Max(bottom, value);
                        break;
                    case Edge.Top:
                        top = Math.Max(top, value);
                        break;
                }
            }

            return new Rect(left, right, top, bottom);
        }

        private Rect CalculateDataRect(Rect figureRect, Rect axesMargin, Rect panelsMargin)
        {
            float wOffset = axesMargin.Left + panelsMargin.Left + axesMargin.Right + panelsMargin.Right;
            float hOffset = axesMargin.Top + panelsMargin.Top + axesMargin.Bottom + panelsMargin.Bottom;

            float dataRectWidth
                = Math.Max(0, figureRect.Width - wOffset);
            float dataRectHeight
                = Math.Max(0, figureRect.Height - hOffset);

            Size<float> dataSize = new Size<float>(dataRectWidth, dataRectHeight);
            PointF location = new PointF(axesMargin.Left + panelsMargin.Left, axesMargin.Top + panelsMargin.Top);

            return new Rect(location, dataSize)
                .WithPan(figureRect.Left, figureRect.Top);
        }


        private void ApplyHorizontalEdgeCalculation(
           IAxis axis,
           Size<float> firstSize,
           Size<float> lastSize,
           ref float left,
           ref float right)
        {
            switch (axis.TickLabelStyle.TextAlign)
            {
                case SKTextAlign.Right:
                    left = Math.Max(left, firstSize.Width);
                    break;
                case SKTextAlign.Left:
                    right = Math.Max(right, lastSize.Width);
                    break;
                default:
                    left = Math.Max(left, firstSize.Width / 2);
                    right = Math.Max(right, lastSize.Width / 2);
                    break;
            }
        }

        private void ApplyVerticalEdgeCalculation(
            IAxis axis,
            Size<float> firstSize,
            Size<float> lastSize,
            ref float top,
            ref float bottom)
        {
            bottom = Math.Max(bottom, firstSize.Height);
            top = Math.Max(top, lastSize.Height);
        }


        // ========== Utils=======================
        private static float GetInitialOffset(Edge direction, Rect offsets)
        {
            switch (direction)
            {
                case Edge.Left:
                    return offsets.Left;
                case Edge.Right:
                    return offsets.Right;

                case Edge.Top:
                    return offsets.Top;
                case Edge.Bottom:
                    return offsets.Bottom;
                default:
                    throw new InvalidEnumArgumentException(nameof(direction));
            }
        }

        private static int GetCollectionCapacity<T>(IEnumerable<T> source)
        {
            if (source is ICollection<T> collection) return collection.Count;
            return source is IReadOnlyCollection<T> readOnlyColl ? readOnlyColl.Count : 0;
        }

        #endregion

        protected Dictionary<IAxis, Rect> ArrangeAxes(Rect dataRect)
        {
            IEnumerable<IAxis> axes = _figure.AxisManager.Axes;
            var result = new Dictionary<IAxis, Rect>(GetCollectionCapacity(axes));
            foreach (var group in axes.GroupBy(x => x.Direction))
            {
                CalculateAxisLayout(group, dataRect, result);
            }

            return result;
        }

        protected abstract void CalculateAxisLayout(
           IEnumerable<IAxis> axes,
           Rect dataRect,
           Dictionary<IAxis, Rect> result);

        protected Dictionary<IPanel, Rect> ArrangePanels(Rect dataRect)
        {
            IEnumerable<IPanel> panels = _figure.PanelManager.Panels;
            var result = new Dictionary<IPanel, Rect>(GetCollectionCapacity(panels));

            var group = panels.GroupBy(x => x.Direction).Select(g => (g.Key, g.AsEnumerable()));
            foreach (var (direction, many) in group)
            {
                float offset = GetInitialOffset(direction, _axesMargin);
                CalculatePanelLayout(many, dataRect, offset, result);
            }

            return result;
        }

        protected abstract void CalculatePanelLayout(
           IEnumerable<IPanel> panels,
           Rect dataRect,
           float startedMargin,
           Dictionary<IPanel, Rect> result);
    }
}
