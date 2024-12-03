using Plot.Core.Draws;
using Plot.Core.Enum;
using System;
using System.Drawing;

namespace Plot.Core.Renderables.Axes
{
    public class AxisLabel : IRenderable, IAxisComponent
    {
        public Edge Edge { get; set; }
        public bool Visible { get; set; } = true;

        public string Label { get; set; }

        // Axis AxisLabel
        public bool AxisLabelVisible { get; set; } = true;
        public Color AxisLabelColor { get; set; } = Color.Black;
        public Font LabelFont { get; } = GDI.Font();
        public float PaddingSizePx { get; set; }

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (AxisLabelVisible)
                {
                    DrawLabels(dims, gfx, Label, LabelFont, AxisLabelColor, Edge);
                }
            }
        }

        private void DrawLabels(PlotDimensions dims, Graphics gfx, string label, Font labelFont, Color color, Edge edge)
        {
            if (string.IsNullOrWhiteSpace(label)) return;

            // 如何解析这个元组返回值
            var (x, y) = GetAxisCenter(dims, edge);

            int rotation;
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
                        sf.LineAlignment = StringAlignment.Near;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case Edge.Bottom:
                        // 原因是此时顶部对其，从y开始画label高度为height的矩形
                        // 如果是Far，则是从y-height开始画高度为height的矩形
                        sf.LineAlignment = StringAlignment.Far;
                        sf.Alignment = StringAlignment.Center;
                        break;
                    default:
                        throw new NotImplementedException($"unsupported edge type {edge}");
                }

                gfx.TranslateTransform(x, y);
                gfx.RotateTransform(rotation);
                gfx.DrawString(label, labelFont, brush, 0, 0, sf);
                gfx.ResetTransform();
            }
        }


        private (float, float) GetAxisCenter(PlotDimensions dims, Edge edge)
        {
            float x, y;
            switch (edge)
            {
                case Edge.Left:
                    x = dims.m_plotOffsetX - PaddingSizePx;
                    break;
                case Edge.Right:
                    x = dims.m_plotOffsetX + dims.m_dataWidth + PaddingSizePx;
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
                    y = dims.m_plotOffsetY - PaddingSizePx;
                    break;
                case Edge.Bottom:
                    y = dims.m_plotOffsetY + dims.m_dataHeight + PaddingSizePx;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            return (x, y);
        }

        public SizeF Measure() => GDI.MeasureStringUsingTemporaryGraphics(Label, LabelFont);

    }
}
