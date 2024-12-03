using Plot.Core;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Plot.App
{

    public partial class App : Form
    {
        private readonly Timer m_addNewDataTimer = new Timer() { Interval = 1, Enabled = false };
        private readonly Timer m_updatePlotTimer = new Timer() { Interval = 5, Enabled = false };
        private readonly List<IPlotSeries> m_seriesList = new List<IPlotSeries>();
        private readonly StreamerPlotSeries m_streamerPlotSeries1;
        private readonly StreamerPlotSeries m_streamerPlotSeries2;
        private readonly SignalPlotSeries m_signalPlotSeries1;

        private readonly Random m_rand = new Random();

        private readonly double[] m_sine = DataGen.SineAnimated(100000);

        int m_currentIndex;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_addNewDataTimer.Tick += AddNewData;
            m_updatePlotTimer.Tick += UpdatePlot;

            var xAxis = formPlot1.Figure.DefaultXAxis;
            xAxis.AxisLabel.Label = "Time";
            xAxis.IsDateTime = true;



            var yAxis1 = formPlot1.Figure.DefaultYAxis;
            yAxis1.AxisLabel.Label = "Stream1";
            //var yAxis2 = formPlot1.Figure.AddAxes(Edge.Left);
            //yAxis2.AxisLabel.Label = "Stream2";
            m_streamerPlotSeries1 = formPlot1.Figure.AddStreamerPlotSeries(xAxis, yAxis1, 100);
            DateTime dateTime = new DateTime(2000, 1, 1);
            m_streamerPlotSeries1.OffsetX = dateTime.ToOADate();
            //m_streamerPlotSeries2 = formPlot1.Figure.AddStreamerPlotSeries(xAxis, yAxis2, 1000);
            //m_streamerPlotSeries2.OffsetX = dateTime.ToOADate();
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            formPlot1.Refresh();
        }

        private void AddNewData(object sender, EventArgs e)
        {
            m_streamerPlotSeries1.Add(m_sine[m_currentIndex]);
            m_currentIndex = (m_currentIndex + 1) % 1000;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            m_addNewDataTimer.Enabled ^= true;
            m_updatePlotTimer.Enabled ^= true;
        }
    }
}
