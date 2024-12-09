using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    public abstract class Axis : IRenderable, IAxisComponent
    {
        private Edge m_edge;
        private bool m_visible;

        public Axis(Edge edge)
        {
            m_visible = true;

            Edge = edge;
        }

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

        public float LabelOffsetPx { get; private set; }
        public float MarginSizePx { get; private set; }
        public float MinimalMargin { get; set; } = 10.0f;

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;

            AxisLabel.OffsetPx = LabelOffsetPx;
            AxisTick.Render(bmp, dims, lowQuality);
            AxisLabel.Render(bmp, dims, lowQuality);
            AxisLine.Render(bmp, dims, lowQuality);
        }

        public void RecalculateTickPositions(PlotDimensions dimsFull) => AxisTick.TickGenerator.Recalculate(dimsFull, AxisTick.TickFont);

        public void ReCalculateAxisSize()
        {
            MarginSizePx = 0f;

            // 刻度线
            if (AxisTick.Visible && AxisTick.MajorTickVisible)
                MarginSizePx += AxisTick.MajorTickLength;

            if (AxisTick.Visible && AxisTick.TickLabelVisible)
            {
                float originalWidth = AxisTick.TickGenerator.LargestLabelWidth;
                float originalHeight = AxisTick.TickGenerator.LargestLabelHeight;

                float angle = AxisLabel.Rotation; // 获取实际旋转角度
                double radians = angle * Math.PI / 180;

                float rotatedWidth = (float)(Math.Abs(originalWidth * Math.Cos(radians)) + Math.Abs(originalHeight * Math.Sin(radians)));
                float rotatedHeight = (float)(Math.Abs(originalWidth * Math.Sin(radians)) + Math.Abs(originalHeight * Math.Cos(radians)));

                float largerRotatedSize = Math.Max(rotatedWidth, rotatedHeight);

                MarginSizePx += largerRotatedSize;

                LabelOffsetPx = MarginSizePx;
            }

            if (AxisLabel.Label != null && AxisLabel.Visible && AxisLabel.LabelExtendOutward)
            {
                SizeF size = AxisLabel.Measure();
                float originalWidth = size.Width;
                float originalHeight = size.Height;

                float angle = AxisLabel.Rotation; // 获取实际旋转角度
                double radians = angle * Math.PI / 180;

                float rotatedWidth = (float)(Math.Abs(originalWidth * Math.Cos(radians)) + Math.Abs(originalHeight * Math.Sin(radians)));
                float rotatedHeight = (float)(Math.Abs(originalWidth * Math.Sin(radians)) + Math.Abs(originalHeight * Math.Cos(radians)));

                float largerRotatedSize = Math.Max(rotatedWidth, rotatedHeight);

                MarginSizePx += largerRotatedSize;
            }
        }

        public float GetSize() => Visible ? MarginSizePx + MinimalMargin : 0;

        public DateTime Origin { get; private set; }
        public void SetDateTimeOrigin(DateTime startDateTime) => Origin = startDateTime;
    }
}
