using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.EventProcess;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Drawing;

namespace Plot.Core
{
    public class Figure
    {
        #region Bitmap Manager
        private class BitmapManager
        {
            private Bitmap m_bmp;
            private readonly Bitmap[] m_oldBitmaps = new Bitmap[3]; // 固定大小为3的数组
            private int m_currentIndex = 0; // 当前索引

            private void StoreBitmap()
            {
                if (m_oldBitmaps[m_currentIndex] != null)
                {
                    Bitmap bmp = m_oldBitmaps[m_currentIndex];
                    bmp.Dispose();
                    bmp = null;
                }

                m_oldBitmaps[m_currentIndex] = m_bmp;
                m_currentIndex = (m_currentIndex + 1) % m_oldBitmaps.Length;
            }

            public Bitmap GetLatestBitmap => m_bmp;

            public void CreateBitmap(int width, int height)
            {
                // 旧的bitmap
                if (m_bmp != null)
                {
                    // if the size don`t changed, return immediately
                    if (m_bmp.Width == width && m_bmp.Height == height) return;

                    StoreBitmap();
                }

                m_bmp = new Bitmap(width, height);
            }
        }

        #endregion

        private readonly AxisManager m_axisManager;
        private readonly BitmapManager m_bitmapManager;
        private readonly SeriesManager m_seriesManager;
        private readonly EventManager m_eventManager;

        // Monitor?
        private readonly object m_lockObj = new object();

        public Figure()
        {
            m_bitmapManager = new BitmapManager();
            m_seriesManager = new SeriesManager();
            m_axisManager = new AxisManager();
            m_eventManager = new EventManager(m_axisManager);
            m_eventManager.MouseEventCompleted += OnMouseEventCompleted;

            m_axisManager.CreateDefaultAxes();
        }

        public Axis DefaultXAxis => m_axisManager.DefaultXAxis;
        public Axis DefaultYAxis => m_axisManager.DefaultYAxis;

        // TODO: Draw Title
        public string LabelTitle { get; set; }

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;


        public void Resize(float width, float height)
        {
            if (width < 10 || height < 10) return;

            m_bitmapManager.CreateBitmap((int)width, (int)height);
            OnBitmapChanged?.Invoke(null, null);

            Render();
        }

        public void Render(bool lowQuality = false, float scale = 1.0f)
        {
            Bitmap bmp;
            lock (m_lockObj)
            {
                bmp = m_bitmapManager.GetLatestBitmap;
                if (bmp == null) return;
            }

            m_seriesManager.GetLimitFromSeries();
            m_axisManager.Layout(bmp.Width / scale, bmp.Height / scale);

            RenderFigureArea(bmp, lowQuality, scale);
            RenderDataArea(bmp, lowQuality, scale);
            m_axisManager.RenderAxes(bmp, lowQuality, scale);
            m_seriesManager.RenderSeries(bmp, lowQuality, scale);

            // 如果此时最新的bitmap不是bmp所持有的，不进行更新
            // TODO: 采用一种机制来检测是不是最新的，不是最新的中断后重新渲染
            if (bmp == m_bitmapManager.GetLatestBitmap)
                OnBitmapUpdated?.Invoke(null, null);
        }

        private void RenderFigureArea(Bitmap bmp, bool lowQuality, float scale)
        {
            PlotDimensions dims = DefaultXAxis.CreatePlotDimensions(DefaultYAxis, scale);
            Color figureColor = Color.White;
            // clear and set the background of figure
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                gfx.Clear(figureColor);
            }
        }

        private void RenderDataArea(Bitmap bmp, bool lowQuality, float scale)
        {
            PlotDimensions dims = DefaultXAxis.CreatePlotDimensions(DefaultYAxis, scale);
            Color dataAreaColor = Color.White;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor))
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                var dataRect = new RectangleF(
                      x: dims.m_plotOffsetX,
                      y: dims.m_plotOffsetY,
                      width: dims.m_dataWidth,
                      height: dims.m_dataHeight);

                gfx.FillRectangle(brush, dataRect);
            }
        }

        public Axis AddAxes(Edge edge) => m_axisManager.AddAxes(edge);
        public Bitmap GetLatestBitmap() => m_bitmapManager.GetLatestBitmap;



        #region Series

        public StreamerPlotSeries AddStreamerPlotSeries(Axis xAxis, Axis yAxis, int sampleRate) => m_seriesManager.AddStreamerPlotSeries(xAxis, yAxis, sampleRate);
        public SignalPlotSeries AddSignalPlotSeries(Axis xAxis, Axis yAxis) => m_seriesManager.AddSignalPlotSeries(xAxis, yAxis);
        public void ClearSeries() => m_seriesManager.ClearSeries();

        #endregion

        // TODO: 减少结构体的复制
        #region Event
        public void MouseDown(InputState inputState) => m_eventManager.MouseDown(inputState);
        public void MouseUp(InputState inputState) => m_eventManager.MouseUp(inputState);
        public void MouseMove(InputState inputState) => m_eventManager.MouseMove(inputState);
        public void MouseDoubleClick(InputState inputState) => m_eventManager.MouseDoubleClick(inputState);
        public void MouseWheel(InputState inputState) => m_eventManager.MouseScroll(inputState);

        private void OnMouseEventCompleted(object sender, EventArgs e) => Render();
        #endregion
    }
}
