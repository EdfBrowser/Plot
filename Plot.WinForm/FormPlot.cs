using Plot.Core;
using System;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class FormPlot : UserControl
    {
        private readonly ControlBackend m_backend;
        private readonly bool m_showBenchMark = false;
        private readonly bool m_busy = false;

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


        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            m_backend.Resize(pictureBox1.Width, pictureBox1.Height);
        }

        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
        }

        private void pictureBox1_MouseUp(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseClick(object sender, MouseEventArgs e)
        {

        }

        private void pictureBox1_MouseDoubleClick(object sender, MouseEventArgs e)
        {

        }


        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {

        }
    }
}
