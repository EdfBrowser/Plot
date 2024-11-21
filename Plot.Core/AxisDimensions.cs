using System;

namespace Plot.Core
{
    internal class AxisDimensions
    {
        internal float FigureSizePx { get; private set; }
        internal float PlotSizePx { get; private set; }
        internal float PlotOffsetPx { get; private set; }
        // The size of the plot area in pixels.
        internal float DataSizePx { get; private set; }
        internal float DataOffsetPx { get; private set; }

        internal bool HasBeenSet { get; private set; }

        internal bool IsInverted { get; set; }

        internal float Min { get; private set; } = float.MaxValue;
        internal float Max { get; private set; } = float.MinValue;

        internal float Span => Max - Min;
        internal float Center => (Max + Min) / 2;

        internal float UnitsPerPx => Span / PlotSizePx;
        internal float PxsPerUnit => PlotSizePx / Span;

        // Remembered limits
        // For smooth Pan and zoom
        // For example, if you move 100px to the left and 200px to the right,
        // you will actually move 100px to the right (the second rendering will not cause a large jump effect).
        internal float MinRemembered { get; private set; }
        internal float MaxRemembered { get; private set; }

        internal (float min, float max) RationalLimits()
        {
            float min = Min == float.MaxValue ? -10 : Min;
            float max = Max == float.MinValue ? 10 : Max;
            return (min == max) ? (min - 1, max + 1) : (min, max);
        }

        internal void Resize(float figureSizePx, float dataSizePx, float dataOffsetPx, float? plotSizePx = null)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = dataSizePx;
            PlotOffsetPx = dataOffsetPx;
            DataSizePx = plotSizePx ?? dataSizePx;
        }

        internal void Resize(float figureSizePx, float plotSizePx, float dataSizePx, float dataOffsetPx, float plotOffsetPx)
        {
            FigureSizePx = figureSizePx;
            PlotSizePx = plotSizePx;
            DataSizePx = dataSizePx;
            DataOffsetPx = dataOffsetPx;
            PlotOffsetPx = plotOffsetPx;
        }

        internal void SetLimits(double xMin, double xMax)
        {
            HasBeenSet = true;
            Min = (float)xMin;
            Max = (float)xMax;
        }

        internal float GetUnit(float centerPx)
        {
            return IsInverted
               ? Min + (PlotOffsetPx + PlotSizePx - centerPx) * UnitsPerPx
               : Min + (centerPx - PlotOffsetPx) * UnitsPerPx;
        }

        internal void PanPx(float px)
        {
            if (IsInverted)
                px = -px;

            Pan(px * UnitsPerPx);
        }

        internal void Pan(float units)
        {
            Min += units;
            Max += units;
        }


        internal void Zoom(float frac = 1, float? zoomTo = null)
        {
            zoomTo = zoomTo ?? Center;
            float spanLeft = zoomTo.Value - Min;
            float spanRight = Max - zoomTo.Value;
            Min = zoomTo.Value - spanLeft / frac;
            Max = zoomTo.Value + spanRight / frac;
        }




        // Remembered limits
        // For smooth Pan and zoom
        // For example, if you move 100px to the left and 200px to the right,
        // you will actually move 100px to the right (the second rendering will not cause a large jump effect).
        internal void Remember() => (MinRemembered, MaxRemembered) = (Min, Max);
        internal void Recall() => (Min, Max) = (MinRemembered, MaxRemembered);


    }
}
