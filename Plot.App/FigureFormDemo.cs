using Plot.Skia;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class FigureFormDemo : Form
    {
        private readonly Timer m_timer;
        private readonly Timer m_updateTimer;
        private readonly IXAxis m_x;
        private readonly DateTime m_dt;
        private readonly Random m_random;
        private readonly SignalSeries m_sig1;
        private readonly SignalSeries m_sig2;
        private readonly SignalSeries m_sig3;
        private readonly List<double> m_data = new List<double>();
        private readonly List<double> m_data2 = new List<double>();
        private readonly List<double> m_data3 = new List<double>();

        public FigureFormDemo()
        {
            InitializeComponent();

#if DEBUG
            var axisManager = figureForm1.Figure.AxisManager;
            m_dt = DateTime.Now;
            m_random = new Random(new Random().Next());

            figureForm1.Figure.RenderManager.SizeChangedEventHandler += delegate (object send, RenderContext rc)
            {
                //using (Graphics g = CreateGraphics())
                //{
                //    // 每1英寸=2.54厘米=g.DpiX
                //    double m_xPxPerCM = g.DpiX / 2.54;
                //    // 总共多少厘米
                //    Rect dataRect = rc.DataRect;
                //    (float delta, float size) = rc.AxesInfo[m_x];
                //    double m_xDataAreaTotalCM =
                //    m_x.GetDataRect(dataRect, delta, size).Width / m_xPxPerCM;
                //    double m_xTotalUnit = m_xDataAreaTotalCM / 3.0;

                //    axisManager.SetLimits(
                //        Range.DefaultDateTime(DateTime.Now, m_xTotalUnit), m_x);
                //}
            };

            m_timer = new Timer();
            m_timer.Tick += Timer_Tick;
            m_timer.Interval = 100;

            m_updateTimer = new Timer();
            m_updateTimer.Tick += RefreshPlot;
            m_updateTimer.Interval = 500;

            axisManager.Remove(Edge.Bottom);
            axisManager.AddDateTimeBottomAxis();

            m_x = axisManager.DefaultBottom;
            IYAxis y = axisManager.DefaultLeft;

            m_x.ScrollMode = AxisScrollMode.Scrolling;

            var seriesManager = figureForm1.Figure.SeriesManager;
            m_sig1 = seriesManager.AddSignalSeries(m_x, y, m_data, 1.0 / 1);

            //((DateTimeBottomAxis)(m_x)).SetOriginDateTime(DateTime.Now);


            IYAxis y2 = axisManager.AddNumericLeftAxis();
            IYAxis y3 = axisManager.AddNumericLeftAxis();

            m_sig2 = seriesManager.AddSignalSeries(m_x, y2, m_data2, 1.0 / 1);
            m_sig3 = seriesManager.AddSignalSeries(m_x, y3, m_data3, 1.0 / 1);
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericBottomAxis();

            m_timer.Start();
            m_updateTimer.Start();
#endif
        }

        private void RefreshPlot(object sender, EventArgs e)
        {
            m_x.ScrollPosition = m_data.Count;
            figureForm1.Refresh();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            m_data.Add(m_random.NextDouble());
            m_data2.Add(m_random.NextDouble());
            m_data3.Add(m_random.NextDouble());
        }
    }
}
