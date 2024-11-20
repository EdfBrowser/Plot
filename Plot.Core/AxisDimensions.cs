namespace Plot.Core
{
    public class AxisDimensions
    {
        public float FigureSizePx { get; private set; }
        public float PlotSizePx { get; private set; }
        public float PlotOffsetPx { get; private set; }
        // The size of the plot area in pixels.
        public float DataSizePx { get; private set; }
        public float DataOffsetPx { get; private set; }

        public bool IsInverted { get; set; }

        public float Min { get; private set; } = float.MaxValue;
        public float Max { get; private set; } = float.MinValue;

        public float Span => Max - Min;
        public float Center => (Max + Min) / 2;

        public float UnitsPerPx => Span / PlotSizePx;
        public float PxsPerUnit => PlotSizePx / Span;

        public (float min, float max) RationalLimits()
        {
            float min = Min == float.MaxValue ? -10 : Min;
            float max = Max == float.MinValue ? 10 : Max;
            return (min == max) ? (min - 1, max + 1) : (min, max);
        }

        public void Resize(float figureSizePx, float dataSizePx, float dataOffsetPx, float? plotSizePx = null)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = dataSizePx;
            PlotOffsetPx = dataOffsetPx;
            DataSizePx = plotSizePx ?? dataSizePx;
        }

        public void Resize(float figureSizePx,float plotSizePx, float dataSizePx,float dataOffsetPx, float plotOffsetPx)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = plotSizePx;
            DataSizePx = dataSizePx;
            DataOffsetPx = dataOffsetPx;
            PlotOffsetPx = plotOffsetPx;
        }

        public void SetLimits(double xMin, double xMax)
        {
            Min = (float)xMin;
            Max = (float)xMax;
        }
    }
}
