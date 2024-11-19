namespace Plot.Core
{
    public class AxisDimensions
    {
        public float FigureSizePx { get; private set; }
        public float DataSizePx { get; private set; }
        // The size of the plot area in pixels.
        public float PlotSizePx { get; set; }
        public float DataOffsetPx { get; private set; }

        public bool IsInverted { get; set; }

        public float Min { get; private set; } = float.MaxValue;
        public float Max { get; private set; } = float.MinValue;

        public float Span => Max - Min;
        public float Center => (Max + Min) / 2;

        public float UnitsPerPx => Span / DataSizePx;
        public float PxsPerUnit => DataSizePx / Span;

        public (float min, float max) RationalLimits()
        {
            float min = Min == float.MaxValue ? -10 : Min;
            float max = Max == float.MinValue ? 10 : Max;
            return (min == max) ? (min - 1, max + 1) : (min, max);
        }

        public void Resize(float figureSizePx, float dataSizePx, float dataOffsetPx, float? plotSizePx = null)
        {
            FigureSizePx = figureSizePx;
            DataSizePx = dataSizePx;
            DataOffsetPx = dataOffsetPx;
            PlotSizePx = plotSizePx ?? dataSizePx;
        }
    }
}
