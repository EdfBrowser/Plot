using SkiaSharp;

namespace Plot.Skia
{
    public class RenderContext
    {
        private readonly Figure m_figure;
        private readonly SKCanvas m_canvas;
        private readonly PixelPanel m_figurePanel;

        internal RenderContext(Figure figure, SKCanvas canvas, PixelPanel figurePanel)
        {
            m_figure = figure;
            m_canvas = canvas;
            m_figurePanel = figurePanel;
        }

        internal Figure Figure => m_figure;
        internal SKCanvas Canvas => m_canvas;
        internal PixelPanel FigurePanel => m_figurePanel;
        public Layout Layout { get; private set; }

        internal void CalculateLayout()
        {
            PixelPanel ScaledFigurePanel = new PixelPanel(
                left: FigurePanel.Left / Figure.FigureControl.DisplayScale,
                right: FigurePanel.Right / Figure.FigureControl.DisplayScale,
                top: FigurePanel.Top / Figure.FigureControl.DisplayScale,
                bottom: FigurePanel.Bottom / Figure.FigureControl.DisplayScale);

            Layout = Figure.LayoutManager.GetLayout(ScaledFigurePanel);
        }
    }
}
