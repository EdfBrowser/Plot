using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core
{
    public class Axis
    {
        private Edge m_edge;

        public Axis(Edge edge, int axisIndex)
        {
            Edge = edge;
            AxisIndex = axisIndex;
        }

        public string AxisLabel { get; set; }

        public int AxisIndex { get; }

        public AxisDimensions Dims { get; } = new AxisDimensions();
        public TickGenerator Generator { get; } = new TickGenerator();

        public bool IsHorizontal => Edge.IsHorizontal();

        public bool IsVertical => Edge.IsVertical();

        public Edge Edge
        {
            get => m_edge;
            private set
            {
                m_edge = value;
                Generator.IsVertical = value.IsVertical();
                Dims.IsInverted = value.IsVertical();
            }
        }

        public float PixelOffset { get; set; } = 0;
        public bool RulerMode { get; set; } = false;
        // Direction
        public bool TicksExtendOutward { get; } = true;

        // Axis AxisLabel
        public bool AxisLabelVisible { get; set; } = true;
        public Color AxisLabelColor { get; set; } = Color.Black;
        public float AxisLabelWidth { get; set; } = 1;

        // Axis Line
        public bool AxisLineVisible { get; set; } = true;
        public Color AxisLineColor { get; set; } = Color.Black;
        public float AxisLineWidth { get; set; } = 1;

        // Tick AxisLabel
        public bool TickLabelVisible { get; set; } = true;
        public int TickLabelRotation { get; set; } = 0;
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


        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality)
        {
            // Draw axis line
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                // Major ticks
                if (MajorTickVisible)
                {
                    float tickLength = MajorTickLength;
                    if (RulerMode)
                        tickLength *= 4;
                    tickLength = TicksExtendOutward ? tickLength : -tickLength;
                    float[] ticks = Generator.TicksMajor.Select(t => t.PosPixel).ToArray();
                    DrawTicks(dims, gfx, ticks, tickLength, MajorTickColor, Edge, PixelOffset, MajorTickWidth);
                }


                // Minor ticks
                if (MinorTickVisible)
                {
                    float[] ticks = Generator.TicksMinor.Select(t => t.PosPixel).ToArray();
                    float tickLength = TicksExtendOutward ? MinorTickLength : -MinorTickLength;
                    DrawTicks(dims, gfx, ticks, tickLength, MinorTickColor, Edge, PixelOffset, MinorTickWidth);
                }

                if (TickLabelVisible)
                {
                    DrawTicksLabel(dims, gfx, Generator.TicksMajor, null, Edge, TickLabelRotation, RulerMode, PixelOffset, MajorTickLength);
                }

                if (AxisLabelVisible)
                {
                    float labelHeight = 0;
                    using (var font = GDI.Font(null))
                    {
                        SizeF size = GDI.MeasureString(gfx, Generator.TicksMajor.FirstOrDefault()?.Label, font);
                        labelHeight = Edge.IsHorizontal() ? size.Height : size.Width;
                    }
                    DrawLabels(dims, gfx, AxisLabel, null, AxisLabelColor, AxisLabelWidth, Edge, PixelOffset, MajorTickLength, labelHeight);
                }

                if (AxisLineVisible)
                {
                    DrawLines(dims, gfx, AxisLineColor, AxisLineWidth, Edge, PixelOffset);
                }
            }
        }

        private void DrawLines(PlotDimensions dims, Graphics gfx, Color color, float lineWidth, Edge edge, float pixelOffset)
        {
            using (var pen = GDI.Pen(color, lineWidth, 1))
            {
                float left = dims.DataOffsetX - pixelOffset;
                float right = left + dims.PlotWidth + pixelOffset;
                float top = dims.DataOffsetY - pixelOffset;
                float bottom = dims.DataOffsetY + dims.PlotHeight + pixelOffset;
                float bottom1 = dims.DataOffsetY + dims.DataHeight + pixelOffset;

                switch (edge)
                {
                    case Edge.Left:
                        gfx.DrawLine(pen, left, top, left, bottom1);
                        break;
                    case Edge.Right:
                        gfx.DrawLine(pen, right, top, right, bottom1);
                        break;
                    case Edge.Top:
                        gfx.DrawLine(pen, left, top, right, top);
                        break;
                    case Edge.Bottom:
                        gfx.DrawLine(pen, left, bottom, right, bottom);
                        break;
                    default:
                        throw new NotImplementedException($"unsupported edge type {Edge}");
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
                if (edge.IsHorizontal())
                {
                    y += size.Height;
                }
                else if (edge.IsVertical())
                {
                    x -= size.Height;
                }
                gfx.TranslateTransform(x, y);
                gfx.RotateTransform(rotation);
                gfx.DrawString(label, font, brush, 0, 0, sf);
                gfx.ResetTransform();
            }
        }


        private static void DrawTicks(PlotDimensions dims, Graphics gfx, float[] ticks, float tickLength,
            Color color, Edge edge, float pixelOffset, float tickWidth)
        {
            if (ticks == null || ticks.Length == 0) return;

            if (edge.IsHorizontal())
            {
                float y = (edge == Edge.Top) ?
                    dims.DataOffsetY - pixelOffset : dims.DataOffsetY + dims.PlotHeight + pixelOffset;
                float tickDelta = (edge == Edge.Top) ? -tickLength : tickLength;

                var xs = ticks.Select(t => dims.GetPixelX(t));
                using (var pen = GDI.Pen(color, tickWidth, 1))
                {
                    foreach (var x in xs)
                    {
                        gfx.DrawLine(pen, x, y, x, y + tickDelta);
                    }
                }
            }
            else if (edge.IsVertical())
            {
                float x = (edge == Edge.Left) ?
                     dims.DataOffsetX - pixelOffset : dims.DataOffsetX + dims.PlotWidth + pixelOffset;
                float tickDelta = (edge == Edge.Left) ? -tickLength : tickLength;

                var ys = ticks.Select(t => dims.GetPixelY(t));
                using (var pen = GDI.Pen(color, tickWidth, 1))
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
            using (var brush = GDI.Brush(TickLabelColor, 1))
            using (var sf = new StringFormat())
            {
                switch (Edge)
                {
                    case Edge.Left:
                        for (int i = 0; i < majorTicks.Length; i++)
                        {
                            float x = dims.DataOffsetX - pixelOffset - majorTickLength;
                            float y = dims.GetPixelY(majorTicks[i].PosPixel);

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
                            float x = dims.GetPixelX(majorTicks[i].PosPixel);
                            float y = dims.DataOffsetY + dims.PlotHeight + majorTickLength + pixelOffset;

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

        private (float x, float y) GetAxisCenter(PlotDimensions dims, Edge edge, float pixelOffset, float tickLength, float labelHeight)
        {
            float x = 0, y = 0;
            switch (edge)
            {
                case Edge.Left:
                    x = dims.DataOffsetX - pixelOffset - tickLength - labelHeight;
                    break;
                case Edge.Right:
                    x = dims.DataOffsetX + dims.PlotWidth + pixelOffset + tickLength + labelHeight;
                    break;
                case Edge.Top:
                    x = dims.DataOffsetX + dims.DataWidth / 2;
                    break;
                case Edge.Bottom:
                    x = dims.DataOffsetX + dims.DataWidth / 2;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            switch (edge)
            {
                case Edge.Left:
                    y = dims.DataOffsetY + dims.DataHeight / 2;
                    break;
                case Edge.Right:
                    y = dims.DataOffsetY + dims.DataHeight / 2;
                    break;
                case Edge.Top:
                    y = dims.DataOffsetY - pixelOffset - tickLength - labelHeight;
                    break;
                case Edge.Bottom:
                    y = dims.DataOffsetY + dims.PlotHeight + pixelOffset + tickLength + labelHeight;
                    break;
                default:
                    throw new NotImplementedException($"unsupported edge type {edge}");
            }

            return (x, y);
        }
    }

    public class TickGenerator
    {
        public Tick[] TicksMajor { get; private set; }
        public Tick[] TicksMinor { get; private set; }

        public bool IsVertical { get; set; } = true;

        private readonly float m_pixelsPerTick = 70;

        public float TickSpacingPx { get; set; } = 100;


        public void GetTicks(PlotDimensions dims)
        {
            float span, pxSize, min, max;
            if (IsVertical)
            {
                span = dims.YSpan;
                pxSize = dims.DataHeight;
                min = dims.YMin;
                max = dims.YMax;
            }
            else
            {
                span = dims.XSpan;
                pxSize = dims.DataWidth;
                min = dims.XMin;
                max = dims.XMax;
            }

            TicksMajor = Calculate(pxSize, span, min, max, true);
            TicksMinor = Calculate(pxSize, span, min, max, false);
        }

        private Tick[] Calculate(float pxSize, float span, float min, float max, bool major = true)
        {
            List<Tick> ticks = new List<Tick>();

            if (pxSize < TickSpacingPx / 2) return Array.Empty<Tick>();

            float minimumTickCount = pxSize / TickSpacingPx;
            if (major == false) minimumTickCount *= 5;

            float tickSpacing = (float)GetIdealTickSpacing(span, (int)minimumTickCount);
            int tickCount = (int)(span / tickSpacing) + 1;

            // To get an integer scale
            // For example, spacing = 1, min = 5.7
            // min % spacing = 0.7 , Therefore, the first scale will move from 5.7 to 0.7,
            // becoming an integer scale
            float tickOffsetFromMin = min % tickSpacing;

            for (int i = 0; i < tickCount + 1; i++)
            {
                float tickDelta = i * tickSpacing - tickOffsetFromMin;
                float posUnit = min + tickDelta;

                if (posUnit > min && posUnit < max)
                {
                    ticks.Add(new Tick(posUnit, posUnit, tickSpacing));
                }
            }

            return ticks.ToArray();
        }

        private double GetIdealTickSpacing(float span, int tickCountTarget)
        {
            double tickSpacing = 0;
            for (int powerOfTen = 10; powerOfTen > -10; powerOfTen--)
            {
                tickSpacing = Math.Pow(10, powerOfTen);

                if (tickSpacing > span) continue;

                double tickCount = span / tickSpacing;
                if (tickCount >= tickCountTarget)
                {
                    // a good tick density
                    if (tickCount >= tickCountTarget * 5) return tickSpacing * 5;
                    if (tickCount >= tickCountTarget * 2) return tickSpacing * 2;
                    return tickSpacing;
                }
            }

            return 0;
        }


        [Obsolete("use GetTicks instead")]
        public void RecalculateTicks(PlotDimensions dims)
        {
            float tick_density = 0;
            if (IsVertical)
            {
                tick_density = dims.DataHeight / m_pixelsPerTick;
            }
            else
            {
                tick_density = dims.DataWidth / m_pixelsPerTick;
            }
            TicksMinor = AutoCalculate(dims, (int)(tick_density * 5));
            TicksMajor = AutoCalculate(dims, (int)(tick_density * 1));
        }

        [Obsolete]
        private Tick[] AutoCalculate(PlotDimensions dims, int targetTickCount)
        {
            float span, pxSize, unitsPerPx, min, max;
            if (IsVertical)
            {
                span = dims.YSpan;
                pxSize = dims.DataHeight;
                unitsPerPx = dims.UnitsPerPxY;
                min = dims.YMin;
                max = dims.YMax;
            }
            else
            {
                span = dims.XSpan;
                pxSize = dims.DataWidth;
                unitsPerPx = dims.UnitsPerPxX;
                min = dims.XMin;
                max = dims.XMax;
            }

            return GenerateTicks(span, pxSize, unitsPerPx, min, max, targetTickCount);
        }
        [Obsolete]
        private Tick[] GenerateTicks(float span, float pxSize, float unitsPerPx,
            float min, float max, float targetTickCount)
        {
            if (targetTickCount <= 0)
                return Array.Empty<Tick>();

            List<Tick> ticks = new List<Tick>();

            // Size value of every tick
            double tickSize = RoundNumberNear(span / targetTickCount * 1.5);
            int lastTick = 123456789;
            // 
            for (int i = 0; i < pxSize; i++)
            {
                float thisPos = i * unitsPerPx + min;
                // 
                int thisTick = (int)(thisPos / tickSize);

                if (lastTick != thisTick)
                {
                    lastTick = thisTick;

                    float thisPosRounded = (float)(thisTick * tickSize);
                    if (thisPosRounded > min && thisPosRounded < max)
                    {
                        ticks.Add(new Tick(thisPosRounded, thisPosRounded, span));
                    }
                }
            }

            return ticks.ToArray();
        }

        [Obsolete]
        private double RoundNumberNear(double target)
        {
            target = Math.Abs(target);
            int lastDivision = 2;
            double round = 1000000000000;
            while (round > 0.00000000001)
            {
                if (round <= target) return round;
                round /= lastDivision;
                if (lastDivision == 2) lastDivision = 5;
                else lastDivision = 2;
            }
            return 0;
        }
    }

    public enum Edge
    {
        Left, Right, Top, Bottom
    }

    public static class EdgeExtensions
    {
        public static bool IsHorizontal(this Edge edge)
        {
            return edge == Edge.Top || edge == Edge.Bottom;
        }

        public static bool IsVertical(this Edge edge)
        {
            return edge == Edge.Left || edge == Edge.Right;
        }
    }


    public class Tick
    {
        public float PosUnit { get; set; }
        public float PosPixel { get; set; }
        public float SpanUnit { get; set; }

        public Tick(float posUnit, float posPixel, float spanUnit)
        {
            PosUnit = posUnit;
            PosPixel = posPixel;
            SpanUnit = spanUnit;
        }


        public string Label
        {
            get
            {
                if (SpanUnit < .01) return string.Format("{0:0.0000}", PosUnit);
                if (SpanUnit < .1) return string.Format("{0:0.000}", PosUnit);
                if (SpanUnit < 1) return string.Format("{0:0.00}", PosUnit);
                if (SpanUnit < 10) return string.Format("{0:0.0}", PosUnit);
                return string.Format("{0:0}", PosUnit);
            }
        }
    }
}
