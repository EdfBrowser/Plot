using Plot.Core.Enum;
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
                return min == max ? (min - 1, max + 1) : (min, max);
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

            public float GetUnit(float centerPx)
            {
                return IsInverted
                   ? Min + (PlotOffsetPx + PlotSizePx - centerPx) * UnitsPerPx
                   : Min + (centerPx - PlotOffsetPx) * UnitsPerPx;
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
            public void Remember() => (MinRemembered, MaxRemembered) = RationalLimits();
            public void Recall() => (Min, Max) = (MinRemembered, MaxRemembered);
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

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            AxisTick.Render(bmp, dims, lowQuality);
            AxisLabel.Render(bmp, dims, lowQuality);
            AxisLine.Render(bmp, dims, lowQuality);
        }
    }
}
