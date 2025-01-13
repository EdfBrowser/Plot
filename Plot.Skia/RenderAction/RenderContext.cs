using SkiaSharp;
using System.Collections.Generic;

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
        internal PixelPanel DataPanel { get; private set; }
        internal PixelPanel ScaledFigurePanel { get; private set; }
        internal Dictionary<IAxis, PixelPanel> AxisPanel { get; private set; }

        internal void Layout()
        {
            ScaledFigurePanel = new PixelPanel(
                left: FigurePanel.Left / Figure.ScaleFactor,
                right: FigurePanel.Right / Figure.ScaleFactor,
                top: FigurePanel.Top / Figure.ScaleFactor,
                bottom: FigurePanel.Bottom / Figure.ScaleFactor);

            (DataPanel, AxisPanel) = Figure.LayoutManager.Layout(ScaledFigurePanel);
        }
    }
}
