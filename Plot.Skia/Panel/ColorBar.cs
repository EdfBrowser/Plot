using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class ColorBar : BasePanel
    {
        private readonly IHasColorBar m_source;
        private readonly IAxis m_axis;
        private readonly Rect _guessRect = new Rect(0, 600f, 0, 400f);

        public ColorBar(IHasColorBar source, Edge direction)
            : base(direction)
        {
            m_source = source;
            m_axis = CreateAxis(direction);

            HeatmapStyle = new HeatmapStyle();
        }

        public HeatmapStyle HeatmapStyle { get; set; }
        internal IAxis Axis { get { return m_axis; } }

        public override float Measure(bool force = false)
        {
            GenerateTicks(_guessRect);
            float offset = m_axis.Measure(force);

            return offset;
        }

        public override void Render(RenderContext rc)
        {
            // 重新计算刻度
            Rect dataRect = rc.GetDataRect(this);

            GenerateTicks(dataRect);

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

            Rect axisDataRect = GetAxisDataRect(dataRect);
            m_axis.Render(rc, axisDataRect);
        }

        private Rect GetAxisDataRect(Rect dataRect)
        {
            switch (Axis.Direction)
            {
                case Edge.Left:
                    return new Rect(dataRect.Left - dataRect.Width, dataRect.Left, dataRect.Top, dataRect.Bottom);
                case Edge.Right:
                    return new Rect(dataRect.Right, dataRect.Right + dataRect.Width, dataRect.Top, dataRect.Bottom);
                case Edge.Top:
                    return new Rect(dataRect.Left, dataRect.Right, dataRect.Top - dataRect.Height, dataRect.Top);
                case Edge.Bottom:
                    return new Rect(dataRect.Left, dataRect.Right, dataRect.Bottom, dataRect.Bottom + dataRect.Height);
                default:
                    throw new NotImplementedException(nameof(Axis.Direction));
            }
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
