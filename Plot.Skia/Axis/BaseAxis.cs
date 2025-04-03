using SkiaSharp;

namespace Plot.Skia
{
    public abstract class BaseAxis : IAxis
    {
        protected BaseAxis()
        {
            RangeMutable = RangeMutable.NotSet;

            Label = new LabelStyle();
            TickLabelStyle = new LabelStyle();
            TickLineStyle = new LineStyle();
            MajorTickStyle = new LineStyle() { Length = 4f };
            MinorTickStyle = new LineStyle() { Length = 2f };
        }

        public abstract Edge Direction { get; }
        public abstract ITickGenerator TickGenerator { get; }
        public abstract float GetPixel(double position, Rect dataRect);
        public abstract double GetWorld(float pixel, Rect dataRect);
        public abstract void Render(RenderContext rc);
        public abstract void Render(RenderContext rc, Rect dataRect);
        public abstract float Measure(bool force = false);

        public float Space { get; set; }
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
        public LineStyle MajorTickStyle { get; }
        public LineStyle MinorTickStyle { get; }
        public LabelStyle TickLabelStyle { get; }
        public LineStyle TickLineStyle { get; }

        protected float MeasuredValue { get; set; }


        public void GenerateTicks(float axisLength) => TickGenerator.Generate(
            RangeMutable.ToRange, Direction, axisLength, TickLabelStyle);

        protected void Render(SKCanvas canvas, Rect dataRect)
        {
            DrawTicks(canvas, dataRect);
            DrawLines(canvas, dataRect);
            DrawLabels(canvas, dataRect);
        }

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
                PointF p1 = Direction == Edge.Left ? dataRect.TopRight : dataRect.TopLeft;
                PointF p2 = Direction == Edge.Left ? dataRect.BottomRight : dataRect.BottomLeft;
                TickLineStyle.Render(canvas, p1, p2);
            }
            else
            {
                PointF p1 = Direction == Edge.Top ? dataRect.BottomLeft : dataRect.TopLeft;
                PointF p2 = Direction == Edge.Top ? dataRect.BottomRight : dataRect.TopRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
        }

        private void DrawLabels(SKCanvas canvas, Rect dataRect)
        {
            // Draw the Title
            if (string.IsNullOrEmpty(Label.Text))
                return;

            float measured = Measure();

            if (Direction.Vertical())
            {
                float x = Direction == Edge.Left
                               ? dataRect.Right : dataRect.Left;

                x = Direction == Edge.Left
                    ? x - measured : x;

                PointF p = new PointF(x, dataRect.VerticalCenter);

                Label.Render(canvas, p);
            }
            else
            {
                float y = Direction == Edge.Top
                               ? dataRect.Bottom : dataRect.Top;

                y = Direction == Edge.Top
                    ? y - measured : y;

                PointF p = new PointF(dataRect.HorizontalCenter, y);

                Label.Render(canvas, p);
            }
        }

        private void DrawTicksForVertical(SKCanvas canvas, Rect dataRect)
        {
            foreach (var tick in TickGenerator.Ticks)
            {
                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float y1 = GetPixel(tick.Position, dataRect);
                float x1 = Direction == Edge.Left ? dataRect.Right : dataRect.Left;
                tickLength = Direction == Edge.Left ? -tickLength : tickLength;

                LineStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
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
                TickLabelStyle.Render(canvas, p);
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
                float y1 = Direction == Edge.Top ? dataRect.Bottom : dataRect.Top;
                tickLength = Direction == Edge.Top ? -tickLength : tickLength;

                LineStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
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

                TickLabelStyle.Render(canvas, p);
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
