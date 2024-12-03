using Plot.Core.Renderables.Axes;
using System.Drawing;

namespace Plot.Core
{
    public static class CustomExtensions
    {
        public static PlotDimensions CreatePlotDimensions(this Axis xAxis, Axis yAxis, float scale)
        {
            SizeF figureSize = new SizeF(xAxis.Dims.FigureSizePx, yAxis.Dims.FigureSizePx);
            SizeF plotSize = new SizeF(xAxis.Dims.DataSizePx, yAxis.Dims.DataSizePx);
            SizeF dataSize = new SizeF(xAxis.Dims.PlotSizePx, yAxis.Dims.PlotSizePx);
            PointF plotOffset = new PointF(xAxis.Dims.PlotOffsetPx, yAxis.Dims.PlotOffsetPx);
            PointF dataOffset = new PointF(xAxis.Dims.DataOffsetPx, yAxis.Dims.DataOffsetPx);

            return new PlotDimensions(figureSize,
                dataSize,
                plotSize,
                plotOffset,
                dataOffset,
                (xAxis.Dims.RationalLimits(), yAxis.Dims.RationalLimits()),
                scale,
                xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);

        }

        public static void SetAxisLimits(this Axis xAxis, Axis yAxis, AxisLimits axisLimits)
        {
            xAxis.Dims.SetLimits(axisLimits.m_xMin, axisLimits.m_xMax);
            yAxis.Dims.SetLimits(axisLimits.m_yMin, axisLimits.m_yMax);
        }

        public static AxisLimits GetAxisLimits(this Axis xAxis, Axis yAxis)
        {
            (double xMin, double xMax) = xAxis.Dims.RationalLimits();
            (double yMin, double yMax) = yAxis.Dims.RationalLimits();
            return new AxisLimits(xMin, xMax, yMin, yMax);
        }
    }
}
