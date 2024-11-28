using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    public abstract class Axis : IRenderable, IAxisComponent
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

        private Edge m_edge;
        private bool m_visible;

        public Axis(Edge edge)
        {
            m_visible = true;

            Edge = edge;
        }

        protected void Grid(bool enable) => AxisTick.GridVisible = enable;
        protected void Tick(bool enable) => AxisTick.Visible = enable;
        protected void Label(bool enable) => AxisLabel.Visible = enable;
        protected void Line(bool enable) => AxisLine.Visible = enable;

        public AxisDimensions Dims { get; } = new AxisDimensions();
        public AxisTick AxisTick { get; } = new AxisTick();
        public AxisLine AxisLine { get; } = new AxisLine();
        public AxisLabel AxisLabel { get; } = new AxisLabel();

        public bool IsHorizontal => Edge.IsHorizontal();
        public bool IsVertical => Edge.IsVertical();

        public bool Visible
        {
            get => m_visible;
            set
            {
                m_visible = value;
                AxisTick.Visible = value;
                AxisLine.Visible = value;
                AxisLabel.Visible = value;
            }
        }

        public Edge Edge
        {
            get => m_edge;
            set
            {
                m_edge = value;
                Dims.IsInverted = value.IsVertical();
                AxisTick.Edge = m_edge;
                AxisLabel.Edge = m_edge;
                AxisLine.Edge = m_edge;
            }
        }

        public float PaddingSizePx { get; private set; }
        public float MinimalPadding { get; set; } = 10f;
        public float RotatedSize { get; private set; }

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;

            AxisTick.RotatedSize = RotatedSize;
            AxisLabel.PaddingSizePx = PaddingSizePx;
            AxisTick.Render(bmp, dims, lowQuality);
            AxisLabel.Render(bmp, dims, lowQuality);
            AxisLine.Render(bmp, dims, lowQuality);
        }

        public void RecalculateTickPositions(PlotDimensions dimsFull) => AxisTick.TickGenerator.ReCalculate(dimsFull, AxisTick.TickFont);

        public void ReCalculateAxisSize()
        {
            PaddingSizePx = 0f;

            if (AxisLabel.Visible)
                PaddingSizePx += AxisLabel.Measure().Height;

            if (AxisTick.Visible && AxisTick.TickLabelVisible)
            {
                // determine how many pixels the largest tick label occupies
                float maxHeight = AxisTick.TickGenerator.LargestLabelHeight;
                float maxWidth = AxisTick.TickGenerator.LargestLabelWidth * 1.2f;

                // calculate the width and height of the rotated label
                float largerEdgeLength = Math.Max(maxWidth, maxHeight);
                float shorterEdgeLength = Math.Min(maxWidth, maxHeight);
                float differenceInEdgeLengths = largerEdgeLength - shorterEdgeLength;
                double radians = AxisTick.TickLabelRotation * Math.PI / 180;
                double fraction = IsHorizontal ? Math.Sin(radians) : Math.Cos(radians);
                double rotatedSize = shorterEdgeLength + differenceInEdgeLengths * fraction;

                // add the rotated label size to the size of this axis
                PaddingSizePx += (float)rotatedSize;
                RotatedSize = (float)rotatedSize;
            }

            // 刻度线
            if (AxisTick.Visible && AxisTick.MajorTickVisible)
                PaddingSizePx += AxisTick.MajorTickLength;
        }

        public float GetSize() => Visible ? PaddingSizePx + MinimalPadding : 0;
    }
}
