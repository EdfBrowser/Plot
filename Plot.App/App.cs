using Plot.Chart;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        private readonly Figure m_figure;
        private Bitmap m_bitmap;

        private Queue<Bitmap> m_bitmapQueue = new Queue<Bitmap>();

        private int m_pointCount = 123;
        private double[] m_xs, m_ys;
        public App()
        {
            InitializeComponent();

            m_figure = new Figure(pictureBox1.Width, pictureBox1.Height)
            {
                LabelTitle = "Custom Chart",
                LabelX = "X-Axis",
                LabelY = "Y-Axis",
            };


            m_xs = m_figure.Gen.Sequence(m_pointCount);
            m_ys = m_figure.Gen.RandomWalk(m_pointCount);


            Load += App_Load;
            pictureBox1.SizeChanged += pictureBox1_SizeChanged;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
        }



        private void App_Load(object sender, EventArgs e)
        {
            Text = "Plot.App";
            Render();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            m_figure.Resize(pictureBox1.Width, pictureBox1.Height);
            m_figure.FrameReDraw();
            Render();
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            double mag = 0.9;
            if (e.Delta > 0) m_figure.Zoom(mag, mag);
            else m_figure.Zoom(1.0 / mag, 1.0 / mag);
            RefreshImage();
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
             
                RefreshImage();

                m_busy = false;
            }
        }

        private void Render()
        {
            m_figure.AxisAuto(m_xs, m_ys, .9, .9);

            RefreshImage();
        }

        private void RefreshImage()
        {
            if (m_bitmap != null)
                m_bitmapQueue.Enqueue(m_bitmap);

            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

            m_figure.BenchmarkThis();
            m_figure.PlotLines(m_xs, m_ys, 1, Color.Red);

            m_figure.Render(bitmap);
            m_bitmap = bitmap;

            pictureBox1.Image = m_bitmap;
            pictureBox1.Refresh();

            if (m_bitmapQueue.Count > 3)
            {
                m_bitmapQueue.Dequeue()?.Dispose();
            }
        }


        // 测试旋转和平移
        private Bitmap test(Bitmap bmp)
        {
            using (Graphics gfx = Graphics.FromImage(bmp))
            {
                gfx.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                gfx.Clear(Color.White);

                // 平移
                gfx.TranslateTransform(100, 0);
                // 旋转
                gfx.RotateTransform(-90);


                // 原来的坐标系下的起点和终点
                //PointF startPoint = new PointF(-300, 100);
                //PointF endPoint = new PointF(-100,0);

                // 画出线
                gfx.DrawString("Original", new System.Drawing.Font("Arial", 12), Brushes.Black, new Point(-200, 0));
                //gfx.DrawLine(Pens.Black, new Point(0, 0), new Point(0, 100));

                gfx.ResetTransform();
            }

            return bmp;
        }
    }
}
