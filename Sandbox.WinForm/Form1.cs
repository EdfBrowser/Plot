using Plot.Skia;
using System.Windows.Forms;

namespace Sandbox.WinForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();


            var axisManager = figureForm1.Figure.AxisManager;
            var seriesManager = figureForm1.Figure.SeriesManager;
            axisManager.Remove(Edge.Bottom);
            IXAxis m_x = axisManager.AddDateTimeBottomAxis();
            m_x.ScrollMode = AxisScrollMode.Scrolling;

            IXAxis x1 = axisManager.DefaultBottom;
            IYAxis y1 = axisManager.DefaultLeft;

            IXAxis x2 = axisManager.AddNumericBottomAxis();
            IXAxis x3 = axisManager.AddNumericBottomAxis();
            IXAxis x4 = axisManager.AddNumericBottomAxis();
            IXAxis x5 = axisManager.AddNumericBottomAxis();

            IYAxis y2 = axisManager.AddNumericLeftAxis();
            IYAxis y3 = axisManager.AddNumericLeftAxis();
            IYAxis y4 = axisManager.AddNumericLeftAxis();
            IYAxis y5 = axisManager.AddNumericLeftAxis();

            seriesManager.AddSignalSeries(x1, y1, Generate.Sin(100), 1.0 / 1);
            seriesManager.AddSignalSeries(x2, y2, Generate.Sin(100), 1.0 / 1);
            seriesManager.AddSignalSeries(x3, y3, Generate.Sin(100), 1.0 / 1);
            seriesManager.AddSignalSeries(x4, y4, Generate.Sin(100), 1.0 / 1);
            seriesManager.AddSignalSeries(x5, y5, Generate.Sin(100), 1.0 / 1);
        }
    }
}
