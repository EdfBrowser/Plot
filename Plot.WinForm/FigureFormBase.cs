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
            bool isDesignMode = DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime;

            try
            {
                Figure = new Figure() { FigureControl = this };
                UserInputProcessor = new UserInputProcessor(this);
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [Browsable(false)]
        protected UserInputProcessor UserInputProcessor { get; }

        public float DisplayScale { get; }
        protected bool IsDesignerAlternative { get; }


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

        protected void SKControl_KeyUp(object sender, KeyEventArgs e)
        {

        }

        protected void SKControl_KeyDown(object sender, KeyEventArgs e)
        {
        }

        protected void SKControl_MouseWheel(object sender, MouseEventArgs e)
            => UserInputProcessor.ProcessMouseWheel(e);

        protected void SKControl_MouseMove(object sender, MouseEventArgs e)
            => UserInputProcessor.ProcessMouseMove(e);

        protected void SKControl_MouseUp(object sender, MouseEventArgs e)
            => UserInputProcessor.ProcessMouseUp(e);

        protected void SKControl_MouseDown(object sender, MouseEventArgs e)
            => UserInputProcessor.ProcessMouseDown(e);

        protected void SKControl_LostFocus(object sender, EventArgs e)
            => UserInputProcessor.ProcessLostFocus();

        private static Color ConvertColor(System.Drawing.Color color)
            => new Color(color.R, color.G, color.B, color.A);
    }

    public static class FigureFormExtensions
    {
        internal static void ProcessMouseDown(
            this UserInputProcessor processor, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);

            IUserAction action = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    action = new LeftMouseDown(p);
                    break;
                case MouseButtons.Right:
                    action = new RightMouseDown(p);
                    break;
            }

            processor.Process(action);
        }

        internal static void ProcessMouseUp(
            this UserInputProcessor processor, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);

            IUserAction action = null;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    action = new LeftMouseUp(p);
                    break;
                case MouseButtons.Right:
                    action = new RightMouseUp(p);
                    break;
            }

            processor.Process(action);
        }

        internal static void ProcessMouseMove(
            this UserInputProcessor processor, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);
            IUserAction action = new MouseMove(p);
            processor.Process(action);
        }

        internal static void ProcessMouseWheel(
            this UserInputProcessor processor, MouseEventArgs e)
        {
            PointF p = new PointF(e.X, e.Y);

            IUserAction action = e.Delta > 0
                ? (IUserAction)new MouseWheelUp(p)
                : (IUserAction)new MouseWheelDown(p);

            processor.Process(action);
        }

        internal static void ProcessLostFocus(this UserInputProcessor processor)
            => processor.ProcessLostFocus();
    }
}
