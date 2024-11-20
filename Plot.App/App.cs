using Plot.Core;
using Plot.Core.Series;
using System;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer addNewDataTimer = new Timer() { Interval = 100, Enabled = true };
        private readonly Timer updatePlotTimer = new Timer() { Interval = 500, Enabled = true };
        private readonly SampleDataSeries dataStreamer1;
        private readonly SampleDataSeries dataStreamer2;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            addNewDataTimer.Tick += AddNewData;
            updatePlotTimer.Tick += updatePlot;

            dataStreamer1 = formPlot1.Figure.AddDataStreamer(0, 0);
            dataStreamer1.SampleRate = 1000; // 1000 Hz
            dataStreamer1.AddSamples(DataGen.SineAnimated(1000));
            dataStreamer1.AxisAuto();

            dataStreamer2 = formPlot1.Figure.AddDataStreamer(0, 1);
            dataStreamer2.SampleRate = 1000; // 1000 Hz
            dataStreamer2.AddSamples(DataGen.SineAnimated(1000));
            dataStreamer2.AxisAuto();
        }

        private void updatePlot(object sender, EventArgs e)
        {
            //formPlot1.Refresh(false, 1.0f);
        }

        private void AddNewData(object sender, EventArgs e)
        {
            //dataStreamer1.AddSamples(DataGen.SineAnimated(1000));
            //dataStreamer1.AxisAuto();
            //dataStreamer2.AddSamples(DataGen.RandomWalk(1000));
            //dataStreamer2.AxisAuto();
        }
    }
}
