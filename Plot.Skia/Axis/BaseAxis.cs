using SkiaSharp;

namespace Plot.Skia
{
    public abstract class BaseAxis : IAxis
    {
        protected BaseAxis()
        {
            AxisSpacing = 10f;
            RangeMutable = RangeMutable.NotSet;

            Label = new LabelStyle();
            TickLabelStyle = new LabelStyle();
            TickLineStyle = new LineStyle();
            MajorTickStyle = new TickStyle() { Length = 4f };
            MinorTickStyle = new TickStyle() { Length = 2f };
        }

        public abstract Edge Direction { get; }
        public abstract ITickGenerator TickGenerator { get; }
        public abstract float GetPixel(double position, Rect dataRect);
        public abstract double GetWorld(float pixel, Rect dataRect);
        public abstract void Render(RenderContext rc);
        public abstract void Render(RenderContext rc, Rect dataRect);
        public abstract float Measure();
        public abstract Rect GetDataRect(
            Rect dataRect, float delta, float size);

        public float AxisSpacing { get; set; }
        public RangeMutable RangeMutable { get; }

        public double Min
        {
            get => RangeMutable.Low;
            private set => RangeMutable.Low = value;
        }
        public double Max
        {
            get => RangeMutable.High;
            private set => RangeMutable.High = value;
        }

        public LabelStyle Label { get; }
        public TickStyle MajorTickStyle { get; }
        public TickStyle MinorTickStyle { get; }
        public LabelStyle TickLabelStyle { get; }
        public LineStyle TickLineStyle { get; }



        public void GenerateTicks(float axisLength) => TickGenerator.Generate(
            RangeMutable.ToRange, Direction, axisLength, TickLabelStyle);

        protected void Render(SKCanvas canvas, Rect dataRect)
        {
            DrawTicks(canvas, dataRect);
            DrawLines(canvas, dataRect);
        }

        // TODO: 修改Layout时存的值（不是正常的）
        // horizontal应该修改y轴，x/y-stacked情况比较特殊，后面需要设计一个
        // 结构体来维护delta和size(上下左右都要保存)
        protected Rect GetHorizontalRect(Rect dataRect, float delta, float size)
            => new Rect(
                dataRect.Left + delta,
                dataRect.Left + delta + size,
                dataRect.Top,
                dataRect.Bottom);

        protected Rect GetVerticalRect(Rect dataRect, float delta, float size)
            => new Rect(
                dataRect.Left,
                dataRect.Right,
                dataRect.Top + delta,
                dataRect.Top + delta + size);

        protected void DrawTicks(SKCanvas canvas, Rect dataRect)
        {
            if (Direction.Horizontal())
                DrawTicksForHorizontal(canvas, dataRect);
            else
                DrawTicksForVertical(canvas, dataRect);
        }

        protected void DrawLines(SKCanvas canvas, Rect dataRect)
        {
            if (Direction.Vertical())
            {
                PointF p1 = Direction == Edge.Left ? dataRect.TopLeft : dataRect.TopRight;
                PointF p2 = Direction == Edge.Left ? dataRect.BottomLeft : dataRect.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
            else
            {
                PointF p1 = Direction == Edge.Top ? dataRect.TopLeft : dataRect.BottomLeft;
                PointF p2 = Direction == Edge.Top ? dataRect.TopRight : dataRect.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
        }

        private void DrawTicksForVertical(SKCanvas canvas, Rect dataRect)
        {
            foreach (var tick in TickGenerator.Ticks)
            {
                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float y1 = GetPixel(tick.Position, dataRect);
                float x1 = Direction == Edge.Left ? dataRect.Left : dataRect.Right;
                tickLength = Direction == Edge.Left ? -tickLength : tickLength;

                TickStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
                PointF p1 = new PointF(x1, y1);
                PointF p2 = new PointF(x1 + tickLength, y1);
                tickStyle.Render(canvas, p1, p2);

                // draw label
                if (string.IsNullOrEmpty(tick.Label))
                    continue;

                TickLabelStyle.Text = tick.Label;
                float labelLength = TickLabelStyle.Measure(tick.Label).Width;
                labelLength = Direction == Edge.Left ? -labelLength : 0;
                PointF p = new PointF(
                    x1 + tickLength + labelLength,
                    y1 + TickLabelStyle.Ascent() / 2);
                TickLabelStyle.Render(canvas, p, SKTextAlign.Left);
            }
        }

        private void DrawTicksForHorizontal(SKCanvas canvas, Rect dataRect)
        {
            IXAxis axis = this as IXAxis;
            foreach (var tick in TickGenerator.Ticks)
            {
                if (axis.Animate && tick.Position > axis.ScrollPosition)
                    continue;

                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float x1 = GetPixel(tick.Position, dataRect);
                float y1 = Direction == Edge.Top ? dataRect.Top : dataRect.Bottom;
                tickLength = Direction == Edge.Top ? -tickLength : tickLength;

                TickStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
                PointF p1 = new PointF(x1, y1);
                PointF p2 = new PointF(x1, y1 + tickLength);
                tickStyle.Render(canvas, p1, p2);

                // draw label
                if (string.IsNullOrEmpty(tick.Label))
                    continue;

                TickLabelStyle.Text = tick.Label;
                float ascent = Direction == Edge.Top
                    ? -TickLabelStyle.Descent()
                    : TickLabelStyle.Ascent();
                PointF p = new PointF(
                    x1,
                    y1 + tickLength + ascent);

                TickLabelStyle.Render(canvas, p, SKTextAlign.Left);
            }
        }

        public void Dispose()
        {
            TickLabelStyle.Dispose();
            Label.Dispose();
            MajorTickStyle.Dispose();
            MinorTickStyle.Dispose();
            TickLineStyle.Dispose();
        }

    }
}
