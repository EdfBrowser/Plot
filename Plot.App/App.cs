using Plot.Core;
using Plot.Core.Enum;
using Plot.Core.Series;
using System;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer addNewDataTimer = new Timer() { Interval = 10, Enabled = false };
        private readonly Timer updatePlotTimer = new Timer() { Interval = 50, Enabled = false };
        private readonly StreamerPlotSeries streamerPlotSeries1;
        private readonly StreamerPlotSeries streamerPlotSeries2;
        private readonly SignalPlotSeries signalPlotSeries1;

        private readonly Random m_rand = new Random();

        private readonly double[] Data = new double[1000];
        private readonly double[] Sine = DataGen.SineAnimated(100000);

        readonly int currentIndex;
        int index;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            addNewDataTimer.Tick += AddNewData;
            updatePlotTimer.Tick += UpdatePlot;

            var xAxis = formPlot1.Figure.DefaultXAxis;
            xAxis.AxisLabel.Label = "Time";
            var yAxis1 = formPlot1.Figure.DefaultYAxis;
            var yAxis2 = formPlot1.Figure.AddAxes(Edge.Left);
            yAxis1.AxisLabel.Label = "Stream1";
            yAxis2.AxisLabel.Label = "Stream2";
            streamerPlotSeries1 = formPlot1.Figure.AddStreamerPlotSeries(xAxis, yAxis1, 500);
            streamerPlotSeries2 = formPlot1.Figure.AddStreamerPlotSeries(xAxis, yAxis2, 1000);
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh();
        }

        private void AddNewData(object sender, EventArgs e)
        {
            streamerPlotSeries1.Add(Sine[index] + m_rand.NextDouble());
            streamerPlotSeries2.Add(Sine[index] + m_rand.NextDouble());
            index++;
            //Data[currentIndex] = Sine[index++];
            //currentIndex = (currentIndex + 1) % 1000;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addNewDataTimer.Enabled ^= true;
            updatePlotTimer.Enabled ^= true;
        }
    }
}
