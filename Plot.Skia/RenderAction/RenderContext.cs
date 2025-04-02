using SkiaSharp;

namespace Plot.Skia
{
    public class RenderContext
    {
        private readonly Rect _figureRect;

        internal RenderContext(Figure figure, SKCanvas canvas, Rect figureRect)
        {
            Figure = figure;
            Canvas = canvas;
            _figureRect = figureRect;
        }


        internal Figure Figure { get; }
        internal SKCanvas Canvas { get; }

        internal Rect ScaledFigureRect { get; private set; }
        internal Rect DataRect { get; private set; }

        internal void CalculateLayout()
        {
            CalculateScaledFigureRect();

            DataRect = Figure.LayoutManager.CalculateLayout(ScaledFigureRect);
        }

        private void CalculateScaledFigureRect()
        {
            float scale = Figure.FigureControl.DisplayScale;
            ScaledFigureRect = new Rect(
               left: _figureRect.Left / scale,
               right: _figureRect.Right / scale,
               top: _figureRect.Top / scale,
               bottom: _figureRect.Bottom / scale);
        }

        internal Rect GetDataRect(IAxis axis)
        {
            return Figure.LayoutManager.GetDataRect(axis);
        }

        internal Rect GetDataRect(IPanel panel)
        {
            return Figure.LayoutManager.GetDataRect(panel);
        }
    }
}
