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

        private Dictionary<IAxis, (float, float)> AxesInfo { get; set; }
        private Dictionary<IPanel, (float, float)> PanelsInfo { get; set; }

        internal Figure Figure => m_figure;
        internal SKCanvas Canvas => m_canvas;

        internal Rect ScaledFigureRect { get; private set; }
        internal Rect DataRect { get; private set; }

        internal void CalculateLayout()
        {
            ScaledFigureRect = new Rect(
                left: m_figureRect.Left / Figure.FigureControl.DisplayScale,
                right: m_figureRect.Right / Figure.FigureControl.DisplayScale,
                top: m_figureRect.Top / Figure.FigureControl.DisplayScale,
                bottom: m_figureRect.Bottom / Figure.FigureControl.DisplayScale);

            (DataRect, AxesInfo, PanelsInfo) = Figure.LayoutManager.Layout
                .GetLayout(m_figure, ScaledFigureRect);
        }

        internal Rect GetDataRect(IAxis axis)
        {
            (float delta, float size) = AxesInfo[axis];
            return axis.GetDataRect(DataRect, delta, size);
        }

        internal Rect GetDataRect(IPanel panel)
        {
            (float delta, float size) = PanelsInfo[panel];
            return panel.GetDataRect(DataRect, delta, size);
        }

        internal (float, float) GetInfo(IPanel panel) => PanelsInfo[panel];
    }
}
