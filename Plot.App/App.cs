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
        private long m_count = 0;

        private int[] m_indexs;
        private string m_scrolling;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_plt = formPlot1.Figure;

            formPlot1.PltSizeChanged += FormPlot1_PltSizeChanged;
            m_addDataTimer = new Timer() { Enabled = false, Interval = 10 };
            m_updatePlotTimer = new Timer() { Enabled = false, Interval = 50 };

            comboBox1.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;

            button2.Click += Button2_Click;
            Load += App_Load;
            m_addDataTimer.Tick += AddDataTimer_Tick;
            m_updatePlotTimer.Tick += UpdatePlot;
        }


        private void App_Load(object sender, EventArgs e)
        {
            WorkFlow();
        }


        private void AddDataTimer_Tick(object sender, EventArgs e)
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < seriesList.Count; i++)
            {
                seriesList[i].AddSample(m_sine[m_indexs[i]]);
                m_indexs[i] = (m_indexs[i] + 1) % m_sine.Length;
            }

            m_count += 1;

            double lastX = m_count * (1.0 / m_sample);
            m_plt.AxisManager.GetDefaultXAxis().ScrollPosition = lastX;
            if (m_scrolling == "Sweeping")
            {
                var vline = m_plt.SeriesManager.GetVLinePlotSeries().FirstOrDefault();
                if (vline != null)
                    vline.X = lastX;
            }
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            // 更新Y轴
            {
                var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
                for (int i = 0; i < seriesList.Count; i++)
                {
                    double min = seriesList[i].Data.Min();
                    double max = seriesList[i].Data.Max();

                    seriesList[i].YAxis.Dims.SetLimits(min, max);
                }

                //for (int i = 0; i < seriesList.Count; i++)
                //{
                //    seriesList[i].OffsetX = seriesList[i].XAxis.Dims.Min;
                //}
            }
           
            m_plt.Render();
        }


        private void FormPlot1_PltSizeChanged(object sender, EventArgs e)
        {
            Measure();
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            m_addDataTimer.Stop();
            m_updatePlotTimer.Stop();

            WorkFlow();

            m_addDataTimer.Start();
            m_updatePlotTimer.Start();
        }

        #region chart
        private void WorkFlow()
        {
            UpdateChartParameters();

            CreateChart();
            Measure();
            Start();

            m_plt.AxisManager.SetGrid(checkBox2.Checked);
            m_plt.Render();
        }

        private void Measure()
        {
            Graphics g = CreateGraphics();
            // 每1英寸=2.54厘米
            double m_xPxPerCM = g.DpiX / 2.54;
            // 总共多少厘米
            double m_xDataAreaTotalCM = m_plt.GetXDataSizePx() / m_xPxPerCM;
            double m_xTotalUnit = m_xDataAreaTotalCM / 3.0;

            g.Dispose();

            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            xAxis.Dims.SetLimits(0, m_xTotalUnit);
        }

        private void CreateChart()
        {
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            xAxis.AxisTick.Animation = false;
            xAxis.ScrollPosition = 0;
            xAxis.ScrollMode = (XAxisScrollMode)Enum.Parse(typeof(XAxisScrollMode), m_scrolling);
            xAxis.AxisTick.TickGenerator.MajorDiv = 1.0;
            xAxis.AxisTick.TickGenerator.LabelFormat = TickLabelFormat.DateTime;
            xAxis.AxisTick.TickLabelRotation = 0;
            xAxis.AxisTick.TickGenerator.MinorDivCount = (int)(1 / 0.25);
            xAxis.AxisTick.TickGenerator.DateTimeFormatString = "yyyy/MM/dd\nhh:mm:ss";
            xAxis.AxisTick.HorizontalAlignment = StringAlignment.Near;
            xAxis.AxisTick.VerticalAlignment = StringAlignment.Near;

            if (xAxis.AxisTick.TickGenerator.LabelFormat == TickLabelFormat.DateTime)
                xAxis.SetDateTimeOrigin(DateTime.Now);
        }

        private void UpdateChartParameters()
        {
            m_count = 0;

            m_channelCount = int.Parse(comboBox3.SelectedItem.ToString());
            m_indexs = new int[m_channelCount];

            m_scrolling = comboBox1.SelectedItem.ToString();
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
                y.AxisTick.MajorGridVisible = false;
                y.AxisTick.MinorGridVisible = false;

                y.AxisLabel.Label = $"Channel {i}";
                y.AxisLabel.LabelExtendOutward = false;
                y.AxisLabel.Rotation = 0;
                y.AxisLabel.HorizontalAlignment = StringAlignment.Near;
                y.AxisLabel.VerticalAlignment = StringAlignment.Center;

                var series = new StreamerPlotSeries(x, y);
                series.SampleRate = m_sample;
                series.Color = DataGen.randomColor;

                m_plt.SeriesManager.AddSeries(series);
            }

            m_plt.AxisManager.AxisSpace = 10;

            if (m_scrolling == "Sweeping")
            {
                var vline = new VLinePlotSeries(x, m_plt.AxisManager.GetDefaultYAxis());
                vline.Color = DataGen.randomColor;
                m_plt.SeriesManager.AddSeries(vline);
            }
        }

        #endregion
    }
}
