using System;
using System.Drawing;

namespace Plot.Chart
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

        private Bitmap m_frame;
        private Graphics m_gfxFrame;


        private Bitmap m_graph;
        private Graphics m_gfxGraph;


        private const string m_font = "Arial";
        private Font m_fontTicks = new Font(m_font, 9, FontStyle.Regular);
        private Font m_fontTitle = new Font(m_font, 20, FontStyle.Bold);
        private Font m_fontAxis = new Font(m_font, 12, FontStyle.Bold);

        private Point m_graphOrigin;

        private StringFormat m_sfCenter = new StringFormat()
        {
            Alignment = StringAlignment.Center,
        };
        private StringFormat m_sfRight = new StringFormat()
        {
            Alignment = StringAlignment.Far,
        };

        private readonly System.Diagnostics.Stopwatch m_stopwatch;


        public Figure(int width, int height)
        {
            m_stopwatch = new System.Diagnostics.Stopwatch();

            StyleUI();

            Resize(width, height);

            FrameReDraw();
        }


        public Axis XAxis { get; set; } = new Axis(-10, 10, 100, false);
        public Axis YAxis { get; set; } = new Axis(-10, 10, 100, false);


        public Color FrameBgColor { get; set; } = Color.White;
        public Color GraphBgColor { get; set; } = Color.Gray;
        public Color AxisColor { get; set; } = Color.Black;
        public Color GridLineColor { get; set; } = Color.LightGray;

        public string LabelY { get; set; } = "";
        public string LabelX { get; set; } = "";
        public string LabelTitle { get; set; } = "";

        public int PadLeft { get; set; } = 50;
        public int PadRight { get; set; } = 50;
        public int PadTop { get; set; } = 47;
        public int PadBottom { get; set; } = 47;

        public int XAxis_Pixel => PadTop + m_graph.Height;
        public int YAxis_Pixel => PadLeft + m_graph.Width;
        public bool ShowGrid { get; set; } = true;

        public void Resize(int width, int height)
        {
            if (width - PadLeft - PadRight < 1) width = PadLeft + PadRight + 1;
            if (height - PadTop - PadBottom < 1) height = PadTop + PadBottom + 1;

            m_frame?.Dispose();
            m_graph?.Dispose();

            m_gfxFrame?.Dispose();
            m_gfxGraph?.Dispose();

            m_frame = new Bitmap(width, height);
            m_gfxFrame = Graphics.FromImage(m_frame);

            //FramePadding(null, null, null, null);

            // now resize the graph
            m_graph = new Bitmap(width - PadLeft - PadRight, height - PadTop - PadBottom);
            m_gfxGraph = Graphics.FromImage(m_graph);

            // set the smoothing and text rendering hints
            m_gfxFrame.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxGraph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            m_gfxFrame.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;

            // reset the origin of the graph
            m_graphOrigin = new Point(PadLeft, PadTop);

            // now resize the axis to the new dimensions
            XAxis.Resize(m_graph.Width);
            YAxis.Resize(m_graph.Height);
        }

        public void GraphClear()
        {
            m_gfxGraph.DrawImage(m_frame, new Point(-PadLeft, -PadTop));
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


            }

            //GraphClear();
        }

        public void Render(Bitmap bmp)
        {
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.DrawImage(m_frame, new Rectangle(0, 0, m_frame.Width, m_frame.Height));
            }
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
                    Point end1 = new Point(PadLeft + tick.PosPixel, XAxis_Pixel - 1);
                    m_gfxFrame.DrawLine(penGrid, start1, end1);
                }
                Point start = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + 1);
                Point end = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + tick_size_minor);
                Point end2 = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + tick_size_minor + 1);
                m_gfxFrame.DrawLine(penAxis, start, end);
                //m_gfxFrame.DrawString(tick.Label, m_fontTicks, axisBrush, end2, m_sfCenter);
            }

            // Y-axis
            foreach (Tick tick in YAxis.TicksMinor)
            {
                if (ShowGrid)
                {
                    Point start1 = new Point(PadLeft + 1, PadTop + tick.PosPixel);
                    Point end1 = new Point(YAxis_Pixel - 1, PadTop + tick.PosPixel);
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
                Point start = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + 1);
                Point end = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + tick_size_major);
                Point end1 = new Point(PadLeft + tick.PosPixel, XAxis_Pixel + tick_size_major + 1);
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

        private void DrawRectangle(Pen penAxis, SolidBrush graphBgBrush)
        {
            Rectangle graphRect = new Rectangle(m_graphOrigin, m_graph.Size);
            m_gfxFrame.DrawRectangle(penAxis, graphRect);
            m_gfxFrame.FillRectangle(graphBgBrush, graphRect);
        }

        private void StyleUI()
        {
            FrameBgColor = Color.White;
            GraphBgColor = Color.FromArgb(255, 235, 235, 235);
            AxisColor = Color.Black;
            GridLineColor = Color.LightGray;
        }
    }
}
