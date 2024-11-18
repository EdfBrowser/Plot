using System.Collections.Generic;
using System.Drawing;

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

        private List<BaseSeries> SeriesList { get; set; } = new List<BaseSeries>();


        public Figure()
        {
            m_stopwatch = new System.Diagnostics.Stopwatch();

            StyleUI();
        }


        public Axis XAxis { get; set; } = new Axis(-10, 10, 100, false, 0);
        // 
        public List<Axis> YAxes { get; set; } = new List<Axis>()
        {
          new Axis(-10, 10, 100, true, 0),
          new Axis(-10, 10, 100, true, 1),
        };


        public Color FrameBgColor { get; set; } = Color.White;
        public Color GraphBgColor { get; set; } = Color.Gray;
        public Color AxisColor { get; set; } = Color.Black;
        public Color GridLineColor { get; set; } = Color.LightGray;

        public string LabelY
        {
            get => m_labelY; set
            {
                m_labelY = value;
            }
        }

        public string LabelX
        {
            get => m_labelX; set
            {
                m_labelX = value;
            }
        }

        public string LabelTitle
        {
            get => m_labelTitle; set
            {
                m_labelTitle = value;
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


            // now resize the graph
            m_graphBmp = new Bitmap(m_frameBmp.Width - PadLeft - PadRight, m_frameBmp.Height - PadTop - PadBottom);
            m_gfxGraph = Graphics.FromImage(m_graphBmp);

            // set the smoothing and text rendering hints
            m_gfxFrame.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxFrame.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // now resize the axis to the new dimensions
            XAxis.Resize(m_graphBmp.Width);


            if (YAxes.Count > 0)
            {
                int pxSize = m_graphBmp.Height / YAxes.Count;
                foreach (var axis in YAxes)
                {
                    axis.Resize(pxSize);
                }
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
            }

            // Y-axis
            foreach (Axis axis in YAxes)
            {
                foreach (Tick tick in axis.TicksMinor)
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
                }
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
            foreach (Axis axis in YAxes)
            {
                foreach (Tick tick in axis.TicksMajor)
                {
                    Point start = new Point(PadLeft - 1, PadTop + tick.PosPixel);
                    Point end = new Point(PadLeft - tick_size_major, PadTop + tick.PosPixel);
                    Point end1 = new Point(PadLeft - tick_size_major - 1, PadTop + tick.PosPixel - 7);
                    m_gfxFrame.DrawLine(penAxis, start, end);
                    m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end1, m_sfRight);
                }
            }
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
            m_gfxGraph.DrawImage(m_frameBmp, new Point(-PadLeft, -PadTop));
            //m_gfxGraph.Clear(GraphBgColor);
            m_pointCount = 0;

            foreach (var series in SeriesList)
            {
                series.Render(m_graphBmp);
            }

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




        public DataStreamSeries AddDataStreamer(int xIndex, int yIndex, int length)
        {
            double[] data = new double[length];
            return AddDataStreamer(xIndex, yIndex, data);
        }

        public DataStreamSeries AddDataStreamer(int xIndex, int yIndex, double[] values)
        {
            DataStreamSeries dataStreamSeries = new DataStreamSeries(xIndex, yIndex, this, values);
            SeriesList.Add(dataStreamSeries);
            return dataStreamSeries;
        }


        public void ClearSeries()
        {
            SeriesList.Clear();
        }
    }
}
