using Plot.Core;
using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        private int m_fps;
        private int m_channelCount;
        private int m_frameCount = 0;
        private int m_playSpeed;

        private readonly Timer m_updatePlotTimer;
        private readonly double[] m_sine = DataGen.SineAnimated(1_000_0);
        private readonly Figure m_plt;

        private int[] m_indexs;


        public App()
        {
            InitializeComponent();

            Text = "Plot.App";
            m_plt = formPlot1.Figure;

            formPlot1.PltSizeChanged += FormPlot1_PltSizeChanged;
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

        private void ResetXAxis()
        {
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();
            //xAxis.AxisTick.MajorTickVisible = false;
            //xAxis.AxisTick.MinorTickVisible = false;
            //xAxis.AxisTick.TickLabelVisible = false;
            xAxis.AxisTick.TickGenerator.MajorDiv = 1.0;
            xAxis.AxisTick.TickGenerator.LabelFormat = TickLabelFormat.DateTime;
            xAxis.AxisTick.TickGenerator.Rotation = -90;
            xAxis.AxisTick.TickLabelRotation = -90;
            xAxis.AxisTick.HorizontalAlignment = StringAlignment.Far;
            xAxis.AxisTick.VerticalAlignment = StringAlignment.Center;
            xAxis.AxisTick.TickGenerator.MinorDivCount = (int)(1 / 0.25);

            double unit = Measure();
            if (xAxis.AxisTick.TickGenerator.LabelFormat == TickLabelFormat.DateTime)
            {
                DateTime startDateTime = DateTime.MinValue;
                DateTime endDateTime = startDateTime.AddSeconds(unit);
                xAxis.SetDateTimeOrigin(startDateTime);
                xAxis.Dims.SetLimits(startDateTime.ToOADate(), endDateTime.ToOADate());
            }
            else
                xAxis.Dims.SetLimits(0, unit);
        }

        private void ResetYAxes()
        {
            m_plt.AxisManager.ClearYAxes();
            for (int i = 0; i < m_channelCount; i++)
            {
                Axis axis = m_plt.AxisManager.AddAxes(Edge.Left);
                axis.AxisTick.MajorTickVisible = false;
                axis.AxisTick.MinorTickVisible = false;
                axis.AxisTick.TickLabelVisible = false;
                axis.AxisLabel.Label = $"Channel {i}";
                axis.AxisLabel.LabelExtendOutward = false;
                axis.AxisLabel.Rotation = 0;
                axis.AxisLabel.LabelFont = GDI.Font(fontSize: 14);
                axis.AxisLabel.HorizontalAlignment = StringAlignment.Near;
                axis.AxisLabel.VerticalAlignment = StringAlignment.Center;
            }

            m_plt.AxisManager.AxisSpace = 10;
        }

        private void UpdateParametersFromComboBoxes()
        {
            m_playSpeed = int.Parse(comboBox1.SelectedItem.ToString());
            m_fps = int.Parse(comboBox2.SelectedItem.ToString());
            m_channelCount = int.Parse(comboBox3.SelectedItem.ToString());
            m_indexs = new int[m_channelCount];


            m_updatePlotTimer.Interval = 1000 / (m_fps * m_playSpeed);
        }

        private void WorkFlow()
        {
            UpdateParametersFromComboBoxes();

            ResetXAxis();
            ResetYAxes();
            ResetSeries();

            UpdateAxisLimits();
            CheckBox2_CheckStateChanged(null, null);
            m_plt.Render();
        }

        private void App_Load(object sender, EventArgs e)
        {
            WorkFlow();
        }

        private void ResetSeries()
        {
            m_plt.SeriesManager.Clear();
            Axis xAxis = m_plt.AxisManager.GetDefaultXAxis();

            List<Axis> yAxes = m_plt.AxisManager.LeftAxes().ToList();
            foreach (var yAxis in yAxes)
            {
                var series = new StreamerPlotSeries(xAxis, yAxis, 100);
                series.Color = DataGen.randomColor;

                m_plt.SeriesManager.AddSeries(series);
            }
        }

        private long m_count = 0;
        private void UpdatePlot(object sender, EventArgs e)
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            for (int i = 0; i < seriesList.Count; i++)
            {
                int number = seriesList[i].SampleRate / m_fps * m_playSpeed;
                if (m_frameCount % 3 == 0) number += 1;
                m_frameCount++;

                seriesList[i].AddRange(m_sine.Skip(m_indexs[i]).Take(number));
                m_indexs[i] = (m_indexs[i] + number) % m_sine.Length;
                m_count += number;
            }
            if (checkBox1.Checked)
                UpdateAxisLimits();

            m_plt.Render();
        }

        private void UpdateAxisLimits()
        {
            var seriesList = m_plt.SeriesManager.GetStreamerPlotSeries().ToList();
            foreach (var series in seriesList)
            {
                (double lastMinX, double lastMaxX) = series.XAxis.Dims.GetLimits();
                //(double lastMinY, double lastMaxY) = series.YAxis.Dims.GetLimits();
                double seriesMinY = series.Data.Min();
                double seriesMaxY = series.Data.Max();
                double seriesMinX = m_count > series.Data.Length ?
                    (m_count - series.Data.Length) * series.SampleInterval : 0;
                seriesMinX = series.XAxis.Origin.AddSeconds(seriesMinX).ToOADate();
                double seriesMaxX = m_count > series.Data.Length ?
                    m_count * series.SampleInterval : series.Data.Length * series.SampleInterval;
                seriesMaxX = series.XAxis.Origin.AddSeconds(seriesMaxX).ToOADate();

                double xMin = seriesMinX;//astMinX < seriesMinX ? lastMinX : seriesMinX;
                double xMax = seriesMaxX;//lastMaxX > seriesMaxX ? lastMaxX : seriesMaxX;

                double yMin = seriesMinY;// lastMinY < seriesMinY ? lastMinY : seriesMinY;
                double yMax = seriesMaxY;// lastMaxY > seriesMaxY ? lastMaxY : seriesMaxY;

                series.XAxis.Dims.SetLimits(xMin, xMax);
                series.YAxis.Dims.SetLimits(yMin, yMax);

                series.OffsetX = xMin;
            }
        }

        private void FormPlot1_PltSizeChanged(object sender, EventArgs e)
        {
            m_updatePlotTimer.Stop();
            WorkFlow();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            m_updatePlotTimer.Enabled ^= true;
        }

        private void CheckBox1_CheckStateChanged(object sender, EventArgs e)
        {
            checkBox1.CheckState ^= CheckState.Unchecked;
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
