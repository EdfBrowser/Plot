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

    public partial class App : Form
    {
        private const int m_fps = 1;
        private readonly Timer m_updatePlotTimer;
        private const int m_channelCount = 16;
        private readonly double[] m_sine = DataGen.SineAnimated(100000);
        private readonly Figure m_plt;
        private readonly int[] m_indexs = new int[m_channelCount];
        private int m_frameCount = 0;
        private int m_playSpeed = 1;

        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_plt = formPlot1.Figure;
            comboBox1.SelectedIndex = 0;
            m_updatePlotTimer = new Timer() { Interval = 1000 / (m_fps * m_playSpeed), Enabled = false };


            button1.Click += Button1_Click;
            checkBox1.CheckStateChanged += CheckBox1_CheckStateChanged;
            comboBox1.SelectedIndexChanged += ComboBox1_SelectedIndexChanged;
            Load += App_Load;
            m_updatePlotTimer.Tick += UpdatePlot;
        }

        private void App_Load(object sender, EventArgs e)
        {
            CreateYAxes();
            CreatePlot();

            m_plt.Render();
        }

        private void CreateYAxes()
        {
            for (int i = 1; i < m_channelCount; i++)
            {
                m_plt.AxisManager.AddAxes(Edge.Left);
            }
        }

        private void CreatePlot()
        {
            DateTime dateTime = new DateTime(2000, 1, 1);
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            xAxis.IsDateTime = true;

            List<Axis> yAxes = m_plt.AxisManager.LeftAxes().ToList();

            for (int i = 0; i < yAxes.Count; i++)
            {
                StreamerPlotSeries series = m_plt.SeriesManager.AddStreamerPlotSeries(xAxis, yAxes[i], 100);
                series.Color = DataGen.randomColor;
                series.OffsetX = dateTime.ToOADate();
            }
        }

        private void UpdatePlot(object sender, EventArgs e)
        {
            List<StreamerPlotSeries> s = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < s.Count; i++)
            {
                int number = s[i].SampleRate / m_fps * m_playSpeed;

                if (m_frameCount % 3 == 0) number += 1;

                s[i].AddRange(m_sine.Skip(m_indexs[i]).Take(number));
                m_indexs[i] = (m_indexs[i] + number) % m_sine.Length;
            }

            m_plt.Render();
            m_frameCount++;
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            m_updatePlotTimer.Enabled ^= true;
        }

        private void CheckBox1_CheckStateChanged(object sender, EventArgs e)
        {
            checkBox1.CheckState ^= CheckState.Unchecked;
            List<StreamerPlotSeries> s = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < s.Count; i++)
            {
                s[i].ManageAxisLimits = checkBox1.Checked;
            }
        }

        private void ComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (comboBox1.SelectedItem.ToString())
            {
                case "1x":
                    m_playSpeed = 1;
                    break;
                case "2x":
                    m_playSpeed = 2;
                    break;
                case "4x":
                    m_playSpeed = 4;
                    break;
                case "8x":
                    m_playSpeed = 8;
                    break;
            }

            m_updatePlotTimer.Interval = 1000 / (m_fps * m_playSpeed);
        }
    }
}
