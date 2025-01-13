using Plot.Skia;
using System;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class FigureFormDemo : Form
    {
        public FigureFormDemo()
        {
            InitializeComponent();

#if DEBUG
            var axisManager = figureForm1.Figure.AxisManager;
            axisManager.Remove(Edge.Bottom);
            IAxis axis = axisManager.AddDateTimeBottomAxis();
            DateTime dt = DateTime.Now;
            PixelRange pixelPanel =
                new PixelRange(dt.ToOADate(), dt.AddSeconds(10).ToOADate());
            AxisManager.SetLimits(pixelPanel, axis);
#endif
        }
    }
}
