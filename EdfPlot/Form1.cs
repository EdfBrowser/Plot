using System.Collections.Generic;
using System;
using System.Windows.Forms;
using Plot.Skia;
using Parser;
using System.Threading.Tasks;

namespace EdfPlot
{
    public partial class Form1 : Form
    {
        private readonly Timer m_timer;
        private readonly Timer m_updateTimer;
        private readonly SignalSeries m_sig1;
        private readonly double[] _buf;

        private readonly Reader _reader;

        private const string _edfFilePath = @"D:\code\X.edf";

        public Form1()
        {
            InitializeComponent();

            _buf = new double[500];

            _reader = new Reader(_edfFilePath);

            var axisManager = figureForm1.Figure.AxisManager;

            m_timer = new Timer();
            m_timer.Tick += Timer_Tick;
            m_timer.Interval = 500;

            m_updateTimer = new Timer();
            m_updateTimer.Tick += RefreshPlot;
            m_updateTimer.Interval = 500;

            //axisManager.Remove(Edge.Bottom);
            //axisManager.AddDateTimeBottomAxis();

            var m_x = axisManager.DefaultBottom;
            IYAxis y = axisManager.DefaultLeft;

            m_x.ScrollMode = AxisScrollMode.Stepping;

            var seriesManager = figureForm1.Figure.SeriesManager;
            m_sig1 = seriesManager.AddSignalSeries(m_x, y, new double[3500], 1.0 / 500);

            m_timer.Start();
            m_updateTimer.Start();
        }

        private void RefreshPlot(object sender, EventArgs e)
        {
            var source = (SignalSouceDouble)m_sig1.SignalSource;
            m_sig1.X.ScrollPosition = source.CurIndex * source.SampleInterval;
            figureForm1.Refresh();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            var source = (SignalSouceDouble)m_sig1.SignalSource;
            _reader.ReadDataAsync(0, _buf).GetAwaiter().GetResult();

            source.AddRange(_buf);
        }
    }
}
