using Plot.Core;
using System;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer addNewDataTimer = new Timer() { Interval = 10, Enabled = true };
        private readonly Timer updatePlotTimer = new Timer() { Interval = 50, Enabled = true };
        private readonly DataStreamSeries dataStreamer1;
        private readonly DataStreamSeries dataStreamer2;
        private readonly Random rand = new Random();

        private double lastPointValue1 = 0;
        private double lastPointValue2 = 0;
        public App()
        {
            InitializeComponent();


            Text = "Plot.App";
            addNewDataTimer.Tick += AddNewData;
            updatePlotTimer.Tick += updatePlot;
            dataStreamer1 = formPlot1.Figure.AddDataStreamer(0, 0, 1000);
            dataStreamer2 = formPlot1.Figure.AddDataStreamer(0, 1, 1000);
        }

        private void updatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh();
        }

        private void AddNewData(object sender, EventArgs e)
        {
            int count = rand.Next(10);
            for (int i = 0; i < count; i++)
            {
                lastPointValue1 += rand.NextDouble() - .5;
                lastPointValue2 += rand.NextDouble() - .5;
                dataStreamer1.Add(lastPointValue1);
                dataStreamer2.Add(lastPointValue2);
            }
        }
    }
}
