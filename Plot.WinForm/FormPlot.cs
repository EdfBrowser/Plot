using Plot.Core;
using Plot.Core.EventProcess;
using System;
using System.Windows.Forms;

namespace Plot.WinForm
{
    public partial class FormPlot : UserControl
    {
        public Figure Figure { get; }

        public FormPlot()
        {
            Figure = new Figure();
            Figure.OnBitmapChanged += OnBitmapChanged;
            Figure.OnBitmapUpdated += OnBitmapUpdated;

            InitializeComponent();

            Load += FormPlot_Load;
            pictureBox1.SizeChanged += PictureBox1_SizeChanged;
            pictureBox1.MouseWheel += PictureBox1_MouseWheel;
            pictureBox1.MouseDown += PictureBox1_MouseDown;
            pictureBox1.MouseUp += PictureBox1_MouseUp;
            pictureBox1.MouseMove += PictureBox1_MouseMove;
            pictureBox1.MouseClick += PictureBox1_MouseClick;
            pictureBox1.MouseDoubleClick += PictureBox1_MouseDoubleClick;
        }


        public void Refresh(bool lowQuality = false, float scale = 1.0f) => Figure.Render(lowQuality, scale);
        private void OnBitmapUpdated(object sender, EventArgs e) => pictureBox1.Refresh();
        private void OnBitmapChanged(object sender, EventArgs e) => pictureBox1.Image = Figure.GetLatestBitmap();



        #region Event
        private void FormPlot_Load(object sender, EventArgs e) => Refresh();
        private void PictureBox1_SizeChanged(object sender, EventArgs e) => Figure.Resize(pictureBox1.Width, pictureBox1.Height);

        private void PictureBox1_MouseWheel(object sender, MouseEventArgs e) => Figure.MouseWheel(GetInputState(e));
        private void PictureBox1_MouseDown(object sender, MouseEventArgs e) => Figure.MouseDown(GetInputState(e));
        private void PictureBox1_MouseUp(object sender, MouseEventArgs e) => Figure.MouseUp(GetInputState(e));
        private void PictureBox1_MouseMove(object sender, MouseEventArgs e) => Figure.MouseMove(GetInputState(e));
        private void PictureBox1_MouseDoubleClick(object sender, MouseEventArgs e) => Figure.MouseDoubleClick(GetInputState(e));
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
