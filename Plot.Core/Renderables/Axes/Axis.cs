using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    public abstract class Axis : IRenderable, IAxisComponent
    {
        private Edge m_edge;
        private bool m_visible;
        private bool m_isDateTime;

        public Axis(Edge edge)
        {
            m_visible = true;

            Edge = edge;
        }

        public AxisDimensions Dims { get; } = new AxisDimensions();
        // TODO: 当plotSize很小时，不需要显示刻度
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

        public bool IsDateTime
        {
            get => m_isDateTime;
            set
            {
                m_isDateTime = value;
                Dims.IsDateTime = value;
                AxisTick.TickGenerator.LabelFormat = value ? TickLabelFormat.DateTime : TickLabelFormat.Numeric;
            }
        }

        public float PaddingSizePx { get; private set; }
        public float MinimalPadding { get; set; } = 10.0f;

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;

            AxisLabel.PaddingSizePx = PaddingSizePx;
            AxisTick.Render(bmp, dims, lowQuality);
            AxisLabel.Render(bmp, dims, lowQuality);
            AxisLine.Render(bmp, dims, lowQuality);
        }

        public void RecalculateTickPositions(PlotDimensions dimsFull) => AxisTick.TickGenerator.Recalculate(dimsFull, AxisTick.TickFont);

        public void ReCalculateAxisSize()
        {
            PaddingSizePx = 0f;

            if (AxisLabel.Visible)
                PaddingSizePx += AxisLabel.Measure().Height;

            // 刻度线
            if (AxisTick.Visible && AxisTick.MajorTickVisible)
                PaddingSizePx += AxisTick.MajorTickLength;

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
            }
        }

        public float GetSize() => Visible ? PaddingSizePx + MinimalPadding : 0;
    }
}
