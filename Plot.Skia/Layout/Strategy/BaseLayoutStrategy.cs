using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace Plot.Skia
{
    internal abstract class BaseLayoutStrategy : ILayoutStrategy
    {
        // TODO: 改用Rect?查看是否还有重复功能得类结构体
        protected readonly struct EdgeOffsets
        {
            private readonly float _left;
            private readonly float _right;
            private readonly float _top;
            private readonly float _bottom;

            internal EdgeOffsets(float left, float right, float top, float bottom)
            {
                _left = left;
                _right = right;
                _top = top;
                _bottom = bottom;
            }

            internal float Left => _left;
            internal float Right => _right;
            internal float Top => _top;
            internal float Bottom => _bottom;
        }

        protected readonly LayoutOptions _options;

        protected BaseLayoutStrategy(LayoutOptions options)
        {
            _options = options;
        }


        public abstract LayoutResult CalculateLayout(Figure figure, Rect figureRect);

        protected void GenerateTick(IEnumerable<IAxis> axes, Rect figureRect)
        {
            foreach (IAxis axis in axes)
            {
                float axisLength = axis.Direction.Horizontal() ? figureRect.Width : figureRect.Height;
                axis.GenerateTicks(axisLength);
            }
        }

        protected Dictionary<IAxis, float> MeasureAxes(IEnumerable<IAxis> axes)
        {
            var dict = new Dictionary<IAxis, float>(GetCollectionCapacity(axes));
            foreach (var axis in axes)
                dict[axis] = axis.Measure(force: true);
            return dict;
        }

        protected Dictionary<IPanel, float> MeasurePanels(IEnumerable<IPanel> panels)
        {
            var dict = new Dictionary<IPanel, float>(GetCollectionCapacity(panels));
            foreach (var panel in panels)
            {
                float measuredValue = panel.Measure(force: true);

                if (panel is ColorBar)
                {
                    measuredValue += _options.ColorBarWidth;
                }

                dict[panel] = measuredValue;
            }
            return dict;
        }

        protected EdgeOffsets CalculateAxesEdge(IEnumerable<IAxis> axes)
        {
            float l = _options.MinAxisSpace,
                r = _options.MinAxisSpace,
                t = _options.MinAxisSpace,
                b = _options.MinAxisSpace;

            foreach (var axis in axes)
            {
                var first = axis.TickGenerator.Ticks.First(x => x.MajorPos);
                var last = axis.TickGenerator.Ticks.Last(x => x.MajorPos);

                var firstSize = axis.TickLabelStyle.Measure(first.Label);
                var lastSize = axis.TickLabelStyle.Measure(last.Label);

                if (axis.Direction.Horizontal())
                {
                    ApplyHorizontalEdgeCalculation(axis, firstSize, lastSize, ref l, ref r, _options);
                }
                else
                {
                    ApplyVerticalEdgeCalculation(axis, firstSize, lastSize, ref t, ref b, _options);
                }
            }

            return new EdgeOffsets(l, r, t, b);
        }

        protected EdgeOffsets CalculatePanelsEdge(IEnumerable<IPanel> panels)
        {
            float l = _options.MinAxisSpace,
                r = _options.MinAxisSpace,
                t = _options.MinAxisSpace,
                b = _options.MinAxisSpace;

            foreach (IPanel panel in panels)
            {
                if (!(panel is ColorBar colorPanel)) continue;

                var first = colorPanel.Axis.TickGenerator.Ticks.First(x => x.MajorPos);
                var last = colorPanel.Axis.TickGenerator.Ticks.Last(x => x.MajorPos);

                var firstSize = colorPanel.Axis.TickLabelStyle.Measure(first.Label);
                var lastSize = colorPanel.Axis.TickLabelStyle.Measure(last.Label);

                if (colorPanel.Axis.Direction.Horizontal())
                {
                    ApplyHorizontalEdgeCalculation(colorPanel.Axis, firstSize, lastSize, ref l, ref r, _options);
                }
                else
                {
                    ApplyVerticalEdgeCalculation(colorPanel.Axis, firstSize, lastSize, ref t, ref b, _options);
                }
            }

            return new EdgeOffsets(l, r, t, b);
        }


        private void ApplyHorizontalEdgeCalculation(
           IAxis axis,
           SizeF firstSize,
           SizeF lastSize,
           ref float left,
           ref float right,
           LayoutOptions options)
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
            SizeF firstSize,
            SizeF lastSize,
            ref float top,
            ref float bottom,
            LayoutOptions options)
        {
            bottom = Math.Max(bottom, firstSize.Height);
            top = Math.Max(top, lastSize.Height);
        }


        // ========== Utils=======================
        protected static float GetInitialOffset(Edge direction, EdgeOffsets offsets)
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

        protected static int GetCollectionCapacity<T>(IEnumerable<T> source)
        {
            if (source is ICollection<T> collection) return collection.Count;
            return source is IReadOnlyCollection<T> readOnlyColl ? readOnlyColl.Count : 0;
        }
    }
}
