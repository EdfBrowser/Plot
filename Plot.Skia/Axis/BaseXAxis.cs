using System.Linq;

namespace Plot.Skia
{
    public abstract class BaseXAxis : BaseAxis, IXAxis
    {
        protected BaseXAxis()
        {
            ScrollMode = AxisScrollMode.Scrolling;
            ScrollPosition = 0f;
            Animate = false;
        }

        public AxisScrollMode ScrollMode { get; set; }
        public double ScrollPosition { get; set; }
        public bool Animate { get; set; }

        public abstract TickLabelFormat LabelFormat { get; }


        public double Width => RangeMutable.Span;


        public override Rect GetDataRect(
            Rect dataRect, float delta, float size)
            => GetHorizontalRect(dataRect, delta, size);

        public override float GetPixel(double position, Rect dataRect)
        {
            double pxPerUnit = dataRect.Width / Width;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataRect.Left + px;
        }

        public override double GetWorld(float pixel, Rect dataRect)
        {
            double unitPerpx = Width / dataRect.Width;
            float pxFromLeft = pixel - dataRect.Left;
            double unitsFromLeft = pxFromLeft / unitPerpx;
            return Min + unitsFromLeft;
        }

        public override void Render(RenderContext rc, float delta, float size)
        {
            Rect dataRect = GetDataRect(rc.DataRect, delta, size);
            DrawTicks(rc.Canvas, dataRect);
            DrawLines(rc.Canvas, dataRect);
        }

        public override float Measure()
        {
            float tickHeight = MajorTickStyle.Length;

            float maxTickLabelLength = 0;
            if (TickGenerator.Ticks.Length > 0)
                maxTickLabelLength = TickGenerator.Ticks
                    .Select(x => TickLabelStyle.Measure(x.Label).Height)
                    .Max();

            float axisLabelLength = 0;
            if (!string.IsNullOrEmpty(Label.Text))
                axisLabelLength = Label.Measure(Label.Text).Height;

            return tickHeight + maxTickLabelLength + axisLabelLength;
        }
    }
}
