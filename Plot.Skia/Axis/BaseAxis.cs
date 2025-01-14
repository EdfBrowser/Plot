using SkiaSharp;

namespace Plot.Skia
{
    public abstract class BaseAxis : IAxis
    {
        protected BaseAxis()
        {
            AxisSpacing = 10f;
            Range = PixelRangeMutable.NotSet;

            Label = new LabelStyle();
            TickLabelStyle = new LabelStyle();
            TickLineStyle = new LineStyle();
            MajorTickStyle = new TickStyle() { Length = 4f };
            MinorTickStyle = new TickStyle() { Length = 2f };
        }

        public abstract Edge Direction { get; }
        public abstract ITickGenerator TickGenerator { get; }
        public abstract float GetPixel(double position, PixelPanel dataPanel);
        public abstract double GetWorld(float pixel, PixelPanel dataPanel);
        public abstract void Render(RenderContext rc, float delta, float size);
        public abstract float Measure();
        public abstract PixelPanel GetPanel(
            PixelPanel panelSize, float delta, float size);

        public float AxisSpacing { get; set; }
        public PixelRangeMutable Range { get; }

        public double Min
        {
            get => Range.Min;
            private set => Range.Min = value;
        }
        public double Max
        {
            get => Range.Max;
            private set => Range.Max = value;
        }

        public LabelStyle Label { get; }
        public TickStyle MajorTickStyle { get; }
        public TickStyle MinorTickStyle { get; }
        public LabelStyle TickLabelStyle { get; }
        public LineStyle TickLineStyle { get; }



        public void GenerateTicks(float axisLength) => TickGenerator.Generate(
            Range.ToPixelRange, Direction, axisLength, TickLabelStyle);



        protected PixelPanel GetHorizontalPanel(PixelPanel panelSize, float delta, float size)
            => new PixelPanel(
                panelSize.Left + delta,
                panelSize.Left + delta + size,
                panelSize.Top,
                panelSize.Bottom);

        protected PixelPanel GetVerticalPanel(PixelPanel panelSize, float delta, float size)
            => new PixelPanel(
                panelSize.Left,
                panelSize.Right,
                panelSize.Top + delta,
                panelSize.Top + delta + size);

        protected void DrawTicks(SKCanvas canvas, PixelPanel panel)
        {
            if (Direction.Horizontal())
                DrawTicksForHorizontal(canvas, panel);
            else
                DrawTicksForVertical(canvas, panel);
        }

        protected void DrawLines(SKCanvas canvas, PixelPanel panel)
        {
            if (Direction.Vertical())
            {
                PointF p1 = Direction == Edge.Left ? panel.TopLeft : panel.TopRight;
                PointF p2 = Direction == Edge.Left ? panel.BottomLeft : panel.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
            else
            {
                PointF p1 = Direction == Edge.Top ? panel.TopLeft : panel.BottomLeft;
                PointF p2 = Direction == Edge.Top ? panel.TopRight : panel.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
        }

        private void DrawTicksForVertical(SKCanvas canvas, PixelPanel panel)
        {
            foreach (var tick in TickGenerator.Ticks)
            {
                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float y1 = GetPixel(tick.Position, panel);
                float x1 = Direction == Edge.Left ? panel.Left : panel.Right;
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
                labelLength = Direction == Edge.Left ? -labelLength : labelLength;
                PointF p = new PointF(x1 + tickLength + labelLength, y1)
                    .Translate(0, TickLabelStyle.Ascent() / 2);
                TickLabelStyle.Render(canvas, p, SKTextAlign.Left);
            }
        }

        private void DrawTicksForHorizontal(SKCanvas canvas, PixelPanel panel)
        {
            IXAxis axis = this as IXAxis;
            foreach (var tick in TickGenerator.Ticks)
            {
                if (axis.Animate && tick.Position > axis.ScrollPosition)
                    break;

                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float x1 = GetPixel(tick.Position, panel);
                float y1 = Direction == Edge.Top ? panel.Top : panel.Bottom;
                tickLength = Direction == Edge.Top ? -tickLength : tickLength;

                TickStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
                PointF p1 = new PointF(x1, y1);
                PointF p2 = new PointF(x1, y1 + tickLength);
                tickStyle.Render(canvas, p1, p2);

                // draw label
                if (string.IsNullOrEmpty(tick.Label))
                    continue;

                TickLabelStyle.Text = tick.Label;
                PointF p = new PointF(x1, y1 + tickLength)
                    .Translate(0, TickLabelStyle.Ascent());

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
