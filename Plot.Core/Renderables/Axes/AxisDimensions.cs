using System;

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

        public bool HasBeenSet { get; private set; }

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

        public bool IsDateTime { get; set; }

        public (double, double) RationalLimits()
        {
            double min, max;
            if (IsDateTime)
            {
                min = Min == double.MaxValue ? DateTime.MinValue.ToOADate() : Min;
                max = Max == double.MinValue ? DateTime.MinValue.AddSeconds(10).ToOADate() : Max;
            }
            else
            {
                min = Min == double.MaxValue ? -10 : Min;
                max = Max == double.MinValue ? 10 : Max;
                if (min == max)
                {
                    min -= .5;
                    max += .5;
                }
            }

            return (min, max);
        }

        public void Resize(float figureSizePx, float plotSizePx, float dataSizePx, float dataOffsetPx, float plotOffsetPx)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = plotSizePx;
            DataSizePx = dataSizePx;
            DataOffsetPx = dataOffsetPx;
            PlotOffsetPx = plotOffsetPx;
        }

        public void SetLimits(double min, double max)
        {
            HasBeenSet = true;
            Min = min;
            Max = max;
        }

        public double GetUnit(double px)
        {
            return IsInverted
               ? Min + (PlotOffsetPx + PlotSizePx - px) * UnitsPerPx
               : Min + (px - PlotOffsetPx) * UnitsPerPx;
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
        public void SuspendLimits() => (MinRemembered, MaxRemembered) = RationalLimits();
        public void ResumeLimits() => (Min, Max) = (MinRemembered, MaxRemembered);
    }

}
