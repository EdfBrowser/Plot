using System.Linq;

namespace Plot.Skia
{
    internal abstract class XAxisBase : IXAxis
    {
        protected XAxisBase()
        {
            Range = CoordinateRangeMutable.NotSet;

            MinorTickStyle = new TickStyle()
            {
                AntiAlias = false,
                Color = Color.Black,
                Length = 4f,
                Width = 1f,
            };

            MinorTickStyle = new TickStyle()
            {
                AntiAlias = false,
                Color = Color.Black,
                Length = 2f,
                Width = 1f,
            };

            TickLineStyle = new LineStyle()
            {
                Width = 1f,
                Color = Color.Black,
                AntiAlias = false,
            };

            TickLabelStyle = new LabelStyle()
            {
                AntiAlias = false,
                Color = Color.Black,
                FontSize = 12,
            };
        }

        public CoordinateRangeMutable Range { get; }
        public abstract Edge Direction { get; }
        public abstract ITickGenerator TickGenerator { get; }

        public double Width => Range.Span;

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

        public float GetPixel(double position, PixelPanel dataPanel)
        {
            double pxPerUnit = dataPanel.Width / Width;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataPanel.Left + px;
        }

        public double GetWorld(float pixel, PixelPanel dataPanel)
        {
            double unitPerpx = Width / dataPanel.Width;
            float pxFromLeft = pixel - dataPanel.Left;
            double unitsFromLeft = pxFromLeft / unitPerpx;
            return Min + unitsFromLeft;
        }

        public float Measure()
        {
            float tickHeight = MajorTickStyle.Length;

            float maxTickLabelHeight = TickGenerator.Ticks.Length > 0
                ? TickGenerator.Ticks.Select(x => TickLabelStyle.Measure(x.Label)).Max()
                : 0;

            float axisLabelHeight = string.IsNullOrEmpty(Label.Text)
                ? 0 : Label.Measure(Label.Text);

            return tickHeight + maxTickLabelHeight + axisLabelHeight;
        }

        public void GenerateTicks(float axisLength) => TickGenerator.Generate(
            Range.ToCoordinateRange, Direction, axisLength, TickLabelStyle);
    }
}
