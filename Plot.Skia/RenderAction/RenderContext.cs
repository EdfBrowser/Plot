using SkiaSharp;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class RenderContext
    {
        private readonly Figure m_figure;
        private readonly SKCanvas m_canvas;
        private readonly Rect m_figureRect;

        internal RenderContext(Figure figure, SKCanvas canvas, Rect figureRect)
        {
            m_figure = figure;
            m_canvas = canvas;
            m_figureRect = figureRect;
        }

        internal Figure Figure => m_figure;
        internal SKCanvas Canvas => m_canvas;

        internal Rect ScaledFigureRect { get; private set; }
        public Rect DataRect { get; private set; }
        public Dictionary<IAxis, (float, float)> AxesInfo { get; private set; }

        internal void CalculateLayout()
        {
            ScaledFigureRect = new Rect(
                left: m_figureRect.Left / Figure.FigureControl.DisplayScale,
                right: m_figureRect.Right / Figure.FigureControl.DisplayScale,
                top: m_figureRect.Top / Figure.FigureControl.DisplayScale,
                bottom: m_figureRect.Bottom / Figure.FigureControl.DisplayScale);

            (DataRect, AxesInfo)
                = Figure.LayoutManager.GetLayout(ScaledFigureRect);
        }
    }
}
