namespace Plot.Core
{
    public class AxisDimensions
    {
        public double FigureSizePx { get; private set; }
        public double DataSizePx { get; private set; }
        public double DataOffsetPx { get; private set; }
        public double IsInverted { get; private set; }

        public double Min { get; private set; } = double.NaN;
        public double Max { get; private set; } = double.NaN;

        public double Span => Max - Min;
        public double Center => (Max + Min) / 2.0;

        public double UnitsPerPx => Span / DataSizePx;
        public double PxsPerUnit => DataSizePx / Span;

        public (double min, double max) RationalLimits()
        {
            double min = double.IsNaN(Min) ? -10 : Min;
            double max = double.IsNaN(Max) ? 10 : Max;
            return (min == max) ? (min - .5, max + .5) : (min, max);
        }


        public void Resize(double figureSizePx, double? dataSizePx = null, double? dataOffsetPx = null)
        {
            FigureSizePx = figureSizePx;
            DataSizePx = dataSizePx ?? DataSizePx;
            DataOffsetPx = dataOffsetPx ?? DataOffsetPx;
        }

        public void SetPadding(float padBefore, float padAfter)
        {
            DataOffsetPx = padBefore;
            DataSizePx = FigureSizePx - padBefore - padAfter;
        }
    }
}
