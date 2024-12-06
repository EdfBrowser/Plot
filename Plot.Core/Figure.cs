using Plot.Core.Draws;
using Plot.Core.EventProcess;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Drawing;

namespace Plot.Core
{
    // TODO: 将结构体作为参数的方法修改为普通参数
    // TODO: 将float改为double类型
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
                }

                m_oldBitmaps[m_currentIndex] = m_bmp;
                m_currentIndex = (m_currentIndex + 1) % m_oldBitmaps.Length;
            }

            public Bitmap GetLatestBitmap => m_bmp;

            public bool CreateBitmap(int width, int height)
            {
                // 旧的bitmap
                if (m_bmp != null)
                {
                    // if the size don`t changed, return immediately
                    if (m_bmp.Width == width && m_bmp.Height == height) return false;

                    StoreBitmap();
                }

                m_bmp = new Bitmap(width, height);
                return true;
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
        }

        // TODO: Draw Title
        public string LabelTitle { get; set; }

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;

        private void OnMouseEventCompleted(object sender, EventArgs e) => Render();
      
        private void RenderFigureArea(Bitmap bmp, bool lowQuality, float scale)
        {
            PlotDimensions dims = CreateDefaultXYPlotDimensions(scale);
            Color figureColor = Color.White;
            // clear and set the background of figure
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                gfx.Clear(figureColor);
            }
        }

        private void RenderDataArea(Bitmap bmp, bool lowQuality, float scale)
        {
            PlotDimensions dims = CreateDefaultXYPlotDimensions(scale);
            Color dataAreaColor = Color.White;
            Color boundaryColor = Color.Gray;
            float boundaryWidth = 1f;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor))
            using (var pen = GDI.Pen(boundaryColor, boundaryWidth))
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                gfx.FillRectangle(brush,
                    dims.m_plotOffsetX + 1,
                    dims.m_plotOffsetY + 1,
                    dims.m_dataWidth - 1,
                    dims.m_dataHeight - 1);

                gfx.DrawRectangle(pen,
                    dims.m_dataOffsetX,
                    dims.m_dataOffsetY,
                    dims.m_dataWidth,
                    dims.m_dataHeight);
            }
        }

        private PlotDimensions CreateDefaultXYPlotDimensions(float scale)
         => m_axisManager.GetDefaultXAxis().CreatePlotDimensions(m_axisManager.GetDefaultYAxis(), scale);

        public Bitmap GetLatestBitmap() => m_bitmapManager.GetLatestBitmap;

        public void Resize(float width, float height)
        {
            if (width < 10 || height < 10) return;

            bool changed = m_bitmapManager.CreateBitmap((int)width, (int)height);
            if (changed)
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
            // TODO: 当连续绘制时，只需要绘制必要部分
            m_seriesManager.RenderSeries(bmp, lowQuality, scale);

            // 如果此时最新的bitmap不是bmp所持有的，不进行更新
            // TODO: 采用一种机制来检测是不是最新的，不是最新的中断后重新渲染
            if (bmp == m_bitmapManager.GetLatestBitmap)
                OnBitmapUpdated?.Invoke(null, null);
        }

        public AxisManager AxisManager => m_axisManager;

        public SeriesManager SeriesManager => m_seriesManager;

        public EventManager EventManager => m_eventManager;
    }
}
