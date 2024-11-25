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
        private readonly SampleDataSeries dataStreamer1;
        private readonly SampleDataSeries dataStreamer2;

        private readonly Random m_rand = new Random();

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
            dataStreamer1 = formPlot1.Figure.AddDataStreamer(xAxis, yAxis, 1000);
            dataStreamer1.Color = Color.Blue;

            var yAxis1 = formPlot1.Figure.AddAxes(Core.Enum.Edge.Left);
            yAxis1.AxisLabel.Label = "Stream2";
            dataStreamer2 = formPlot1.Figure.AddDataStreamer(xAxis, yAxis1, 1000);
            dataStreamer2.Color = Color.Green;

            formPlot1.Refresh();
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh();
        }

        private void AddNewData(object sender, EventArgs e)
        {
            int count = m_rand.Next(10);
            for (int i = 0; i < count; i++)
            {
                dataStreamer1.Add(m_rand.NextDouble() + .5);
                dataStreamer2.Add(m_rand.NextDouble() - .5);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            addNewDataTimer.Enabled ^= true;
            updatePlotTimer.Enabled ^= true;
        }
    }
}
