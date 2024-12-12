using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.Ticks;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace Plot.Core.Renderables.Axes
{
    public class AxisTick : IRenderable, IAxisComponent
    {
        private Edge m_edge;
        public TickGenerator TickGenerator { get; } = new TickGenerator();

        public Edge Edge
        {
            get => m_edge; set
            {
                m_edge = value;
                TickGenerator.IsVertical = value.IsVertical();
            }
        }

        public bool Visible { get; set; } = true;

        // Direction
        public bool TicksExtendOutward { get; } = true;

        // Tick AxisLabel
        public bool TickLabelVisible { get; set; } = true;
        private float m_tickLabelRotation = 0;
        public float TickLabelRotation
        {
            get => m_tickLabelRotation;
            set
            {
                m_tickLabelRotation = value;
                TickGenerator.Rotation = value;
            }
        }
        public Color TickLabelColor { get; set; } = Color.Black;

        // Major Tick
        public bool MajorTickVisible { get; set; } = true;
        public float MajorTickWidth { get; set; } = 1;
        public float MajorTickLength { get; set; } = 5;
        public Color MajorTickColor { get; set; } = Color.Black;

        // Minor Tick
        public bool MinorTickVisible { get; set; } = true;
        public float MinorTickLength { get; set; } = 2;
        public float MinorTickWidth { get; set; } = 1;
        public Color MinorTickColor { get; set; } = Color.Black;

        public bool GridVisible { get; set; } = true;
        public DashStyle MajorGridStyle { get; set; } = DashStyle.Solid;
        public Color MajorGridColor { get; set; } = Color.LightGray;
        public float MajorGridWidth { get; set; } = 1;
        public float MinorGridWidth { get; set; } = 1;
        public Color MinorGridColor { get; set; } = Color.LightGray;
        public DashStyle MinorGridStyle { get; set; } = DashStyle.Solid;
        public Font TickFont { get; set; } = GDI.Font();
        public StringAlignment HorizontalAlignment { get; set; } = StringAlignment.Near;
        public StringAlignment VerticalAlignment { get; set; } = StringAlignment.Near;



        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            if (!Visible)
                return;
            double[] majorTicks = TickGenerator.GetVisibleMajorTicks(dims)
             .Select(t => t.m_position)
             .ToArray();

            double[] minorTicks = TickGenerator.GetVisibleMinorTicks(dims)
                .Select(t => t.m_position)
                .ToArray();

            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                if (GridVisible)
                {
                    DrawGridLines(dims, gfx, majorTicks, MajorGridStyle, MajorGridColor, MajorGridWidth, Edge);
                    DrawGridLines(dims, gfx, minorTicks, MinorGridStyle, MinorGridColor, MinorGridWidth, Edge);
                }

                // Major ticks
                if (MajorTickVisible)
                {
                    float tickLength = MajorTickLength;
                    //if (RulerMode)
                    //    tickLength *= 4;
                    tickLength = TicksExtendOutward ? tickLength : -tickLength;
                    DrawTicks(dims, gfx, majorTicks, tickLength, MajorTickColor, Edge, MajorTickWidth);
                }


                // Minor ticks
                if (MinorTickVisible)
                {
                    float tickLength = TicksExtendOutward ? MinorTickLength : -MinorTickLength;
                    DrawTicks(dims, gfx, minorTicks, tickLength, MinorTickColor, Edge, MinorTickWidth);
                }

                if (TickLabelVisible)
                {
                    DrawTicksLabel(dims, gfx, TickGenerator.GetVisibleMajorTicks(dims), Edge, TickLabelRotation, MajorTickLength);
                }
            }
        }

        private void DrawGridLines(PlotDimensions dims, Graphics gfx, double[] ticks, DashStyle style,
            Color color, float tickWidth, Edge edge)
        {
            if (ticks == null || ticks.Length == 0) return;
            // don't draw grid lines on the last pixel to prevent drawing over the data frame
            float xEdgeLeft = dims.m_dataOffsetX + 1;
            float xEdgeRight = dims.m_dataOffsetX + dims.m_dataWidth - 1;
            float yEdgeTop = dims.m_dataOffsetY + 1;
            float yEdgeBottom = dims.m_dataOffsetY + dims.m_dataHeight - 1;

            if (edge.IsHorizontal())
            {
                var xs = ticks.Select(t => dims.GetPixelX(t)).Where(x => xEdgeLeft < x && x < xEdgeRight);
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    pen.DashStyle = style;
                    foreach (var x in xs)
                    {
                        gfx.DrawLine(pen, x, yEdgeTop, x, yEdgeBottom);
                    }
                }
            }
            else if (edge.IsVertical())
            {
                var ys = ticks.Select(t => dims.GetPixelY(t)).Where(y => yEdgeTop < y && y < yEdgeBottom);
                using (var pen = GDI.Pen(color, tickWidth))
                {
                    pen.DashStyle = style;
                    foreach (var y in ys)
                    {
                        gfx.DrawLine(pen, xEdgeLeft, y, xEdgeRight, y);
                    }
                }
            }
        }

        private static void DrawTicks(PlotDimensions dims, Graphics gfx, double[] ticks, float tickLength,
        Color color, Edge edge, float tickWidth)
        {
            if (ticks == null || ticks.Length == 0) return;

            if (edge.IsHorizontal())
            {
                float y = edge == Edge.Top ?
                    dims.m_plotOffsetY : dims.m_plotOffsetY + dims.m_dataHeight;
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
                     dims.m_plotOffsetX : dims.m_plotOffsetX + dims.m_dataWidth;
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


        private void DrawTicksLabel(PlotDimensions dims, Graphics gfx, Tick[] majorTicks, Edge edge, float rotation, float majorTickLength)
        {
            if (majorTicks == null || majorTicks.Length == 0) return;

            using (var brush = GDI.Brush(TickLabelColor))
            using (var sf = new StringFormat())
            {
                switch (Edge)
                {
                    case Edge.Left:
                        for (int i = 0; i < majorTicks.Length; i++)
                        {
                            float x = dims.m_plotOffsetX - majorTickLength;
                            float y = dims.GetPixelY(majorTicks[i].m_position);

                            sf.Alignment = HorizontalAlignment;
                            sf.LineAlignment = VerticalAlignment;

                            gfx.TranslateTransform(x, y);
                            gfx.RotateTransform(rotation);
                            gfx.DrawString(majorTicks[i].m_label, TickFont, brush, 0, 0, sf);
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
                            float x = dims.GetPixelX(majorTicks[i].m_position);
                            float y = dims.m_plotOffsetY + dims.m_dataHeight + majorTickLength;

                            sf.Alignment = HorizontalAlignment;
                            sf.LineAlignment = VerticalAlignment;

                            gfx.TranslateTransform(x, y);
                            gfx.RotateTransform(rotation);
                            gfx.DrawString(majorTicks[i].m_label, TickFont, brush, 0, 0, sf);
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
