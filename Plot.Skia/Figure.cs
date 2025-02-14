using SkiaSharp;
using System;

namespace Plot.Skia
{
    public class Figure : IDisposable
    {
        private readonly AxisManager m_axisManager;
        private readonly RenderManager m_renderManager;
        private readonly LayoutManager m_layoutManager;
        private readonly BackgroundManager m_backgroundManager;
        private readonly SeriesManager m_seriesManager;
        private readonly PanelManager m_panelManager;

        public Figure()
        {
            m_axisManager = new AxisManager(this);
            m_renderManager = new RenderManager(this);
            m_layoutManager = new LayoutManager(this);
            m_backgroundManager = new BackgroundManager(this);
            m_seriesManager = new SeriesManager(this);
            m_panelManager = new PanelManager(this);
        }

        internal LayoutManager LayoutManager => m_layoutManager;

        public RenderManager RenderManager => m_renderManager;
        public AxisManager AxisManager => m_axisManager;
        public BackgroundManager BackgroundManager => m_backgroundManager;
        public SeriesManager SeriesManager => m_seriesManager;
        public PanelManager PanelManager => m_panelManager;

        public IFigureControl FigureControl { get; set; }

        public void Dispose()
        {
            AxisManager.Dispose();
            BackgroundManager.Dispose();
            SeriesManager.Dispose();
            PanelManager.Dispose();
        }

        public void Render(SKSurface s)
            => m_renderManager.Render(s.Canvas, s.Canvas.LocalClipBounds.ToRect());
    }
}
