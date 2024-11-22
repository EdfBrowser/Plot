using Plot.Core.Draws;
using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    internal class AxisLabel : IRenderable, IAxisComponent
    {
        public Edge Edge { get; set; }
        public float PixelOffset { get; set; }
        public bool Visible { get; set; } = true;
        public bool RulerMode { get; set; }

        internal string Label { get; set; }

        // Axis AxisLabel
        internal bool AxisLabelVisible { get; set; } = true;
        internal Color AxisLabelColor { get; set; } = Color.Black;
        internal float AxisLabelWidth { get; set; } = 1;

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (AxisLabelVisible)
                {
                    float labelHeight = 14;
                    //using (var font = GDI.Font(null))
                    //{
                    //    SizeF size = GDI.MeasureString(gfx, Generator.TicksMajor.FirstOrDefault()?.Title, font);
                    //    labelHeight = Edge.IsHorizontal() ? size.Height : size.Width;
                    //}
                    DrawLabels(dims, gfx, Label, null, AxisLabelColor, AxisLabelWidth, Edge, PixelOffset, 5, labelHeight);
                }
            }
        }

        private void DrawLabels(PlotDimensions dims, Graphics gfx, string label, string tickFont,
         Color color, float lineWidth, Edge edge, float pixelOffset, float tickLength, float labelHeight)
        {
            if (string.IsNullOrWhiteSpace(label)) return;


            // 如何解析这个元组返回值
            var (x, y) = GetAxisCenter(dims, edge, pixelOffset, tickLength, labelHeight);

            int rotation = 0;
            switch (edge)
            {
                case Edge.Left:
                    rotation = -90;
                    break;
                case Edge.Right:
                    rotation = 90;
                    break;
                case Edge.Top:
                    rotation = 0;
                    break;
                case Edge.Bottom:
                    rotation = 0;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            using (var font = GDI.Font(tickFont))
            using (var brush = GDI.Brush(color))
            using (var sf = new StringFormat())
            {
                switch (edge)
                {
                    case Edge.Left:
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case Edge.Right:
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case Edge.Top:
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case Edge.Bottom:
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    default:
                        throw new NotImplementedException($"unsupported edge type {edge}");
                }

                SizeF size = GDI.MeasureString(gfx, label, font);
                if (edge == Edge.Bottom)
                {
                    y += size.Height;
                }
                else if (edge == Edge.Left)
                {
                    x -= size.Height;
                }
                gfx.TranslateTransform(x, y);
                gfx.RotateTransform(rotation);
                gfx.DrawString(label, font, brush, 0, 0, sf);
                gfx.ResetTransform();
            }
        }


        private (float x, float y) GetAxisCenter(PlotDimensions dims, Edge edge, float pixelOffset, float tickLength, float labelHeight)
        {
            float x = 0, y = 0;
            switch (edge)
            {
                case Edge.Left:
                    x = dims.m_plotOffsetX - pixelOffset - tickLength - labelHeight;
                    break;
                case Edge.Right:
                    x = dims.m_plotOffsetX + dims.m_dataWidth + pixelOffset + tickLength + labelHeight;
                    break;
                case Edge.Top:
                    x = dims.m_plotOffsetX + dims.m_plotWidth / 2;
                    break;
                case Edge.Bottom:
                    x = dims.m_plotOffsetX + dims.m_plotWidth / 2;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            switch (edge)
            {
                case Edge.Left:
                    y = dims.m_plotOffsetY + dims.m_plotHeight / 2;
                    break;
                case Edge.Right:
                    y = dims.m_plotOffsetY + dims.m_plotHeight / 2;
                    break;
                case Edge.Top:
                    y = dims.m_plotOffsetY - pixelOffset - tickLength - labelHeight;
                    break;
                case Edge.Bottom:
                    y = dims.m_plotOffsetY + dims.m_dataHeight + pixelOffset + tickLength + labelHeight;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            return (x, y);
        }
    }
}
