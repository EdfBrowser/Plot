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

        public bool LabelExtendOutward { get; set; } = true;
        public string Label { get; set; }

        // Axis AxisLabel
        public bool AxisLabelVisible { get; set; } = true;
        public Color AxisLabelColor { get; set; } = Color.Black;
        public Font LabelFont { get; set; } = GDI.Font(fontSize: 14);
        public float OffsetPx { get; set; }
        public float Rotation { get; set; } = 0;

        public StringAlignment HorizontalAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment VerticalAlignment { get; set; } = StringAlignment.Near;


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

            using (var brush = GDI.Brush(color))
            using (var sf = new StringFormat())
            {
                sf.Alignment = HorizontalAlignment;
                sf.LineAlignment = VerticalAlignment;

                gfx.TranslateTransform(x, y);
                gfx.RotateTransform(-Rotation);
                gfx.DrawString(label, labelFont, brush, 0, 0, sf);
                gfx.ResetTransform();
            }
        }


        private (float, float) GetAxisCenter(PlotDimensions dims, Edge edge)
        {
            float x, y;
            float marginSizePx = LabelExtendOutward ? OffsetPx : -OffsetPx;
            float delta;
            switch (edge)
            {
                case Edge.Left:
                    delta = -marginSizePx;
                    x = dims.m_plotOffsetX + delta;
                    break;
                case Edge.Right:
                    delta = marginSizePx;
                    x = dims.m_plotOffsetX + dims.m_dataWidth + delta;
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
                    delta = -marginSizePx;
                    y = dims.m_plotOffsetY + delta;
                    break;
                case Edge.Bottom:
                    delta = marginSizePx;
                    y = dims.m_plotOffsetY + dims.m_dataHeight + delta;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            return (x, y);
        }

        public SizeF Measure() => GDI.MeasureStringUsingTemporaryGraphics(Label, LabelFont);

    }
}
