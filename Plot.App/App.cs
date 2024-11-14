using System;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class App : Form
    {
        public App()
        {
            InitializeComponent();

            Text = "Plot.App";


            PlotSignalBtn.Click += PlotSignalBtn_Click;
            PlotLineBtn.Click += PlotLineBtn_Click;

            nud_sec.ValueChanged += nud_sec_ValueChanged;
        }

        readonly int m_sampleRate = 500;

        private void nud_sec_ValueChanged(object sender, EventArgs e)
        {
            int pointCount = (int)(nud_sec.Value * m_sampleRate);
            label2.Text = string.Format("{0:0.00} thousand data points", pointCount / 1000.0);
            label3.Text = string.Format("{0:0.00} minutes of data", pointCount / m_sampleRate / 60.0);
        }

        private void PlotSignalBtn_Click(object sender, EventArgs e)
        {
            plot1.AddSignal((int)nud_sec.Value, m_sampleRate);

            plot1.Render();
        }

        private void PlotLineBtn_Click(object sender, EventArgs e)
        {
            plot1.AddPlotLine();

            plot1.Render();
        }
    }
}
