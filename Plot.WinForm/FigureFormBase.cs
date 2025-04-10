using Plot.Skia;
using SkiaSharp.Views.Desktop;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Plot.WinForm
{
    [ToolboxItem(false)]
#if DEBUG
    public class FigureFormBase
#else
    public abstract class FigureFormBase
#endif
        : SKControl, IFigureControl
    {
        private const int m_defaultDpi = 96;

        protected FigureFormBase()
        {
            bool isDesignMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            try
            {
                Figure = new Figure() { FigureControl = this };
                DisplayScale = DetectDisplayScale();
            }
            catch (Exception)
            {
                if (isDesignMode)
                    return;

                throw;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Figure Figure { get; protected set; }

        public float DisplayScale { get; }
        protected bool IsDesignerAlternative { get; }


        public void Reset()
        {
            Figure figure = new Figure();
            Reset(figure, true);
        }

        public void Reset(Figure figure, bool disposeOldFigure)
        {
            Figure old = Figure;
            Figure = figure;
            if (disposeOldFigure)
                old?.Dispose();

            Figure.FigureControl = this;
        }

        public float DetectDisplayScale()
        {
            using (System.Drawing.Graphics g = CreateGraphics())
                return g.DpiX / m_defaultDpi;
        }
    }
}
