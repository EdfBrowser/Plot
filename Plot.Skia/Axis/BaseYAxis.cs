using System.Linq;

namespace Plot.Skia
{
    public abstract class BaseYAxis : BaseAxis, IYAxis
    {
        public double Height => RangeMutable.Span;

        public override Rect GetDataRect(
            Rect dataRect, float delta, float size)
          => GetVerticalRect(dataRect, delta, size);

        public override float GetPixel(double position, Rect dataRect)
        {
            double pxPerUnit = dataRect.Height / Height;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataRect.Bottom - px;
        }

        public override double GetWorld(float pixel, Rect dataRect)
        {
            double unitPerpx = Height / dataRect.Height;
            float pxFromLeft = pixel - dataRect.Bottom;
            double unitsFromLeft = pxFromLeft * unitPerpx;
            return Min - unitsFromLeft;
        }

        public override void Render(RenderContext rc)
        {
            Rect dataRect = rc.GetDataRect(this);
            Render(rc.Canvas, dataRect);
        }

        public override void Render(RenderContext rc, Rect dataRect)
            => Render(rc.Canvas, dataRect);

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
