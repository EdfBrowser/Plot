using Plot.Core.Draws;
using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    public class AxisLine : IRenderable, IAxisComponent
    {
        public Edge Edge { get; set; }
        public bool Visible { get; set; } = true;

        // Axis Line
        public bool AxisLineVisible { get; set; } = true;
        public Color AxisLineColor { get; set; } = Color.Black;
        public float AxisLineWidth { get; set; } = 1f;


        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            // Draw axis line
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (AxisLineVisible)
                {
                    DrawLines(dims, gfx, AxisLineColor, AxisLineWidth, Edge);
                }
            }
        }

        private void DrawLines(PlotDimensions dims, Graphics gfx, Color color, float lineWidth, Edge edge)
        {
            using (var pen = GDI.Pen(color, lineWidth))
            {
                float left = dims.m_plotOffsetX;
                float top = dims.m_plotOffsetY;
                float dataWidth = dims.m_dataOffsetX + dims.m_dataWidth;
                float plotWidth = left + dims.m_plotWidth;
                float dataHeight = dims.m_dataOffsetY + dims.m_dataHeight;
                float plotHeight = top + dims.m_plotHeight;

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
