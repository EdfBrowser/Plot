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
            PlotDimensions Dims = XAxis.CreatePlotDimensions(YAxis, scale);
            float px = Dims.GetPixelX(X);
            float yMin = Dims.m_dataOffsetY;//Dims.GetPixelY(Dims.m_yMin);
            float yMax = Dims.m_dataOffsetY + Dims.m_dataHeight;//Dims.GetPixelY(Dims.m_yMax);

            using (var pen = GDI.Pen(Color, LineWidth))
            using (var gfx = GDI.Graphics(bmp, Dims, lowQuality, true))
                gfx.DrawLine(pen, px, yMin, px, yMax);
        }

        public void ValidateData()
        {

        }
    }
}
