using Plot.Core.Draws;
using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    internal class AxisLine : IRenderable, IAxisComponent
    {
        public Edge Edge { get; set; }
        public float PixelOffset { get; set; }
        public bool Visible { get; set; } = true;
        public bool RulerMode { get; set; }

        // Axis Line
        internal bool AxisLineVisible { get; set; } = true;
        internal Color AxisLineColor { get; set; } = Color.Black;
        internal float AxisLineWidth { get; set; } = 1;


        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            // Draw axis line
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (AxisLineVisible)
                {
                    DrawLines(dims, gfx, AxisLineColor, AxisLineWidth, Edge, PixelOffset);
                }
            }
        }

        private void DrawLines(PlotDimensions dims, Graphics gfx, Color color, float lineWidth, Edge edge, float pixelOffset)
        {
            if (!Visible)
                return;
            using (var pen = GDI.Pen(color, lineWidth))
            {
                float left = dims.m_plotOffsetX - pixelOffset;
                float top = dims.m_plotOffsetY - pixelOffset;
                float dataWidth = left + dims.m_dataWidth + pixelOffset;
                float plotWidth = left + dims.m_plotWidth + pixelOffset;
                float dataHeight = top + dims.m_dataHeight + pixelOffset;
                float plotHeight = top + dims.m_plotHeight + pixelOffset;

                switch (edge)
                {
                    case Edge.Left:
                        gfx.DrawLine(pen, left, top, left, plotHeight);
                        break;
                    case Edge.Right:
                        gfx.DrawLine(pen, dataWidth, top, dataWidth, plotHeight);
                        break;
                    case Edge.Top:
                        gfx.DrawLine(pen, left, top, plotWidth, top);
                        break;
                    case Edge.Bottom:
                        gfx.DrawLine(pen, left, dataHeight, plotWidth, dataHeight);
                        break;
                    default:
                        throw new NotImplementedException($"unsupported edge type {Edge}");
                }
            }
        }
    }
}
