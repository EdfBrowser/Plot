using Plot.Core;
using Plot.Core.EventProcess;
using System;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class FormPlot : UserControl
    {
        private readonly Figure m_figure;
        private readonly EventManager m_eventManager;

        public FormPlot()
        {
            m_figure = new Figure();
            m_eventManager = m_figure.EventManager;
            Figure.OnBitmapChanged += OnBitmapChanged;
            Figure.OnBitmapUpdated += OnBitmapUpdated;

            InitializeComponent();

            pictureBox1.SizeChanged += PictureBox1_SizeChanged;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseClick += PictureBox1_MouseClick;
            pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;
        }

        public Figure Figure => m_figure;

        private void OnBitmapUpdated(object sender, EventArgs e) => pictureBox1.Refresh();
        private void OnBitmapChanged(object sender, EventArgs e) => pictureBox1.Image = m_figure.GetLatestBitmap();


        #region Event
        private void PictureBox1_SizeChanged(object sender, EventArgs e) => m_figure.Resize(pictureBox1.Width, pictureBox1.Height);

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e) => m_eventManager.MouseScroll(GetInputState(e));
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) => m_eventManager.MouseDown(GetInputState(e));
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) => m_eventManager.MouseUp(GetInputState(e));
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e) => m_eventManager.MouseMove(GetInputState(e));
        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e) => m_eventManager.MouseDoubleClick(GetInputState(e));
        private void PictureBox1_MouseClick(object sender, MouseEventArgs e) { }
        #endregion

        private static InputState GetInputState(MouseEventArgs e)
            => new InputState(
               (e.X, e.Y),
               (e.Button == MouseButtons.Left, e.Button == MouseButtons.Right, e.Button == MouseButtons.Middle),
               (ModifierKeys.HasFlag(Keys.Shift), ModifierKeys.HasFlag(Keys.Control), ModifierKeys.HasFlag(Keys.Alt)),
                e.Delta);
    }
}
