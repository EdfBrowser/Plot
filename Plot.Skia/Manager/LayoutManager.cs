using System;

namespace Plot.Skia
{
    internal class LayoutManager
    {
        private readonly Figure _figure;
        private readonly ILayoutStrategy _strategy;
        private readonly LayoutOptions _options;
        private LayoutResult _layoutResult;

        internal LayoutManager(Figure figure)
        {
            _figure = figure;

            _options = new LayoutOptions();
            _strategy = new StackedLayoutStrategy(figure, _options);
            //_strategy = new LayeredLayoutStrategy(_options);
        }

        internal void CalculateLayout(Rect figureRect)
        {
            _layoutResult = _strategy.CalculateLayout(figureRect);
        }

        internal Rect GetDataRect(IAxis axis)
        {
            return _layoutResult.Axes[axis];
        }

        internal Rect GetDataRect(IPanel panel)
        {
            return _layoutResult.Panels[panel];
        }

        internal Rect GetDataRect()
        {
            return _layoutResult.DataRect;
        }
    }
}
