using System.Linq;

namespace Plot.Skia
{
    public abstract class BaseXAxis : BaseAxis, IXAxis
    {
        private double m_scrollPosition;
        protected BaseXAxis()
        {
            m_scrollPosition = 0;
            ScrollMode = AxisScrollMode.Scrolling;
            ScrollPosition = 0f;
            Animate = false;
        }

        public AxisScrollMode ScrollMode { get; set; }
        public double ScrollPosition
        {
            get => m_scrollPosition;
            set
            {
                m_scrollPosition = value;
                if (value > Max)
                    TriggerScrollMode();
            }
        }

        public bool Animate { get; set; }

        public double Width => RangeMutable.Span;

        private void TriggerScrollMode()
        {
            double min = 0, max = 0;

            switch (ScrollMode)
            {
                case AxisScrollMode.Stepping:
                    max = ScrollPosition + Width * 0.15;
                    min = max - Width;
                    break;
                case AxisScrollMode.Scrolling:
                    max = ScrollPosition;
                    min = max - Width;
                    break;
                case AxisScrollMode.Sweeping:
                    max = ScrollPosition + Width;
                    min = ScrollPosition;
                    Animate = true;
                    break;
            }

            RangeMutable.Set(min, max);
        }


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
            double unitsFromLeft = pxFromLeft * unitPerpx;
            return Min + unitsFromLeft;
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
                // TODO: 优化逻辑（每次都反问全部逻辑了）或许force已经解决了，还没测试
                if (TickGenerator.Ticks.Count() > 0)
                    maxTickLabelLength = TickGenerator.Ticks
                        .Select(x => TickLabelStyle.Measure(x.Label).Height)
                        .Max();
            }

            if (Label.Renderable)
            {
                if (!string.IsNullOrEmpty(Label.Text))
                    axisLabelLength = Label.Measure(Label.Text).Height;
            }

            MeasuredValue = tickHeight + maxTickLabelLength + axisLabelLength;
            return MeasuredValue;
        }
    }
}
