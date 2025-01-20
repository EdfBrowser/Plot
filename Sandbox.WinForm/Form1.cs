using Plot.Skia;
using System;
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

            IXAxis x = axisManager.DefaultBottom;
            IYAxis y = axisManager.DefaultLeft;
            SignalSeries sig = seriesManager.AddSignalSeries(x, y);
            sig.Data = Generate.Sin(count: 1_0);
            //sig.SampleRate = 10;
        }
    }
}
