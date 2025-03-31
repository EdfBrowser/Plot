using System.Linq;

namespace Plot.Skia
{
    public abstract class BaseYAxis : BaseAxis, IYAxis
    {
        public double Height => RangeMutable.Span;

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

        public override float Measure(bool force = false)
        {
            if (!force)
                return MeasuredValue;

            float tickHeight = 0;
            float maxTickLabelLength = 0;
            float axisLabelLength = 0;

            if (MajorTickStyle.Renderable)
            {
                tickHeight = MajorTickStyle.Length;

                if (TickGenerator.Ticks.Count() > 0)
                {
                    maxTickLabelLength = TickGenerator.Ticks
                         .Select(x => TickLabelStyle.Measure(x.Label).Width)
                         .Max();
                }
            }

            if (Label.Renderable)
            {
                if (!string.IsNullOrEmpty(Label.Text))
                    axisLabelLength = Label.Measure(Label.Text).Width;
            }

            MeasuredValue = tickHeight + maxTickLabelLength + axisLabelLength;
            return MeasuredValue;
        }
    }
}
