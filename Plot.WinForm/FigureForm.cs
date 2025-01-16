using SkiaSharp.Views.Desktop;
using System.ComponentModel;

namespace Plot.WinForm
{
    [ToolboxItem(true)]
    public class FigureForm : FigureFormBase
    {
        private SKControl m_sKControl;
        public FigureForm()
        {
            if (IsDesignerAlternative) return;

            SetupSKControl();

            //HandleCreated += (s, e) => SetupSKControl();
            //HandleDestroyed += (s, e) => TearDownSKControl();
            // Form 才会创建handler
        }

        private void SetupSKControl()
        {
            TearDownSKControl();

            m_sKControl = new SKControl() { Dock = System.Windows.Forms.DockStyle.Fill };

            m_sKControl.MouseDown += SKControl_MouseDown;
            m_sKControl.MouseUp += SKControl_MouseUp;
            m_sKControl.MouseMove += SKControl_MouseMove;
            m_sKControl.MouseWheel += SKControl_MouseWheel;
            m_sKControl.KeyDown += SKControl_KeyDown;
            m_sKControl.KeyUp += SKControl_KeyUp;
            m_sKControl.LostFocus += SKControl_LostFocus;
            m_sKControl.PaintSurface += SKControl_PaintSurface;

            Controls.Add(m_sKControl);
        }

        private void TearDownSKControl()
        {
            if (m_sKControl == null) return;

            m_sKControl.PaintSurface -= SKControl_PaintSurface;
            m_sKControl.MouseDown -= SKControl_MouseDown;
            m_sKControl.MouseUp -= SKControl_MouseUp;
            m_sKControl.MouseMove -= SKControl_MouseMove;
            m_sKControl.MouseWheel -= SKControl_MouseWheel;
            m_sKControl.KeyDown -= SKControl_KeyDown;
            m_sKControl.KeyUp -= SKControl_KeyUp;

            Controls.Remove(m_sKControl);

            if (!m_sKControl.IsDisposed)
                m_sKControl.Dispose();
        }

        private void SKControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
            => Figure.Render(e.Surface);

        public override void Refresh()
        {
            base.Refresh();
        }
    }
}
