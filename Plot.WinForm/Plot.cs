using Plot.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class Plot : UserControl
    {
        private Bitmap m_lastBmp;
        private readonly Queue<Bitmap> m_bitmapQueue = new Queue<Bitmap>();

        private readonly Figure m_figure;

        private readonly bool m_isDesignerMode = Process.GetCurrentProcess().ProcessName == "devenv";
        public Plot()
        {
            m_figure = new Figure(10, 10);

            if (DesignMode || m_isDesignerMode)
            {
                try
                {
                    Render(true, true);
                }
                catch (Exception ex)
                {
                    InitializeComponent();

                    pictureBox1.Visible = false;
                    richTextBox1.Visible = true;
                    richTextBox1.Dock = DockStyle.Fill;
                    richTextBox1.Text = "ERROR: ScottPlot failed to render in design mode.\n\n" +
                        "This may be due to incompatible System.Drawing.Common versions or a 32-bit/64-bit mismatch.\n\n" +
                        "Although rendering failed at design time, it may still function normally at runtime.\n\n" +
                        $"Exception details:\n{ex}";
                    return;
                }
            }

            InitializeComponent();

            richTextBox1.Visible = false;

            pictureBox1.SizeChanged += pictureBox1_SizeChanged;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
        }

        public Figure Figure => m_figure;

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            if (pictureBox1.Width < 10 || pictureBox1.Height < 10)
                return;
            m_figure.Resize(pictureBox1.Width, pictureBox1.Height);
            Render(true, true);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double mag = 1.1;
            if (e.Delta > 0) m_figure.Zoom(mag, mag);
            else m_figure.Zoom(1.0 / mag, 1.0 / mag);
            Render(true);
        }

        int m_lastX, m_lastY;
        bool m_pressed = false;
        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_lastX = 0; m_lastY = 0;
                m_pressed = false;
            }
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                m_lastX = e.X; m_lastY = e.Y;
                m_pressed = true;
            }
        }

        bool m_busy = false;
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (m_pressed && !m_busy)
            {
                m_busy = true;

                int dx = e.X - m_lastX;
                int dy = e.Y - m_lastY;

                // 判断用户点击的方向
                if (Math.Abs(dx) > Math.Abs(dy)) // 偏向 X 轴的移动
                {
                    m_lastX = e.X; // 更新最后的 X 坐标
                    m_figure.Pan(dx, null); // 在 X 轴上滚动
                }
                //else // 偏向 Y 轴的移动
                //{
                //    m_lastY = e.Y; // 更新最后的 Y 坐标
                //    m_figure.Pan(null, dy); // 在 Y 轴上滚动
                //}

                Render(true);

                m_busy = false;
            }
        }

        public void Render(bool redraw = false, bool axisAuto = false)
        {
            Figure.BenchmarkThis();
            if (redraw) m_figure.FrameReDraw();
            else m_figure.GraphClear();

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


        public void AddSignal(int second, int sample)
        {
            int pointCount = (int)(second * sample);
            double[] Ys = m_figure.Gen.RandomWalk(pointCount);
            m_figure.AxisSet(0, pointCount / sample, null, null);
            m_figure.AxisAuto(null, Ys, .9, .9);


            m_figure.PlotSignal(Ys, 1.0 / sample);
        }


        public void AddPlotLine()
        {
            int pointCount = 123;

            double[] m_xs = m_figure.Gen.Sequence(pointCount);
            double[] m_ys = m_figure.Gen.RandomWalk(pointCount);

            m_figure.AxisAuto(m_xs, m_ys, .9, .9);

            m_figure.PlotLines(m_xs, m_ys, 1, Color.Red);
        }
    }
}
