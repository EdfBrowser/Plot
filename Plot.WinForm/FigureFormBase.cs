using Plot.Skia;
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
        : UserControl, IFigureControl
    {
        private const int m_defaultDpi = 96;

        protected FigureFormBase()
        {
            bool isDesignMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime; ;

            try
            {
                Figure = new Figure() { FigureControl = this };
                DisplayScale = DetectDisplayScale();
            }
            catch (Exception)
            {
                if (isDesignMode)
                {
                    IsDesignerAlternative = true;
                    FormsPlotDesignerAlternative altControl =
                        new FormsPlotDesignerAlternative() { Dock = DockStyle.Fill };
                    Controls.Add(altControl);
                    return;
                }

                throw;
            }
        }

        public override System.Drawing.Color BackColor
        {
            get => base.BackColor;
            set
            {
                base.BackColor = value;
                if (Figure != null)
                    Figure.BackgroundManager.FigureBackground.Color = ConvertColor(value);
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        public Figure Figure { get; protected set; }
        public float DisplayScale { get; protected set; }
        public bool IsDesignerAlternative { get; }

        public void Reset()
        {
            Figure figure = new Figure();
            figure.BackgroundManager.FigureBackground.Color = ConvertColor(BackColor);
            figure.BackgroundManager.DataBackground.Color = Color.White;
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


        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
        }

        private static Color ConvertColor(System.Drawing.Color color)
            => new Color(color.R, color.G, color.B, color.A);
    }
}
