using Plot.Core;
using Plot.Core.Series;
using System;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer addNewDataTimer = new Timer() { Interval = 300, Enabled = false };
        private readonly Timer updatePlotTimer = new Timer() { Interval = 200, Enabled = false };
        private readonly SampleDataSeries dataStreamer1;
        private readonly SampleDataSeries dataStreamer2;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            addNewDataTimer.Tick += AddNewData;
            updatePlotTimer.Tick += updatePlot;

            var xAxis = formPlot1.Figure.BottomAxes[0];
            xAxis.Title = "Time";
            var yAxis = formPlot1.Figure.LeftAxes[0];
            yAxis.Title = "Stream1";
            dataStreamer1 = formPlot1.Figure.AddDataStreamer(xAxis, yAxis);
            dataStreamer1.SampleRate = 100; // 1000 Hz
            dataStreamer1.AddSamples(DataGen.SineAnimated(100));


            var yAxis1 = formPlot1.Figure.AddAxes(Core.Enum.Edge.Left);
            yAxis1.Title = "Stream2";
            dataStreamer2 = formPlot1.Figure.AddDataStreamer(xAxis, yAxis1);
            dataStreamer2.SampleRate = 1000; // 1000 Hz
            dataStreamer2.AddSamples(DataGen.SineAnimated(1000));

            formPlot1.Refresh(false, 1.0f);
        }

        private void updatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh(false, 1.0f);
        }

        private void AddNewData(object sender, EventArgs e)
        {
            dataStreamer1.AddSamples(DataGen.SineAnimated(1000));
            //dataStreamer2.AddSamples(DataGen.SineAnimated(1000));
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addNewDataTimer.Enabled ^= true;
            updatePlotTimer.Enabled ^= true;
        }
    }
}
