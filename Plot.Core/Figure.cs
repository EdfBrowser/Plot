using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core
{
    /*
     * Figure:
     * Frame: 
     * Graph:
     * Axis:
     * 
     */
    public class Figure
    {

        private Bitmap m_frameBmp;
        private Graphics m_gfxFrame;


        private Bitmap m_graphBmp;
        private Graphics m_gfxGraph;


        private const string m_font = "Arial";
        private readonly Font m_fontTicks = new Font(m_font, 9, FontStyle.Regular);
        private readonly Font m_fontTitle = new Font(m_font, 20, FontStyle.Bold);
        private readonly Font m_fontAxis = new Font(m_font, 12, FontStyle.Bold);

        private readonly StringFormat m_sfCenter = new StringFormat()
        {
            Alignment = StringAlignment.Center,
        };
        private readonly StringFormat m_sfRight = new StringFormat()
        {
            Alignment = StringAlignment.Far,
        };

        private string m_labelTitle = "";
        private string m_labelX = "";
        private string m_labelY = "";

        private long m_pointCount = 0;

        private readonly System.Diagnostics.Stopwatch m_stopwatch;


        public Figure(int width, int height)
        {
            m_stopwatch = new System.Diagnostics.Stopwatch();

            StyleUI();

            Resize(width, height);
        }


        public Axis XAxis { get; set; } = new Axis(-10, 10, 100, false);
        public Axis YAxis { get; set; } = new Axis(-10, 10, 100, true);


        public Color FrameBgColor { get; set; } = Color.White;
        public Color GraphBgColor { get; set; } = Color.Gray;
        public Color AxisColor { get; set; } = Color.Black;
        public Color GridLineColor { get; set; } = Color.LightGray;

        public string LabelY
        {
            get => m_labelY; set
            {
                m_labelY = value;
                FrameReDraw();
            }
        }

        public string LabelX
        {
            get => m_labelX; set
            {
                m_labelX = value;
                FrameReDraw();
            }
        }

        public string LabelTitle
        {
            get => m_labelTitle; set
            {
                m_labelTitle = value;
                FrameReDraw();
            }
        }

        public int PadLeft { get; set; } = 50;
        public int PadRight { get; set; } = 50;
        public int PadTop { get; set; } = 47;
        public int PadBottom { get; set; } = 47;

        public int YAxis_Pixel => PadTop + m_graphBmp.Height;
        public int XAxis_Pixel => PadLeft + m_graphBmp.Width;
        public bool ShowGrid { get; set; } = true;

        public DataGen Gen { get; } = new DataGen();

        public string BenchmarkMessage
        {
            get
            {
                double ms = m_stopwatch.ElapsedTicks * 1000.0 / System.Diagnostics.Stopwatch.Frequency;
                double hz = 1.0 / ms * 1000.0;
                string msg = "";
                // argb
                double imageSizeMB = m_frameBmp.Width * m_frameBmp.Height * 4.0 / 1024 / 1024;
                msg += string.Format("{0} x {1} ({2:0.00} MB) ", m_frameBmp.Width, m_frameBmp.Height, imageSizeMB);
                msg += string.Format("with {0:n0} data points rendered in ", m_pointCount);
                msg += string.Format("{0:0.00 ms} ({1:0.00} Hz)", ms, hz);
                return msg;
            }
        }

        public void Resize(int width, int height)
        {
            if (width - PadLeft - PadRight < 1) width = PadLeft + PadRight + 1;
            if (height - PadTop - PadBottom < 1) height = PadTop + PadBottom + 1;

            m_frameBmp?.Dispose();
            m_graphBmp?.Dispose();

            m_gfxFrame?.Dispose();
            m_gfxGraph?.Dispose();

            m_frameBmp = new Bitmap(width, height);
            m_gfxFrame = Graphics.FromImage(m_frameBmp);

            //FramePadding(null, null, null, null);

            // now resize the graph
            m_graphBmp = new Bitmap(m_frameBmp.Width - PadLeft - PadRight, m_frameBmp.Height - PadTop - PadBottom);
            m_gfxGraph = Graphics.FromImage(m_graphBmp);

            // set the smoothing and text rendering hints
            m_gfxFrame.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxFrame.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // now resize the axis to the new dimensions
            XAxis.Resize(m_graphBmp.Width);
            YAxis.Resize(m_graphBmp.Height);
        }

        public void GraphClear()
        {
            m_gfxGraph.DrawImage(m_frameBmp, new Point(-PadLeft, -PadTop));
            //m_gfxGraph.Clear(GraphBgColor);
            m_pointCount = 0;
        }

        public void FrameReDraw()
        {
            m_gfxFrame.Clear(FrameBgColor);

            // prepare something useful for drawing
            using (SolidBrush axisBrush = new SolidBrush(AxisColor))
            using (SolidBrush graphBgBrush = new SolidBrush(GraphBgColor))
            using (Pen penGrid = new Pen(GridLineColor))
            using (Pen penAxis = new Pen(axisBrush))
            {
                // draw the rectangle and ticks and labels
                DrawRectangle(penAxis, graphBgBrush);
                DrawMajorTicks(penAxis, axisBrush);
                DrawMinorTicks(penAxis, axisBrush, penGrid);
                DrawTitle(axisBrush);

            }
        }



        private void DrawRectangle(Pen penAxis, SolidBrush graphBgBrush)
        {
            Rectangle graphRect = new Rectangle(PadLeft - 1, PadTop - 1, m_graphBmp.Width + 1, m_graphBmp.Height + 1);
            m_gfxFrame.DrawRectangle(penAxis, graphRect);
            m_gfxFrame.FillRectangle(graphBgBrush, graphRect);
        }

        private void DrawTitle(SolidBrush axisBrush)
        {
            Point title = new Point(m_frameBmp.Width / 2, 2);
            m_gfxFrame.DrawString(LabelTitle, m_fontTitle, axisBrush, title, m_sfCenter);
            Point xLabel = new Point(m_frameBmp.Width / 2, YAxis_Pixel + 14);
            m_gfxFrame.DrawString(LabelX, m_fontAxis, axisBrush, xLabel, m_sfCenter);

            m_gfxFrame.TranslateTransform(m_gfxFrame.VisibleClipBounds.Width, 0);
            m_gfxFrame.RotateTransform(-90);

            Point yLabel = new Point(-(m_graphBmp.Height / 2 + PadTop), -m_frameBmp.Width + 2);
            m_gfxFrame.DrawString(LabelY, m_fontAxis, axisBrush, yLabel, m_sfCenter);

            m_gfxFrame.ResetTransform();
        }



        private void DrawMinorTicks(Pen penAxis, SolidBrush axisBrush, Pen penGrid)
        {
            int tick_size_minor = 2;

            // X-axis
            foreach (Tick tick in XAxis.TicksMinor)
            {
                if (ShowGrid)
                {
                    Point start1 = new Point(PadLeft + tick.PosPixel, PadTop + 1);
                    Point end1 = new Point(PadLeft + tick.PosPixel, YAxis_Pixel - 1);
                    m_gfxFrame.DrawLine(penGrid, start1, end1);
                }
                Point start = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + 1);
                Point end = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + tick_size_minor);
                Point end2 = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + tick_size_minor + 1);
                m_gfxFrame.DrawLine(penAxis, start, end);
                //m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end2, m_sfCenter);
            }

            // Y-axis
            foreach (Tick tick in YAxis.TicksMinor)
            {
                if (ShowGrid)
                {
                    Point start1 = new Point(PadLeft + 1, PadTop + tick.PosPixel);
                    Point end1 = new Point(XAxis_Pixel - 1, PadTop + tick.PosPixel);
                    m_gfxFrame.DrawLine(penGrid, start1, end1);
                }
                Point start = new Point(PadLeft - 1, PadTop + tick.PosPixel);
                Point end = new Point(PadLeft - tick_size_minor, PadTop + tick.PosPixel);
                m_gfxFrame.DrawLine(penAxis, start, end);
                //m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end, m_sfCenter);
            }
        }

        private void DrawMajorTicks(Pen penAxis, SolidBrush axisBrush)
        {
            int tick_size_major = 5;

            // X-axis
            foreach (Tick tick in XAxis.TicksMajor)
            {
                Point start = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + 1);
                Point end = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + tick_size_major);
                Point end1 = new Point(PadLeft + tick.PosPixel, YAxis_Pixel + tick_size_major + 1);
                m_gfxFrame.DrawLine(penAxis, start, end);
                m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end1, m_sfCenter);
            }

            // Y-axis
            foreach (Tick tick in YAxis.TicksMajor)
            {
                Point start = new Point(PadLeft - 1, PadTop + tick.PosPixel);
                Point end = new Point(PadLeft - tick_size_major, PadTop + tick.PosPixel);
                Point end1 = new Point(PadLeft - tick_size_major - 1, PadTop + tick.PosPixel - 7);
                m_gfxFrame.DrawLine(penAxis, start, end);
                m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end1, m_sfRight);
            }
        }

        public void AxisSet(double? x1, double? x2, double? y1, double? y2)
        {
            if (x1 != null || x2 != null) XAxis.AxisLimits(x1, x2);
            if (y1 != null || y2 != null) YAxis.AxisLimits(y1, y2);

            FrameReDraw();
        }

        public void AxisAuto(double[] xs, double[] ys, double? zoomX, double? zoomY)
        {
            if (xs != null && xs.Length > 0) AxisSet(xs.Min(), xs.Max(), null, null);
            if (ys != null && ys.Length > 0) AxisSet(null, null, ys.Min(), ys.Max());

            Zoom(zoomX, zoomY);
        }

        public void Zoom(double? xFrac, double? yFrac)
        {
            if (xFrac != null) XAxis.Zoom(xFrac.Value);
            if (yFrac != null) YAxis.Zoom(yFrac.Value);

            FrameReDraw();
        }

        public void Pan(double? dx, double? dy)
        {
            if (dx != null) XAxis.Pan(dx.Value);
            if (dy != null) YAxis.Pan(dy.Value);

            FrameReDraw();
        }

        public void BenchmarkThis(bool enable = true)
        {
            if (enable)
            {
                m_stopwatch.Restart();
            }
            else
            {
                m_pointCount = 0;

                m_stopwatch.Stop();
                m_stopwatch.Reset();
            }
        }

        public void Render(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(m_frameBmp, new Rectangle(0, 0, m_frameBmp.Width, m_frameBmp.Height));
                g.DrawImage(m_graphBmp, new Rectangle(PadLeft, PadTop, m_graphBmp.Width, m_graphBmp.Height));

                if (m_stopwatch.ElapsedTicks > 0)
                {
                    using (Font font = new Font(m_font, 8, FontStyle.Regular))
                    using (SolidBrush stampBrush = new SolidBrush(AxisColor))
                    {
                        Point stamp = new Point(m_frameBmp.Width - PadRight - 2, m_frameBmp.Height - PadBottom - 14);
                        g.DrawString(BenchmarkMessage, font, stampBrush, stamp, m_sfRight);
                    }
                }
            }
        }


        private void StyleUI()
        {
            FrameBgColor = Color.White;
            GraphBgColor = Color.FromArgb(255, 235, 235, 235);
            AxisColor = Color.Black;
            GridLineColor = Color.LightGray;
        }










        private Point[] PointsFromArrays(double[] Xs, double[] Ys)
        {
            int pointCount = Math.Min(Xs.Length, Ys.Length);
            Point[] points = new Point[pointCount];
            for (int i = 0; i < pointCount; i++)
            {
                points[i] = new Point(XAxis.GetPixel(Xs[i]), YAxis.GetPixel(Ys[i]));
            }
            return points;
        }


        public void PlotLines(double[] Xs, double[] Ys, float lineWidth = 1, Color? lineColor = null)
        {
            if (lineColor == null) lineColor = Color.Red;
            Point[] points = PointsFromArrays(Xs, Ys);

            using (SolidBrush brush = new SolidBrush(lineColor.Value))
            using (Pen penLine = new Pen(brush, lineWidth))
            {
                // adjust the pen caps and joins to make it as smooth as possible
                penLine.StartCap = System.Drawing.Drawing2D.LineCap.Round;
                penLine.EndCap = System.Drawing.Drawing2D.LineCap.Round;
                penLine.LineJoin = System.Drawing.Drawing2D.LineJoin.Round;


                m_gfxGraph.DrawLines(penLine, points);

                m_pointCount += points.Length;
            }
        }

        public void PlotSignal(double[] values, double pointSpacing = 1, double offsetX = 0, double offsetY = 0, float lineWidth = 1, Color? lineColor = null)
        {
            if (lineColor == null) lineColor = Color.Red;
            if (values == null || values.Length == 0) return;

            int pointCount = values.Length;
            double lastPointX = pointCount * pointSpacing + offsetX;
            int dataMinPx = (int)((offsetX - XAxis.Min) / XAxis.UnitsPerPx); // pixel
            int dataMaxPx = (int)((lastPointX - XAxis.Min) / XAxis.UnitsPerPx); // pixel

            double binningUnitsPerPx = XAxis.UnitsPerPx / pointSpacing;
            double dataPointsPerPx = XAxis.UnitsPerPx / pointSpacing;

            List<Point> points = new List<Point>();
            List<double> ys = new List<double>();

            for (int i = 0; i < values.Length; i++) ys.Add(values[i]); // copy entire array into list (SLOW!!!)


            if (dataPointsPerPx < 1)
            {
                // LOW DENSITY
                // TODO: 到底是offset  - min 还是 min - offset
                int iLeft = (int)(((offsetX - XAxis.Min) / XAxis.UnitsPerPx) * dataPointsPerPx);
                int iRight = iLeft + (int)(dataPointsPerPx * m_graphBmp.Width);
                for (int i = Math.Max(0, iLeft - 2); i < Math.Min(iRight + 3, ys.Count - 1); i++)
                {
                    int xPx = XAxis.GetPixel(offsetX + i * pointSpacing);
                    int yPx = YAxis.GetPixel(ys[i]);
                    points.Add(new Point(xPx, yPx));
                }
            }
            else
            {
                // HIGH DENSITY
                for (int xPixel = Math.Max(0, dataMinPx); xPixel < Math.Min(m_graphBmp.Width, dataMaxPx); xPixel++)
                {
                    int iLeft = (int)(binningUnitsPerPx * (xPixel - dataMinPx));
                    int iRight = (int)(iLeft + binningUnitsPerPx);
                    iLeft = Math.Max(iLeft, 0);
                    iRight = Math.Min(ys.Count - 1, iRight);
                    iRight = Math.Max(iRight, 0);
                    if (iLeft == iRight) continue;
                    double yPxMin = ys.GetRange(iLeft, iRight - iLeft).Min() + offsetY;
                    double yPxMax = ys.GetRange(iLeft, iRight - iLeft).Max() + offsetY;

                    points.Add(new Point(xPixel, YAxis.GetPixel(yPxMin)));
                    points.Add(new Point(xPixel, YAxis.GetPixel(yPxMax)));
                }
            }


            if (points.Count < 2) return;
            float markerSize = 3;

            using (SolidBrush brush = new SolidBrush(lineColor.Value))
            using (Pen penLine = new Pen(brush, lineWidth))
            {
                System.Drawing.Drawing2D.SmoothingMode originalSmoothingMode = m_gfxGraph.SmoothingMode;
                m_gfxGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.None; // no antialiasing

                m_gfxGraph.DrawLines(penLine, points.ToArray());
                if (dataPointsPerPx < .5)
                {
                    foreach (Point pt in points)
                    {
                        m_gfxGraph.FillEllipse(brush, pt.X - markerSize / 2, pt.Y - markerSize / 2, markerSize, markerSize);
                    }
                }

                m_gfxGraph.SmoothingMode = originalSmoothingMode;
                m_pointCount += values.Length;
            }
        }

        public void PlotScatter(double[] Xs, double[] Ys, float markerSize = 3, Color? markerColor = null)
        {
            if (markerColor == null) markerColor = Color.Red;
            Point[] points = PointsFromArrays(Xs, Ys);
            using (SolidBrush brush = new SolidBrush(markerColor.Value))
            {
                for (int i = 0; i < points.Length; i++)
                {
                    m_gfxGraph.FillEllipse(brush, points[i].X - markerSize / 2,
                        points[i].Y - markerSize / 2,
                        markerSize, markerSize);
                }
                m_pointCount += points.Length;
            }
        }



        /* MOUSE STUFF */

        MouseAxes m_mousePan = null;
        MouseAxes m_mouseZoom = null;

        public void MousePanStart(int xPx, int yPx) { m_mousePan = new MouseAxes(XAxis, YAxis, xPx, yPx); }
        public void MousePanEnd() { m_mousePan = null; }
        public void MouseZoomStart(int xPx, int yPx) { m_mouseZoom = new MouseAxes(XAxis, YAxis, xPx, yPx); }
        public void MouseZoomEnd() { m_mouseZoom = null; }
        public bool MouseIsDragging() { return (m_mousePan != null || m_mouseZoom != null); }

        public void MouseMove(int xPx, int yPx)
        {
            if (m_mousePan != null)
            {
                m_mousePan.Pan(xPx, yPx);
                AxisSet(m_mousePan.X1, m_mousePan.X2, m_mousePan.Y1, m_mousePan.Y2);
            }
            else if (m_mouseZoom != null)
            {
                m_mouseZoom.Zoom(xPx, yPx);
                AxisSet(m_mouseZoom.X1, m_mouseZoom.X2, m_mouseZoom.Y1, m_mouseZoom.Y2);
            }
        }
    }
}
