using SkiaSharp;

namespace Plot.Skia
{
    public class Figure
    {
        private readonly AxisManager m_axisManager;
        private readonly RenderManager m_renderManager;
        private readonly LayoutManager m_layoutManager;

        public Figure()
        {
            m_axisManager = new AxisManager(this);
            m_renderManager = new RenderManager(this);
            m_layoutManager = new LayoutManager(this);
        }

        public RenderManager RenderManager => m_renderManager;
        public LayoutManager LayoutManager => m_layoutManager;
        public AxisManager AxisManager => m_axisManager;

        public float ScaleFactor { get; set; } = 1f;

        public void Render(SKSurface s)
            => m_renderManager.Render(s.Canvas, s.Canvas.LocalClipBounds.ToPixelPanel());

        public void Render()
        {
            using (SKSurface s = SKSurface.CreateNull(600, 500))
            {
                Render(s);
            }
        }
    }
}
