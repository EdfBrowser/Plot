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
                (xAxis.Dims.GetLimits(), yAxis.Dims.GetLimits()),
                scale,
                xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);

        }
    }
}
