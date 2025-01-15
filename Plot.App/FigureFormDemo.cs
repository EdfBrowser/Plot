using Plot.Skia;
using System;
using System.Windows.Forms;

namespace Plot.App
{
    public partial class FigureFormDemo : Form
    {
        private readonly Timer m_timer;
        private readonly IXAxis m_x;
        private DateTime m_dt;
        private readonly Random m_random;

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
                //        RangeMutable.DefaultDateTime(DateTime.Now, m_xTotalUnit), m_x);
                //}
            };

            m_timer = new Timer();
            m_timer.Tick += Timer_Tick;
            m_timer.Interval = 500;

            axisManager.Remove(Edge.Bottom);
            m_x = axisManager.AddDateTimeBottomAxis();
            m_x.ScrollMode = AxisScrollMode.Sweeping;

            m_timer.Start();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericLeftAxis();
            //axisManager.AddNumericBottomAxis();
#endif
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            m_dt = m_dt.AddSeconds(m_random.NextDouble());
            m_x.ScrollPosition = m_dt.ToOADate();
            figureForm1.Refresh();
        }
    }
}
