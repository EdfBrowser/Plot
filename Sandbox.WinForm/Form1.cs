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
            IXAxis x = axisManager.DefaultBottom;
            IYAxis y = axisManager.DefaultLeft;
            SignalSeries sig = seriesManager.AddSignalSeries(x, y);
            sig.Data = Generate.Sin(count: 10);
            //sig.SampleRate = 10;
        }
    }
}
