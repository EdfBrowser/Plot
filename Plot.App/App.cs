using Plot.Core;
using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        private int m_channelCount;

        private readonly Timer m_addDataTimer;
        private readonly Timer m_updatePlotTimer;
        private readonly double[] m_sine = DataGen.SineAnimated(1_000_0);
        private readonly int m_sample = 100;
        private readonly Figure m_plt;
        private readonly Random m_random = new Random(10);

        private int[] m_indexs;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_plt = formPlot1.Figure;

            formPlot1.PltSizeChanged += FormPlot1_PltSizeChanged;
            m_addDataTimer = new Timer() { Enabled = false, Interval = 10 };
            m_updatePlotTimer = new Timer() { Enabled = false, Interval = 50 };

            comboBox3.SelectedIndex = 0;

            button1.Click += Button1_Click;
            button2.Click += Button2_Click;
            Load += App_Load;
            m_addDataTimer.Tick += AddDataTimer_Tick;
            m_updatePlotTimer.Tick += UpdatePlot;
        }


        private void App_Load(object sender, EventArgs e)
        {
            WorkFlow();
        }


        private long m_count = 0;
        private void AddDataTimer_Tick(object sender, EventArgs e)
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < seriesList.Count; i++)
            {
                seriesList[i].Data[m_indexs[i]] = m_sine[m_indexs[i]];
                m_indexs[i] = (m_indexs[i] + 1) % seriesList[i].Data.Length;
            }

            m_count += 1;

            double lastX = m_count * (1.0 / m_sample);
            m_plt.AxisManager.GetDefaultXAxis().ScrollPosition = lastX;
        }
       
        private void UpdatePlot(object sender, EventArgs e)
        {
            m_plt.Render();
        }


        private void FormPlot1_PltSizeChanged(object sender, EventArgs e)
        {
            Button1_Click(this, e);
            WorkFlow();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            m_addDataTimer.Enabled ^= true;
            m_updatePlotTimer.Enabled ^= true;
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            Button1_Click(this, e); 
            WorkFlow();
        }

        #region chart
        private void WorkFlow()
        {
            UpdateChartParameters();

            CreateChart();
            Start();

            m_plt.AxisManager.SetGrid(checkBox2.Checked);
            m_plt.Render();
        }

        private double Measure()
        {
            Graphics g = CreateGraphics();
            // 每1英寸=2.54厘米
            double m_xPxPerCM = g.DpiX / 2.54;
            // 总共多少厘米
            double m_xDataAreaTotalCM = m_plt.GetXDataSizePx() / m_xPxPerCM;
            double m_xTotalUnit = m_xDataAreaTotalCM / 3.0;

            g.Dispose();

            return m_xTotalUnit;
        }

        private void CreateChart()
        {
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            xAxis.ScrollPosition = 0;
            xAxis.ScrollMode = XAxisScrollMode.Scrolling;
            xAxis.AxisTick.TickGenerator.MajorDiv = 1.0;
            xAxis.AxisTick.TickGenerator.LabelFormat = TickLabelFormat.DateTime;
            xAxis.AxisTick.TickLabelRotation = 0;
            xAxis.AxisTick.TickGenerator.MinorDivCount = (int)(1 / 0.25);
            xAxis.AxisTick.TickGenerator.DateTimeFormatString = "yyyy/MM/dd\nhh:mm:ss";
            xAxis.AxisTick.HorizontalAlignment = StringAlignment.Near;
            xAxis.AxisTick.VerticalAlignment = StringAlignment.Near;

            double unit = Measure();
            if (xAxis.AxisTick.TickGenerator.LabelFormat == TickLabelFormat.DateTime)
                xAxis.SetDateTimeOrigin(DateTime.Now);

            xAxis.Dims.SetLimits(0, unit);
        }

        private void UpdateChartParameters()
        {
            m_channelCount = int.Parse(comboBox3.SelectedItem.ToString());
            m_indexs = new int[m_channelCount];
            checkBox1.CheckState ^= CheckState.Unchecked;
        }

        private void Start()
        {
            m_plt.AxisManager.ClearYAxes();
            m_plt.SeriesManager.Clear();

            Axis x = m_plt.AxisManager.GetDefaultXAxis();
            for (int i = 0; i < m_channelCount; i++)
            {
                Axis y = m_plt.AxisManager.AddAxes(Edge.Left);
                y.AxisTick.MajorTickVisible = false;
                y.AxisTick.MinorTickVisible = false;
                y.AxisTick.TickLabelVisible = false;
                y.AxisLabel.Label = $"Channel {i}";
                y.AxisLabel.LabelExtendOutward = false;
                y.AxisLabel.Rotation = 0;
                y.AxisLabel.HorizontalAlignment = StringAlignment.Near;
                y.AxisLabel.VerticalAlignment = StringAlignment.Center;

                var series = new StreamerPlotSeries(x, y, m_sample * 6);
                series.SampleRate = m_sample;
                series.Color = DataGen.randomColor;

                m_plt.SeriesManager.AddSeries(series);
            }

            m_plt.AxisManager.AxisSpace = 10;
        }

        #endregion
    }
}
