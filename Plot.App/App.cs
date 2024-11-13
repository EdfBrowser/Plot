using Plot.Chart;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        private readonly Figure m_figure;
        private Bitmap m_bitmap;

        private Queue<Bitmap> m_bitmapQueue = new Queue<Bitmap>();


        public App()
        {
            InitializeComponent();

            m_figure = new Figure(pictureBox1.Width, pictureBox1.Height);

            Load += App_Load;
            SizeChanged += App_SizeChanged;
        }

        private void App_Load(object sender, EventArgs e)
        {
            Text = "Plot.App";
            RefreshImage();
        }


        private void App_SizeChanged(object sender, EventArgs e)
        {
            m_figure.Resize(pictureBox1.Width, pictureBox1.Height);
            m_figure.FrameReDraw();
            RefreshImage();
        }


        private void RefreshImage()
        {
            if (m_bitmap != null)
                m_bitmapQueue.Enqueue(m_bitmap);

            Bitmap bitmap = new Bitmap(pictureBox1.Width, pictureBox1.Height);

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
