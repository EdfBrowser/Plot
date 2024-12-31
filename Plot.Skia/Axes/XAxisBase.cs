using Plot.Skia.Enums;
using Plot.Skia.Structs;

namespace Plot.Skia.Axes
{
    internal abstract class XAxisBase : IXAxis
    {
        protected XAxisBase()
        {
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
        }

        public double Width => Max - Min;

        public double Min { get; set; }
        public double Max { get; set; }

        public abstract Edge Direction { get; set; }

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
            double unitsFromLeft = pxFromLeft * unitPerpx;
            return Min + unitsFromLeft;
        }
    }
}
