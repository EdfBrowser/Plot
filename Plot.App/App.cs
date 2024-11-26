using Plot.Core;
using Plot.Core.Series;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer addNewDataTimer = new Timer() { Interval = 10, Enabled = false };
        private readonly Timer updatePlotTimer = new Timer() { Interval = 50, Enabled = false };
        private readonly SignalPlotSeries signalPlotSeries1;

        private readonly Random m_rand = new Random();

        private double[] Data = new double[1000];
        private double[] Sine = DataGen.SineAnimated(1000);

        int currentIndex;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            addNewDataTimer.Tick += AddNewData;
            updatePlotTimer.Tick += UpdatePlot;

            var xAxis = formPlot1.Figure.BottomAxes[0];
            xAxis.AxisLabel.Label = "Time";
            var yAxis = formPlot1.Figure.LeftAxes[0];
            yAxis.AxisLabel.Label = "Stream1";
            signalPlotSeries1 = formPlot1.Figure.AddSignalPlotSeries(xAxis, yAxis);
            signalPlotSeries1.Data = Data;
            signalPlotSeries1.SampleRate = 1;

            formPlot1.Refresh();
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh();
        }

        private void AddNewData(object sender, EventArgs e)
        {
            Data[currentIndex] = Sine[currentIndex];
            currentIndex = (currentIndex + 1) % 1000;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addNewDataTimer.Enabled ^= true;
            updatePlotTimer.Enabled ^= true;
        }
    }
}
