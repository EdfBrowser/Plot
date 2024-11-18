using Plot.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class FormPlot : UserControl
    {
        private ControlBackend m_backend;
        private bool m_showBenchMark = false;
        private bool m_busy = false;

        public FormPlot()
        {
            m_backend = new ControlBackend(1, 1);
            m_backend.OnBitmapChanged += OnBitmapChanged;
            m_backend.OnBitmapUpdated += OnBitmapUpdated;

            InitializeComponent();

            pictureBox1.SizeChanged += pictureBox1_SizeChanged;
            pictureBox1.MouseWheel += pictureBox1_MouseWheel;
            pictureBox1.MouseDown += pictureBox1_MouseDown;
            pictureBox1.MouseUp += pictureBox1_MouseUp;
            pictureBox1.MouseMove += pictureBox1_MouseMove;
            pictureBox1.MouseClick += pictureBox1_MouseClick;
            pictureBox1.MouseDoubleClick += pictureBox1_MouseDoubleClick;
        }

        public override void Refresh()
        {
            base.Refresh();

            m_backend.Render();
        }

        private void OnBitmapUpdated(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void OnBitmapChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = m_backend.GetLatestBitmap();
        }

        public Figure Figure => m_backend.Plt;


        public void PlotSignal(List<double[]> values, double sampleRate, double offsetX = 0,
           double offsetY = 0, float lineWidth = 1f, Color? lineColor = null, string label = null, bool render = true)
        {
            //m_figure.ClearSeries();
            //for( int i = 0; i < values.Count; i++)
            //{
            //    m_figure.AddSignal(m_figure.XAxis, m_figure.YAxes[i], values[i],
            //     1.0 / sampleRate, offsetX, offsetY, lineWidth, lineColor);
            //}
          
            ////m_figure.GraphClear();
            //if (render)
            //    Render();
        }
       
        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            m_backend.Resize(pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            //double mag = 1.2;
            //if (e.Delta > 0) m_figure.Zoom(mag, mag);
            //else m_figure.Zoom(1.0 / mag, 1.0 / mag);
            //Render(true);
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left) m_figure.MousePanStart(e.X, e.Y); // left-click-drag pans
            //else if (e.Button == MouseButtons.Right) m_figure.MouseZoomStart(e.X, e.Y); // right-click-drag zooms
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Left) m_figure.MousePanEnd();
            //else if (e.Button == MouseButtons.Right) m_figure.MouseZoomEnd();
        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {
            //if (e.Button == MouseButtons.Middle) AxisAuto();
        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            //m_showBenchMark = !m_showBenchMark; // double-click graph to display benchmark stats
            //Render();
        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            //if (m_figure.MouseIsDragging() && m_busy == false)
            //{
            //    m_figure.MouseMove(e.X, e.Y);
            //    m_busy = true;
            //    Render(true);
            //    Application.DoEvents();
            //    m_busy = false;
            //}
        }
    }
}
