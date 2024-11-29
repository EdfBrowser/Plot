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

        public float Min { get; private set; } = float.MaxValue;
        public float Max { get; private set; } = float.MinValue;

        public float Span => Max - Min;
        public float Center => (Max + Min) / 2;

        public float UnitsPerPx => Span / PlotSizePx;
        public float PxsPerUnit => PlotSizePx / Span;

        // Remembered limits
        // For smooth Pan and zoom
        // For example, if you move 100px to the left and 200px to the right,
        // you will actually move 100px to the right (the second rendering will not cause a large jump effect).
        public float MinRemembered { get; private set; }
        public float MaxRemembered { get; private set; }

        public (float min, float max) RationalLimits()
        {
            float min = Min == float.MaxValue ? -10 : Min;
            float max = Max == float.MinValue ? 10 : Max;
            return min == max ? (min - .5f, max + .5f) : (min, max);
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
            Min = (float)min;
            Max = (float)max;
        }

        public float GetUnit(float px)
        {
            return IsInverted
               ? Min + (PlotOffsetPx + PlotSizePx - px) * UnitsPerPx
               : Min + (px - PlotOffsetPx) * UnitsPerPx;
        }

        public void PanPx(float px)
        {
            if (IsInverted)
                px = -px;

            Pan(px * UnitsPerPx);
        }

        public void Pan(float units)
        {
            Min += units;
            Max += units;
        }


        public void Zoom(float frac = 1, float? zoomTo = null)
        {
            //Console.WriteLine($"Min/Max: {Min}/{Max}");
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
        public void SuspendLimits() => (MinRemembered, MaxRemembered) = RationalLimits();
        public void ResumeLimits() => (Min, Max) = (MinRemembered, MaxRemembered);
    }

}
