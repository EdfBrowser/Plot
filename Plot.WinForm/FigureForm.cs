using SkiaSharp.Views.Desktop;
using System.ComponentModel;

namespace Plot.WinForm
{
    [ToolboxItem(true)]
    public class FigureForm : FigureFormBase
    {
        public FigureForm()
        {
            if (IsDesignerAlternative) return;

            SetupSKControl();
        }

        private void SetupSKControl()
        {
            PaintSurface += SKControl_PaintSurface;
        }

        private void TearDownSKControl()
        {
            PaintSurface -= SKControl_PaintSurface;
        }

        private void SKControl_PaintSurface(object sender, SKPaintSurfaceEventArgs e)
            => Figure.Render(e.Surface);

        public override void Refresh()
        {
            base.Refresh();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);

            if (disposing)
            {
                TearDownSKControl();
                Figure?.Dispose();
            }
        }
    }
}
