using Plot.Core;
using System;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class FormPlot : UserControl
    {
        public Figure Figure { get; }
        private readonly bool m_showBenchMark = false;
        private readonly bool m_busy = false;

        public FormPlot()
        {
            Figure = new Figure(1, 1);
            Figure.OnBitmapChanged += OnBitmapChanged;
            Figure.OnBitmapUpdated += OnBitmapUpdated;

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

            Figure.Render();
        }

        private void OnBitmapUpdated(object sender, EventArgs e)
        {
            pictureBox1.Refresh();
        }

        private void OnBitmapChanged(object sender, EventArgs e)
        {
            pictureBox1.Image = Figure.GetLatestBitmap();
        }

        private void pictureBox1_SizeChanged(object sender, EventArgs e)
        {
            Figure.Resize(pictureBox1.Width, pictureBox1.Height);
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
