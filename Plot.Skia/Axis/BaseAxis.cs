using SkiaSharp;
using System.Collections.Generic;

namespace Plot.Skia
{
    internal abstract class BaseAxis : IAxis
    {
        protected BaseAxis()
        {
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
        public abstract void Render(RenderContext rc);

        public PixelRangeMutable Range { get; }

        public double Min
        {
            get => Range.Min;
            set => Range.Min = value;
        }
        public double Max
        {
            get => Range.Max;
            set => Range.Max = value;
        }

        public LabelStyle Label { get; set; }
        public TickStyle MajorTickStyle { get; set; }
        public TickStyle MinorTickStyle { get; set; }
        public LabelStyle TickLabelStyle { get; set; }
        public LineStyle TickLineStyle { get; set; }

        public float Measure()
        {
            float tickHeight = MajorTickStyle.Length;

            float maxTickLabelLength = TickGenerator.Ticks.Length > 0
                ? TickGenerator.LargestLabelLength : 0;

            float axisLabelLength = 0;
            if (!string.IsNullOrEmpty(Label.Text))
            {
                (float w, float h) = Label.Measure(Label.Text);
                axisLabelLength = Direction.Horizontal() ? h : w;
            }

            return tickHeight + maxTickLabelLength + axisLabelLength;
        }

        public void GenerateTicks(float axisLength) => TickGenerator.Generate(
            Range.ToCoordinateRange, Direction, axisLength, TickLabelStyle);


        protected void DrawTicks(SKCanvas canvas, Dictionary<IAxis, PixelPanel> axisPanel)
        {
            if (Direction.Horizontal())
                DrawTicksForHorizontal(canvas, axisPanel);
            else
                DrawTicksForVertical(canvas, axisPanel);
        }


        protected void DrawLines(SKCanvas canvas, Dictionary<IAxis, PixelPanel> axisPanel)
        {
            PixelPanel dataPanel = axisPanel[this];

            if (Direction.Vertical())
            {
                PointF p1 = Direction == Edge.Left ? dataPanel.TopLeft : dataPanel.TopRight;
                PointF p2 = Direction == Edge.Left ? dataPanel.BottomLeft : dataPanel.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
            else
            {
                PointF p1 = Direction == Edge.Top ? dataPanel.TopLeft : dataPanel.BottomLeft;
                PointF p2 = Direction == Edge.Top ? dataPanel.TopRight : dataPanel.BottomRight;
                TickLineStyle.Render(canvas, p1, p2);
            }
        }

        private void DrawTicksForVertical(SKCanvas canvas, Dictionary<IAxis, PixelPanel> axisPanel)
        {
            PixelPanel dataPanel = axisPanel[this];
            foreach (var tick in TickGenerator.Ticks)
            {
                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float y1 = GetPixel(tick.Position, dataPanel);
                float x1 = Direction == Edge.Left ? dataPanel.Left : dataPanel.Right;
                float offset = Direction == Edge.Left ? -tickLength : tickLength;

                TickStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
                tickStyle.Render(canvas, new PointF(x1, y1), new PointF(x1 + offset, y1));

                // draw label
                if (string.IsNullOrEmpty(tick.Label))
                    continue;

                TickLabelStyle.Text = tick.Label;
                float labelLength = TickLabelStyle.Measure(tick.Label).width;
                labelLength = Direction == Edge.Left ? -labelLength : labelLength;
                TickLabelStyle.Render(canvas, new PointF(x1 + offset + labelLength, y1));
            }
        }

        private void DrawTicksForHorizontal(SKCanvas canvas, Dictionary<IAxis, PixelPanel> axisPanel)
        {
            PixelPanel dataPanel = axisPanel[this];
            foreach (var tick in TickGenerator.Ticks)
            {
                float tickLength = tick.MajorPos
                    ? MajorTickStyle.Length : MinorTickStyle.Length;
                float x1 = GetPixel(tick.Position, dataPanel);
                float y1 = Direction == Edge.Top ? dataPanel.Top : dataPanel.Bottom;
                float offset = Direction == Edge.Top ? -tickLength : tickLength;

                TickStyle tickStyle = tick.MajorPos ? MajorTickStyle : MinorTickStyle;
                tickStyle.Render(canvas, new PointF(x1, y1), new PointF(x1, y1 + offset));

                // draw label
                if (string.IsNullOrEmpty(tick.Label))
                    continue;

                TickLabelStyle.Text = tick.Label;
                float labelLength = TickLabelStyle.Measure(tick.Label).height;
                labelLength = Direction == Edge.Top ? -labelLength : labelLength;
                TickLabelStyle.Render(canvas, new PointF(x1, y1 + offset + labelLength));
            }
        }
    }
}
