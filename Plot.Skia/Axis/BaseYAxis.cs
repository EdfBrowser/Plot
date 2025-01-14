using System.Linq;

namespace Plot.Skia
{
    public abstract class BaseYAxis : BaseAxis, IYAxis
    {
        public double Height => Range.Span;

        public override PixelPanel GetPanel(
            PixelPanel panelSize, float delta, float size)
          => GetVerticalPanel(panelSize, delta, size);

        public override float GetPixel(double position, PixelPanel dataPanel)
        {
            double pxPerUnit = dataPanel.Height / Height;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataPanel.Bottom - px;
        }

        public override double GetWorld(float pixel, PixelPanel dataPanel)
        {
            double unitPerpx = Height / dataPanel.Height;
            float pxFromLeft = pixel - dataPanel.Bottom;
            double unitsFromLeft = pxFromLeft / unitPerpx;
            return Min - unitsFromLeft;
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
                      .Select(x => TickLabelStyle.Measure(x.Label).Width)
                      .Max();

            float axisLabelLength = 0;
            if (!string.IsNullOrEmpty(Label.Text))
                axisLabelLength = Label.Measure(Label.Text).Width;

            return tickHeight + maxTickLabelLength + axisLabelLength + 10;
        }
    }
}
