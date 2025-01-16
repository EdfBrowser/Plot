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

        public abstract TickLabelFormat LabelFormat { get; }


        public double Width => RangeMutable.Span;

        // TODO: 用户可以自定义（事件或者策略模式）
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
