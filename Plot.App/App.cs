using Plot.Core;
using Plot.Core.Enum;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;

namespace Plot.App
{
    // TODO: 手动添加每个刻度的距离，和lchart一样
    public partial class App : Form
    {
        private int m_fps;
        private int m_channelCount;
        private int m_frameCount = 0;
        private int m_playSpeed;

        private Timer m_updatePlotTimer;
        private readonly double[] m_sine = DataGen.SineAnimated(100000);
        private readonly Figure m_plt;

        private int[] m_indexs;


        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_plt = formPlot1.Figure;

            m_updatePlotTimer = new Timer() { Enabled = false };

            comboBox1.SelectedIndex = 0;
            comboBox2.SelectedIndex = 0;
            comboBox3.SelectedIndex = 0;



            button1.Click += Button1_Click;
            checkBox1.CheckStateChanged += CheckBox1_CheckStateChanged;
            checkBox2.CheckStateChanged += CheckBox2_CheckStateChanged;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            comboBox2.SelectedIndexChanged += ComboBox2_SelectedIndexChanged;
            comboBox3.SelectedIndexChanged += ComboBox3_SelectedIndexChanged;
            Load += App_Load;
            m_updatePlotTimer.Tick += UpdatePlot;
        }


        private void UpdateParametersFromComboBoxes()
        {
            m_playSpeed = GetComboBoxValue(comboBox1);
            m_fps = GetComboBoxValue(comboBox2);
            m_channelCount = GetComboBoxValue(comboBox3);
            m_indexs = new int[m_channelCount];


            m_updatePlotTimer.Interval = 1000 / (m_fps * m_playSpeed);
        }

        private int GetComboBoxValue(ComboBox comboBox)
        {
            return int.Parse(comboBox.SelectedItem.ToString());
        }

        private void App_Load(object sender, EventArgs e)
        {
            WorkFlow();
        }

        private void WorkFlow()
        {
            UpdateParametersFromComboBoxes();
            InitializeAxesAndSeries();
            UpdateAxisLimits();
            CheckBox2_CheckStateChanged(null, null);
            m_plt.Render();
        }

        private void InitializeAxesAndSeries()
        {
            CreateYAxes();
            CreateSeries();
        }

        private void CreateYAxes()
        {
            m_plt.AxisManager.ClearYAxes();
            for (int i = 0; i < m_channelCount; i++)
            {
                m_plt.AxisManager.AddAxes(Edge.Left);
            }
        }

        private void CreateSeries()
        {
            m_plt.SeriesManager.Clear();
            DateTime dateTime = new DateTime(2000, 1, 1);
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            xAxis.IsDateTime = true;

            List<Axis> yAxes = m_plt.AxisManager.LeftAxes().ToList();
            foreach (var yAxis in yAxes)
            {
                var series = m_plt.SeriesManager.AddStreamerPlotSeries(xAxis, yAxis, 100);
                series.Color = DataGen.randomColor;
                series.OffsetX = dateTime.ToOADate();
            }
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < seriesList.Count; i++)
            {
                UpdateSeriesData(seriesList[i], i);
            }
            m_plt.Render();
            m_frameCount++;
        }

        private void UpdateSeriesData(StreamerPlotSeries series, int index)
        {
            int number = CalculateSamplesToAdd(series);
            series.AddRange(m_sine.Skip(m_indexs[index]).Take(number));
            m_indexs[index] = (m_indexs[index] + number) % m_sine.Length;
        }

        private int CalculateSamplesToAdd(StreamerPlotSeries series)
        {
            int number = series.SampleRate / m_fps * m_playSpeed;
            if (m_frameCount % 3 == 0) number += 1;
            return number;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            m_updatePlotTimer.Enabled ^= true;
        }

        private void CheckBox1_CheckStateChanged(object sender, EventArgs e)
        {
            checkBox1.CheckState ^= CheckState.Unchecked;
            UpdateAxisLimits();
        }

        private void UpdateAxisLimits()
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            foreach (var series in seriesList)
            {
                series.ManageAxisLimits = checkBox1.Checked;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParametersFromComboBoxes();
        }

        private void ComboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            UpdateParametersFromComboBoxes();
        }

        private void ComboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            m_updatePlotTimer.Stop();
            WorkFlow();
        }


        private void CheckBox2_CheckStateChanged(object sender, EventArgs e)
        {
            m_plt.AxisManager.SetGrid(checkBox2.Checked);
            m_plt.Render();
        }
    }
}
