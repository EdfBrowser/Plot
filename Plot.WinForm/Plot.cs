using Plot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class Plot : UserControl
    {
        private struct SignalData
        {
            public double[] m_values;
            public double m_sampleRate;
            public double m_xSpacing;
            public double m_offsetX;
            public double m_offsetY;
            public float m_lineWidth;
            public Color m_lineColor;
            public string m_label;

            public SignalData(double[] values, double sampleRate, double offsetX = 0,
                double offsetY = 0, float lineWidth = 1f, Color? lineColor = null, string label = null)
            {
                m_values = values;
                m_sampleRate = sampleRate;
                m_xSpacing = 1.0 / sampleRate;
                m_offsetX = offsetX;
                m_offsetY = offsetY;
                m_lineWidth = lineWidth;
                m_lineColor = lineColor ?? Color.Red;
                m_label = label;
            }
        }

        private struct XYData
        {
            public double[] m_xs;
            public double[] m_ys;
            public float m_lineWidth;
            public Color m_lineColor;
            public float m_markerSize;
            public Color m_markerColor;
            public string m_label;

            public XYData(double[] xs, double[] ys, float lineWidth = 1f, Color? lineColor = null,
                float markerSize = 3f, Color? markerColor = null, string label = null)
            {
                m_xs = xs;
                m_ys = ys;
                m_lineWidth = lineWidth;
                m_lineColor = lineColor ?? Color.Red;
                m_markerSize = markerSize;
                m_markerColor = markerColor ?? Color.Red;
                m_label = label;
            }
        }

        private struct AxisLine
        {
            public double m_value;
            public float m_lineWidth;
            public Color m_lineColor;
            public string m_label;

            public AxisLine(double value, float lineWidth = 1f, Color? lineColor = null, string label = null)
            {
                m_value = value;
                m_lineWidth = lineWidth;
                m_lineColor = lineColor ?? Color.Black;
                m_label = label;
            }
        }

        private List<SignalData> m_signalDataList = new List<SignalData>();
        private List<XYData> m_xyDataList = new List<XYData>();
        private List<AxisLine> m_hLines = new List<AxisLine>();
        private List<AxisLine> m_vLines = new List<AxisLine>();


        private Bitmap m_lastBmp;
        private readonly Queue<Bitmap> m_bitmapQueue = new Queue<Bitmap>();

        private readonly Figure m_figure;

        private bool m_showBenchMark = false;
        private bool m_busy = false;

        public Plot()
        {
            InitializeComponent();

            m_figure = new Figure(10, 10);

            Render(true);

            pictureBox1.SizeChanged += pictureBox1_SizeChanged;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
        }


        public Figure Figure => m_figure;


        public void Hline(double Ypos, float lineWidth = 1f, Color? lineColor = null)
        {
            m_hLines.Add(new AxisLine(Ypos, lineWidth, lineColor));
            Render();
        }

        public void Vline(double Xpos, float lineWidth = 1f, Color? lineColor = null)
        {
            m_vLines.Add(new AxisLine(Xpos, lineWidth, lineColor));
            Render();
        }


        public void PlotXY(double[] Xs, double[] Ys, float lineWidth = 1f, Color? lineColor = null,
            float markerSize = 5, Color? markerColor = null, string label = null, bool render = true)
        {
            m_xyDataList.Add(
                new XYData(Xs, Ys, lineWidth, lineColor, markerSize, markerColor, label));
            m_figure.GraphClear();
            if (render)
                Render();
        }

        public void PlotSignal(double[] values, double sampleRate, double offsetX = 0,
            double offsetY = 0, float lineWidth = 1f, Color? lineColor = null, string label = null, bool render = true)
        {
            m_signalDataList.Add(
                new SignalData(values, sampleRate, offsetX, offsetY, lineWidth, lineColor, label));
            m_figure.GraphClear();
            if (render)
                Render();
        }

        public void Clear(bool renderAfterClearing = false, bool clearXYdata = true, bool clearSignals = true, bool clearHlines = true, bool clearVlines = true)
        {
            if (clearXYdata) m_xyDataList.Clear();
            if (clearSignals) m_signalDataList.Clear();
            if (clearHlines) m_hLines.Clear();
            if (clearVlines) m_vLines.Clear();
            if (renderAfterClearing) Render();
        }

        public void SaveDialog(string filename = "output.png")
        {
            SaveFileDialog savefile = new SaveFileDialog();
            savefile.FileName = filename;
            savefile.Filter = "PNG Files (*.png)|*.png|All files (*.*)|*.*";
            if (savefile.ShowDialog() == DialogResult.OK) filename = savefile.FileName;
            else return;

            string basename = System.IO.Path.GetFileNameWithoutExtension(filename);
            string extension = System.IO.Path.GetExtension(filename).ToLower();
            string fullPath = System.IO.Path.GetFullPath(filename);

            switch (extension)
            {
                case ".png":
                    pictureBox1.Image.Save(filename, System.Drawing.Imaging.ImageFormat.Png);
                    break;
                case ".jpg":
                    pictureBox1.Image.Save(filename, System.Drawing.Imaging.ImageFormat.Jpeg);
                    break;
                case ".bmp":
                    pictureBox1.Image.Save(filename);
                    break;
                default:
                    //TODO: messagebox error
                    break;
            }
        }

        public void AxisAuto()
        {
            double x1 = 0, x2 = 0, y1 = 0, y2 = 0;

            foreach (XYData xyData in m_xyDataList)
            {
                if (x1 == x2)
                {
                    // this is the first data we are scaling to, so just copy its bounds
                    x1 = xyData.m_xs.Min();
                    x2 = xyData.m_xs.Max();
                    y1 = xyData.m_ys.Min();
                    y2 = xyData.m_ys.Max();
                }
                else
                {
                    // we've seen some data before, so only take it if it expands the axes
                    x1 = Math.Min(x1, xyData.m_xs.Min());
                    x2 = Math.Max(x2, xyData.m_xs.Max());
                    y1 = Math.Min(y1, xyData.m_ys.Min());
                    y2 = Math.Max(y2, xyData.m_ys.Max());
                }
            }
            foreach (SignalData signalData in m_signalDataList)
            {
                if (x1 == x2)
                {
                    // this is the first data we are scaling to, so just copy its bounds
                    x1 = signalData.m_offsetX;
                    x2 = signalData.m_offsetX + signalData.m_values.Length * signalData.m_xSpacing;
                    y1 = signalData.m_values.Min() + signalData.m_offsetY;
                    y2 = signalData.m_values.Max() + signalData.m_offsetY;
                }
                else
                {
                    // we've seen some data before, so only take it if it expands the axes
                    x1 = Math.Min(x1, signalData.m_offsetX);
                    x2 = Math.Max(x2, signalData.m_offsetX + signalData.m_values.Length * signalData.m_xSpacing);
                    y1 = Math.Min(y1, signalData.m_values.Min() + signalData.m_offsetY);
                    y2 = Math.Max(y2, signalData.m_values.Max() + signalData.m_offsetY);
                }
            }

            m_figure.AxisSet(x1, x2, y1, y2);
            m_figure.Zoom(null, .9);
            Render(true);
        }

        public void Render(bool redraw = false, bool axisAuto = false)
        {
            m_figure.BenchmarkThis();
            if (redraw) m_figure.FrameReDraw();
            else m_figure.GraphClear();

            foreach (SignalData signalData in m_signalDataList)
            {
                m_figure.PlotSignal(signalData.m_values, signalData.m_xSpacing,
                    signalData.m_offsetX, signalData.m_offsetY, signalData.m_lineWidth, signalData.m_lineColor);
            }

            // plot XY points
            foreach (XYData xyData in m_xyDataList)
            {
                if (xyData.m_lineWidth > 0 && xyData.m_xs.Length > 1)
                    m_figure.PlotLines(xyData.m_xs, xyData.m_ys, xyData.m_lineWidth, xyData.m_lineColor);
                m_figure.PlotScatter(xyData.m_xs, xyData.m_ys, xyData.m_markerSize, xyData.m_markerColor);
            }

            // plot axis lines
            foreach (AxisLine axisLine in m_hLines)
            {
                m_figure.PlotLines(
                    new double[] { m_figure.XAxis.Min, m_figure.XAxis.Max },
                    new double[] { axisLine.m_value, axisLine.m_value },
                    axisLine.m_lineWidth,
                    axisLine.m_lineColor
                    );
            }
            foreach (AxisLine axisLine in m_vLines)
            {
                m_figure.PlotLines(
                    new double[] { axisLine.m_value, axisLine.m_value },
                    new double[] { m_figure.YAxis.Min, m_figure.YAxis.Max },
                    axisLine.m_lineWidth,
                    axisLine.m_lineColor
                    );
            }

            if (m_lastBmp != null)
                m_bitmapQueue.Enqueue(m_lastBmp);

            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            m_figure.Render(bitmap);
            m_lastBmp = bitmap;


            pictureBox1.Image = GetLastBitmap();
        }


        private Bitmap GetLastBitmap()
        {
            if (m_bitmapQueue.Count > 3)
            {
                m_bitmapQueue.Dequeue()?.Dispose();
            }

            return m_lastBmp;
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Width < 10 || pictureBox1.Height < 10)
                return;
            m_figure.Resize(pictureBox1.Width, pictureBox1.Height);
            Render(true, true);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double mag = 1.2;
            if (e.Delta > 0) m_figure.Zoom(mag, mag);
            else m_figure.Zoom(1.0 / mag, 1.0 / mag);
            Render(true);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) m_figure.MousePanStart(e.X, e.Y); // left-click-drag pans
            else if (e.Button == MouseButtons.Right) m_figure.MouseZoomStart(e.X, e.Y); // right-click-drag zooms
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left) m_figure.MousePanEnd();
            else if (e.Button == MouseButtons.Right) m_figure.MouseZoomEnd();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle) AxisAuto();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            m_showBenchMark = !m_showBenchMark; // double-click graph to display benchmark stats
            Render();
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_figure.MouseIsDragging() && m_busy == false)
            {
                m_figure.MouseMove(e.X, e.Y);
                m_busy = true;
                Render(true);
                Application.DoEvents();
                m_busy = false;
            }
        }
    }
}
