using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class ColorBar : BasePanel
    {
        private readonly IHasColorBar m_source;
        private readonly IAxis m_axis;

        public ColorBar(IHasColorBar source, Edge direction)
            : base(direction)
        {
            m_source = source;
            m_axis = CreateAxis(direction);

            HeatmapStyle = new HeatmapStyle();
        }

        public HeatmapStyle HeatmapStyle { get; set; }

        public override float Measure()
        {
            Rect guessRect = new Rect(0, 600f, 0, 400f);
            GenerateTicks(guessRect);
            float offset = m_axis.Measure();

            return Width + offset;
        }

        public override void Render(RenderContext rc)
        {
            // 重新计算刻度
            GenerateTicks(rc.DataRect);

            Rect dataRect = rc.GetDataRect(this);

            bool vertical = Direction.Vertical();
            IEnumerable<uint> argbs = m_source.ColorMap.ToColorArray(vertical);

            int w = vertical ? 1 : 256;
            int h = vertical ? 256 : 1;

            HeatmapStyle.Render(
              rc.Canvas,
              argbs.ToArray(),
              new Size<int>(w, h),
              dataRect
            );

            (float delta, float size) = rc.GetInfo(this);

            float size2 = Direction.Vertical()
                ? size - dataRect.Width
                : size - dataRect.Height;

            float delta2;
            switch (Direction)
            {
                case Edge.Left:
                    delta2 = rc.DataRect.Left - dataRect.Left;
                    break;
                case Edge.Right:
                    delta2 = dataRect.Right - rc.DataRect.Right;
                    break;
                case Edge.Top:
                    delta2 = rc.DataRect.Top - dataRect.Top;
                    break;
                case Edge.Bottom:
                    delta2 = dataRect.Bottom - rc.DataRect.Bottom;
                    break;
                default: throw new NotImplementedException($"{nameof(Direction)} isn`t supported!");
            }

            Rect rect;
            switch (Direction)
            {
                case Edge.Left:
                    rect = new Rect(
                        rc.DataRect.Left - delta2,
                        rc.DataRect.Left - delta2 + size2,
                        rc.DataRect.Top,
                        rc.DataRect.Bottom);
                    break;
                case Edge.Right:
                    rect = new Rect(
                        rc.DataRect.Right + delta2 - size2,
                        rc.DataRect.Right + delta2,
                        rc.DataRect.Top,
                        rc.DataRect.Bottom);
                    break;
                case Edge.Top:
                    rect = new Rect(
                        rc.DataRect.Left,
                        rc.DataRect.Right,
                        rc.DataRect.Top - delta2,
                        rc.DataRect.Top - delta2 + size2);
                    break;
                case Edge.Bottom:
                    rect = new Rect(
                        rc.DataRect.Left,
                        rc.DataRect.Right,
                        rc.DataRect.Bottom + delta2 - size2,
                        rc.DataRect.Bottom + delta2);
                    break;
                default: throw new NotImplementedException($"{nameof(Direction)} isn`t supported!");
            }

            m_axis.Render(rc, rect);
        }

        private static IAxis CreateAxis(Edge direction)
        {
            switch (direction)
            {
                case Edge.Left:
                    return new NumericLeftAxis();
                case Edge.Right:
                    return new NumericRightAxis();
                case Edge.Top:
                    return new NumericTopAxis();
                case Edge.Bottom:
                    return new NumericBottomAxis();
                default:
                    throw new NotImplementedException($"{nameof(direction)} isn`t supported!");
            }
        }

        private void GenerateTicks(Rect dataRect)
        {
            Range range = m_source.GetRange();
            m_axis.RangeMutable.Set(range.Low, range.High);

            float axisLength = Direction.Vertical()
              ? dataRect.Height
              : dataRect.Width;
            m_axis.GenerateTicks(axisLength);
        }


        public override void Dispose()
        {
            HeatmapStyle.Dispose();
        }
    }
}
