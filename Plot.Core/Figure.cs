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
            //XAxis.Dims.Resize(bmp.Width);
            //XAxis.Dims.SetPadding(PadLeft, PadRight);
            //foreach(var axis in YAxes)
            //{
            //    axis.Dims.Resize(bmp.Height);
            //}




            SizeF figureSize = new SizeF(bmp.Width, bmp.Height);
            SizeF dataSize = new SizeF(bmp.Width - PadLeft - PadRight, bmp.Height - PadTop - PadBottom);
            PointF offset = new PointF(PadLeft, PadTop);
            PlotDimensions dims = new PlotDimensions(figureSize, dataSize, offset);
            RenderClear(bmp, dims);
            RenderBeforePlot(bmp, dims);
            PlotRender(bmp, dims);
            PlotAfterRender(bmp, dims);
        }


        private void RenderClear(Bitmap bmp, PlotDimensions dims)
        {
            Color figureColor = Color.LightGray;
            // clear and set the background of figure
            using (var gfx = GDI.Graphics(bmp))
            {
                gfx.Clear(figureColor);
            }
        }


        private void RenderBeforePlot(Bitmap bmp, PlotDimensions dims)
        {
            Color dataAreaColor = Color.White;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor, 1f))
            using (var gfx = GDI.Graphics(bmp))
            {
                var dataRect = new RectangleF(
                      x: dims.DataOffsetX,
                      y: dims.DataOffsetY,
                      width: dims.DataWidth,
                      height: dims.DataHeight);

                gfx.FillRectangle(brush, dataRect);
            }

            // Draw Axes
            // X-axis
            // Draw X-axis line
            using (var pen = GDI.Pen(AxisColor, 1f, 1f))
            using (var gfx = GDI.Graphics(bmp))
            {
                float left = dims.DataOffsetX;
                float right = left + dims.DataWidth;
                float top = dims.DataOffsetY;
                float bottom = dims.DataOffsetY + dims.DataHeight;

                gfx.DrawLine(pen, left, bottom, right, bottom);
                gfx.DrawLine(pen, left, top, right, top);
                gfx.DrawLine(pen, left, top, left, bottom);
                gfx.DrawLine(pen, right, top, right, bottom);
            }


            // Draw X-Axis Ticks
            bool rulerMode = false;
            bool ticksExtendOutward = true;

            bool majorTickVisible = true;
            float majorTickWidth = 1f;
            float majorTickLength = 5f;
            Color majorTickColor = Color.Black;

            bool minorTickVisible = true;
            float minorTickLength = 2f;
            float minorTickWidth = 1f;
            Color minorTickColor = Color.Black;

            using (var gfx = GDI.Graphics(bmp))
            {
                // Major ticks
                if (majorTickVisible)
                {
                    float tickLength = majorTickLength;
                    if (rulerMode)
                        tickLength *= 4;
                    tickLength = ticksExtendOutward ? tickLength : -tickLength;
                    DrawTicks(dims, gfx, XAxis.TicksMajor, tickLength, majorTickColor, true, majorTickWidth);
                }
                

                // Minor ticks
                if (minorTickVisible)
                {
                    float tickLength = ticksExtendOutward ? minorTickLength : -minorTickLength;
                    DrawTicks(dims, gfx, XAxis.TicksMinor, tickLength, minorTickColor, true, minorTickWidth);
                }
            }
        }


        private void PlotAfterRender(Bitmap bmp, PlotDimensions dims)
        {
        }

        private void PlotRender(Bitmap bmp, PlotDimensions dims)
        {
        }

        private static void DrawTicks(PlotDimensions dims, Graphics gfx, Tick[] ticks, float tickLength, Color color, bool isHorizontal, float tickWidth)
        {
            if (ticks == null || ticks.Length == 0) return;

            if (isHorizontal)
            {
                float y = dims.DataOffsetY + dims.DataHeight;
                float tickDelta = tickLength;

                var xs = ticks.Select(t => dims.GetPixelX(t.PosPixel));
                using (var pen = GDI.Pen(color, tickWidth, 1f))
                {
                    foreach (float x in xs)
                    {
                        gfx.DrawLine(pen, x, y, x, y + tickDelta);
                    }
                }
            }
            else
            {
                float x = dims.DataOffsetX;
                float tickDelta = -tickLength;

                var ys = ticks.Select(t => dims.GetPixelY(t.PosPixel));
                using (var pen = GDI.Pen(color, tickWidth, 1f))
                {
                    foreach (float y in ys)
                    {
                        gfx.DrawLine(pen, x, y, x + tickDelta, y);
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
