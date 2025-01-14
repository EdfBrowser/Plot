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

        public abstract TickLabelFormat LabelFormat { get; set; }


        public double Width => Range.Span;


        public override PixelPanel GetPanel(
            PixelPanel panelSize, float delta, float size)
            => GetHorizontalPanel(panelSize, delta, size);

        public override float GetPixel(double position, PixelPanel dataPanel)
        {
            double pxPerUnit = dataPanel.Width / Width;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataPanel.Left + px;
        }

        public override double GetWorld(float pixel, PixelPanel dataPanel)
        {
            double unitPerpx = Width / dataPanel.Width;
            float pxFromLeft = pixel - dataPanel.Left;
            double unitsFromLeft = pxFromLeft / unitPerpx;
            return Min + unitsFromLeft;
        }

        public override void Render(RenderContext rc, float delta, float size)
        {
            PixelPanel panel = GetPanel(rc.Layout.DataPanel, delta, size);
            DrawTicks(rc.Canvas, panel);
            DrawLines(rc.Canvas, panel);
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
