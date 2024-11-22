using Plot.Core.Enum;
using System;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.Emit;

namespace Plot.Core.Renderables.Axes
{
    public abstract class Axis : IRenderable, IAxisComponent
    {
        private class AxisDimensions
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
                return min == max ? (min - 1, max + 1) : (min, max);
            }

            internal void Resize(float figureSizePx, float plotSizePx, float dataSizePx, float dataOffsetPx, float plotOffsetPx)
            {
                FigureSizePx = figureSizePx;
                PlotSizePx = plotSizePx;
                DataSizePx = dataSizePx;
                DataOffsetPx = dataOffsetPx;
                PlotOffsetPx = plotOffsetPx;
            }

            internal void SetLimits(double min, double max)
            {
                HasBeenSet = true;
                Min = (float)min;
                Max = (float)max;
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
            internal void Remember() => (MinRemembered, MaxRemembered) = RationalLimits();
            internal void Recall() => (Min, Max) = (MinRemembered, MaxRemembered);
        }

        private Edge m_edge;
        private float m_pixelOffset;
        private bool m_rulerMode;
        private bool m_visible;
        private string m_title;

        private readonly AxisTick m_axisTick;
        private readonly AxisLine m_axisLine;
        private readonly AxisLabel m_axisLabel;

        private readonly AxisDimensions m_dims;


        public Axis(Edge edge)
        {
            m_pixelOffset = 0;
            m_rulerMode = false;
            m_visible = true;
            m_title = "";

            m_axisTick = new AxisTick();
            m_axisLine = new AxisLine();
            m_axisLabel = new AxisLabel();
            m_dims = new AxisDimensions();

            Edge = edge;
        }

        protected void Grid(bool enable) => m_axisTick.GridVisible = enable;
        protected void Tick(bool enable) => m_axisTick.Visible = enable;
        protected void Label(bool enable) => m_axisLabel.Visible = enable;
        protected void Line(bool enable) => m_axisLine.Visible = enable;

        public bool IsHorizontal => Edge.IsHorizontal();
        public bool IsVertical => Edge.IsVertical();

        public bool Visible
        {
            get => m_visible;
            set
            {
                m_visible = value;
                m_axisTick.Visible = value;
                m_axisLine.Visible = value;
                m_axisLabel.Visible = value;
            }
        }

        public bool RulerMode
        {
            get => m_rulerMode;
            set
            {
                m_rulerMode = value;
                m_axisTick.RulerMode = value;
                m_axisLine.RulerMode = value;
                m_axisLabel.RulerMode = value;
            }
        }

        public float PixelOffset
        {
            get => m_pixelOffset;
            set
            {
                m_pixelOffset = value;
                m_axisTick.PixelOffset = value;
                m_axisLabel.PixelOffset = value;
                m_axisLine.PixelOffset = value;
            }
        }

        public Edge Edge
        {
            get => m_edge;
            set
            {
                m_edge = value;
                m_dims.IsInverted = value.IsVertical();
                m_axisTick.Edge = m_edge;
                m_axisLabel.Edge = m_edge;
                m_axisLine.Edge = m_edge;
            }
        }

        public string Title
        {
            get => m_title;
            set
            {
                m_title = value;
                m_axisLabel.Label = value;
            }
        }



        public float FigureSizePx => m_dims.FigureSizePx;
        public float PlotSizePx => m_dims.PlotSizePx;
        public float DataSizePx => m_dims.DataSizePx;
        public float PlotOffsetPx => m_dims.PlotOffsetPx;
        public float DataOffsetPx => m_dims.DataOffsetPx;
        public bool IsInverted => m_dims.IsInverted;

        public (float min, float max) GetLimit() => m_dims.RationalLimits();
        public void Resize(float px, float plotSize, float dataSize, float p1, float plotOffset)
            => m_dims.Resize(px, plotSize, dataSize, p1, plotOffset);
        public void GetTicks(PlotDimensions dims) => m_axisTick.TickGenerator.GetTicks(dims);
        public bool HasBeenSet() => m_dims.HasBeenSet;
        public void SetLimit(float min, float max) => m_dims.SetLimits(min, max);

        #region Event
        public float LastMouseX { get; private set; }
        public float LastMouseY { get; private set; }
        public void MouseDown(float x, float y)
        {
            m_dims.Remember();
            LastMouseX = x;
            LastMouseY = y;
        }
        public void ZoomByPosition(float xfrac, float yfrac, float x, float y)
        {
            m_dims.Recall();

            float frac = IsHorizontal ? xfrac : yfrac;
            float centerPx = IsHorizontal ? x : y;
            float center = m_dims.GetUnit(centerPx);
            if (float.IsNaN(frac) || frac == 1.0f || float.IsNaN(center))
                return;

            m_dims.Zoom(frac, center);
        }
        public void ZoomByCenter(float newX, float newY)
        {
            m_dims.Recall();

            float deltaPx = IsHorizontal ? newX - LastMouseX : LastMouseY - newY;
            float delta = deltaPx * m_dims.UnitsPerPx;

            float deltaFrac = delta / (Math.Abs(delta) + m_dims.Center);

            float frac = (float)Math.Pow(10, deltaFrac);
            if (float.IsNaN(frac) || frac == 1.0f)
                return;

            m_dims.Zoom(frac);
        }
        public void PanAll(float newX, float newY)
        {
            m_dims.Recall();

            if (IsHorizontal)
                m_dims.PanPx(newX - LastMouseX);
            else
                m_dims.PanPx(newY - LastMouseY);
        }

        #endregion
        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            m_axisTick.Render(bmp, dims, lowQuality);
            m_axisLabel.Render(bmp, dims, lowQuality);
            m_axisLine.Render(bmp, dims, lowQuality);
        }

    }
}
