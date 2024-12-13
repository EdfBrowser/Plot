namespace Plot.Core.Renderables.Axes
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

        public double Min { get; private set; } = double.MaxValue;
        public double Max { get; private set; } = double.MinValue;

        public double Span => Max - Min;
        public double Center => (Max + Min) / 2;

        public double UnitsPerPx => Span / PlotSizePx;
        public double PxsPerUnit => PlotSizePx / Span;

        // Remembered limits
        // For smooth Pan and zoom
        // For example, if you move 100px to the left and 200px to the right,
        // you will actually move 100px to the right (the second rendering will not cause a large jump effect).
        public double MinRemembered { get; private set; }
        public double MaxRemembered { get; private set; }



        public void Resize(float figureSizePx, float plotSizePx, float dataSizePx, float dataOffsetPx, float plotOffsetPx)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = plotSizePx;
            DataSizePx = dataSizePx;
            DataOffsetPx = dataOffsetPx;
            PlotOffsetPx = plotOffsetPx;
        }

        public (double, double) GetLimits()
        {
            double min = Min == double.MaxValue ? -5 : Min;
            double max = Max == double.MinValue ? 5 : Max;

            return (min == max) ? (min - 1, max + 1) : (min, max);
        }

        public void SetLimits(double min, double max)
        {
            Min = min;
            Max = max;
        }

        public double GetUnit(float px)
        {
            return IsInverted
               ? Min + (PlotOffsetPx + PlotSizePx - px) * UnitsPerPx
               : Min + (px - PlotOffsetPx) * UnitsPerPx;
        }

        public float GetPixel(double unit)
        {
            return IsInverted
               ? (float)(PlotOffsetPx + (Max - unit) * PxsPerUnit)
               : (float)(PlotOffsetPx + (unit - Min) * PxsPerUnit);
        }

        public void PanPx(double px)
        {
            if (IsInverted)
                px = -px;

            Pan(px * UnitsPerPx);
        }

        public void Pan(double units)
        {
            Min += units;
            Max += units;
        }


        public void Zoom(double frac = 1, double? zoomTo = null)
        {
            zoomTo = zoomTo ?? Center;
            double spanLeft = zoomTo.Value - Min;
            double spanRight = Max - zoomTo.Value;
            Min = zoomTo.Value - spanLeft / frac;
            Max = zoomTo.Value + spanRight / frac;
        }

        // Remembered limits
        // For smooth Pan and zoom
        // For example, if you move 100px to the left and 200px to the right,
        // you will actually move 100px to the right (the second rendering will not cause a large jump effect).
        public void SuspendLimits() => (MinRemembered, MaxRemembered) = GetLimits();
        public void ResumeLimits() => (Min, Max) = (MinRemembered, MaxRemembered);
    }

}
