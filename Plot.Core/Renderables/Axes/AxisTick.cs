using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.Ticks;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Plot.Core.Renderables.Axes
{
    internal class AxisTick : IRenderable, IAxisComponent
    {
        private Edge m_edge;
        internal TickGenerator TickGenerator { get; } = new TickGenerator();

        public bool RulerMode { get; set; }
        public Edge Edge
        {
            get => m_edge; set
            {
                m_edge = value;
                TickGenerator.IsVertical = value.IsVertical();

            }
        }
        public float PixelOffset { get; set; }

        public bool Visible { get; set; } = true;

        // Direction
        internal bool TicksExtendOutward { get; } = true;

        // Tick AxisLabel
        internal bool TickLabelVisible { get; set; } = true;
        internal int TickLabelRotation { get; set; } = 0;
        internal Color TickLabelColor { get; set; } = Color.Black;

        // Major Tick
        internal bool MajorTickVisible { get; set; } = true;
        internal float MajorTickWidth { get; set; } = 1;
        internal float MajorTickLength { get; set; } = 5;
        internal Color MajorTickColor { get; set; } = Color.Black;

        // Minor Tick
        internal bool MinorTickVisible { get; set; } = true;
        internal float MinorTickLength { get; set; } = 2;
        internal float MinorTickWidth { get; set; } = 1;
        internal Color MinorTickColor { get; set; } = Color.Black;

        internal bool GridVisible { get; set; } = true;
        internal DashStyle MajorGridStyle { get; set; } = DashStyle.Solid;
        internal Color MajorGridColor { get; set; } = Color.LightGray;
        internal float MajorGridWidth { get; set; } = 1;
        internal float minorGridWidth { get; set; } = 1;
        internal Color minorGridColor { get; set; } = Color.LightGray;
        internal DashStyle minorGridStyle { get; set; } = DashStyle.Solid;

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (GridVisible)
                {
                    float[] majorTicks = TickGenerator.TicksMajor.Select(t => t.m_posPixel).ToArray();
                    float[] minorTicks = TickGenerator.TicksMinor.Select(t => t.m_posPixel).ToArray();
                    DrawGridLines(dims, gfx, majorTicks, MajorGridStyle, MajorGridColor, MajorGridWidth, Edge);
                    DrawGridLines(dims, gfx, minorTicks, minorGridStyle, minorGridColor, minorGridWidth, Edge);
                }

                // Major ticks
                if (MajorTickVisible)
                {
                    float tickLength = MajorTickLength;
                    if (RulerMode)
                        tickLength *= 4;
                    tickLength = TicksExtendOutward ? tickLength : -tickLength;
                    float[] ticks = TickGenerator.TicksMajor.Select(t => t.m_posPixel).ToArray();
                    DrawTicks(dims, gfx, ticks, tickLength, MajorTickColor, Edge, PixelOffset, MajorTickWidth);
                }


                // Minor ticks
                if (MinorTickVisible)
                {
                    float[] ticks = TickGenerator.TicksMinor.Select(t => t.m_posPixel).ToArray();
                    float tickLength = TicksExtendOutward ? MinorTickLength : -MinorTickLength;
                    DrawTicks(dims, gfx, ticks, tickLength, MinorTickColor, Edge, PixelOffset, MinorTickWidth);
                }

                if (TickLabelVisible)
                {
                    DrawTicksLabel(dims, gfx, TickGenerator.TicksMajor, null, Edge, TickLabelRotation, RulerMode, PixelOffset, MajorTickLength);
                }
            }
        }

        private void DrawGridLines(PlotDimensions dims, Graphics gfx, float[] ticks, DashStyle style,
            Color color, float tickWidth, Edge edge)
        {
            if (ticks == null || ticks.Length == 0) return;

            if (edge.IsHorizontal())
            {
                float y = edge == Edge.Top ?
                    dims.m_plotOffsetY : dims.m_plotOffsetY + dims.m_dataHeight;
                float y2 = (edge == Edge.Top) ? dims.m_plotOffsetY + dims.m_dataHeight : dims.m_plotOffsetY;

                var xs = ticks.Select(t => dims.GetPixelX(t));
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    pen.DashStyle = style;
                    foreach (var x in xs)
                    {
                        gfx.DrawLine(pen, x, y, x, y2);
                    }
                }
            }
            else if (edge.IsVertical())
            {
                float x = edge == Edge.Left ?
                     dims.m_plotOffsetX : dims.m_plotOffsetX + dims.m_dataWidth;
                float x2 = edge == Edge.Left ? dims.m_plotOffsetX + dims.m_dataWidth : dims.m_plotOffsetX;

                var ys = ticks.Select(t => dims.GetPixelY(t));
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    pen.DashStyle = style;
                    foreach (var y in ys)
                    {
                        gfx.DrawLine(pen, x, y, x2, y);
                    }
                }
            }
        }

        private static void DrawTicks(PlotDimensions dims, Graphics gfx, float[] ticks, float tickLength,
        Color color, Edge edge, float pixelOffset, float tickWidth)
        {
            if (ticks == null || ticks.Length == 0) return;

            if (edge.IsHorizontal())
            {
                float y = edge == Edge.Top ?
                    dims.m_plotOffsetY - pixelOffset : dims.m_plotOffsetY + dims.m_dataHeight + pixelOffset;
                float tickDelta = edge == Edge.Top ? -tickLength : tickLength;

                var xs = ticks.Select(t => dims.GetPixelX(t));
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    foreach (var x in xs)
                    {
                        gfx.DrawLine(pen, x, y, x, y + tickDelta);
                    }
                }
            }
            else if (edge.IsVertical())
            {
                float x = edge == Edge.Left ?
                     dims.m_plotOffsetX - pixelOffset : dims.m_plotOffsetX + dims.m_dataWidth + pixelOffset;
                float tickDelta = edge == Edge.Left ? -tickLength : tickLength;

                var ys = ticks.Select(t => dims.GetPixelY(t));
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    foreach (var y in ys)
                    {
                        gfx.DrawLine(pen, x, y, x + tickDelta, y);
                    }
                }
            }
        }


        private void DrawTicksLabel(PlotDimensions dims, Graphics gfx, Tick[] majorTicks, string tickFont, Edge edge,
            float rotation, bool rulerMode, float pixelOffset, float majorTickLength)
        {
            if (majorTicks == null || majorTicks.Length == 0) return;

            using (var font = GDI.Font(tickFont))
            using (var brush = GDI.Brush(TickLabelColor))
            using (var sf = new StringFormat())
            {
                switch (Edge)
                {
                    case Edge.Left:
                        for (int i = 0; i < majorTicks.Length; i++)
                        {
                            float x = dims.m_plotOffsetX - pixelOffset - majorTickLength;
                            float y = dims.GetPixelY(majorTicks[i].m_posPixel);

                            sf.Alignment = StringAlignment.Far;
                            sf.LineAlignment = rulerMode ? StringAlignment.Far : StringAlignment.Center;
                            if (rotation == 90)
                            {
                                sf.Alignment = StringAlignment.Center;
                                sf.LineAlignment = StringAlignment.Far;
                            }

                            gfx.TranslateTransform(x, y);
                            gfx.RotateTransform(-rotation);
                            gfx.DrawString(majorTicks[i].Label, font, brush, 0, 0, sf);
                            gfx.ResetTransform();
                        }
                        break;
                    case Edge.Right:
                        break;
                    case Edge.Top:
                        break;
                    case Edge.Bottom:
                        for (int i = 0; i < majorTicks.Length; i++)
                        {
                            float x = dims.GetPixelX(majorTicks[i].m_posPixel);
                            float y = dims.m_plotOffsetY + dims.m_dataHeight + majorTickLength + pixelOffset;

                            sf.Alignment = rotation == 0 ? StringAlignment.Center : StringAlignment.Far;
                            if (rulerMode) sf.Alignment = StringAlignment.Near;
                            sf.LineAlignment = rotation == 0 ? StringAlignment.Near : StringAlignment.Center;

                            gfx.TranslateTransform(x, y);
                            gfx.RotateTransform(-rotation);
                            gfx.DrawString(majorTicks[i].Label, font, brush, 0, 0, sf);
                            gfx.ResetTransform();
                        }
                        break;
                    default:
                        throw new NotImplementedException($"unsupported edge type {edge}");
                }
            }
        }
    }
}
