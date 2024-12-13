using Plot.Core.Draws;
using Plot.Core.Renderables.Axes;
using System.Drawing;


namespace Plot.Core.Series
{
    public class VLinePlotSeries : IPlotSeries
    {
        public VLinePlotSeries(Axis xAxis, Axis yAxis)
        {
            XAxis = xAxis;
            YAxis = yAxis;
        }

        public Axis XAxis { get; }

        public Axis YAxis { get; }

        public Color Color { get; set; }
        public float LineWidth { get; set; }

        public double X { get; set; }

        public void Plot(Bitmap bmp, bool lowQuality, float scale)
        {
            float px = XAxis.Dims.GetPixel(X);
            float yMin = YAxis.Dims.DataOffsetPx;
            float yMax = YAxis.Dims.DataOffsetPx + YAxis.Dims.DataSizePx;

            using (var pen = GDI.Pen(Color, LineWidth))
            using (var gfx = GDI.Graphics(bmp, lowQuality, scale))
                gfx.DrawLine(pen, px, yMin, px, yMax);
        }

        public void ValidateData()
        {

        }
    }
}
