namespace Plot.Skia
{
    internal class LayoutManager
    {
        private readonly Figure m_figure;
        private readonly ILayoutStrategy _strategy;
        private readonly LayoutOptions _options;
        private LayoutResult _layoutResult;

        internal LayoutManager(Figure figure)
        {
            m_figure = figure;

            _options = new LayoutOptions();
            //_strategy = new StackedLayoutStrategy(_options);
            _strategy = new LayeredLayoutStrategy(_options);
        }

        internal Rect CalculateLayout(Rect figureRect)
        {
            _layoutResult = _strategy.CalculateLayout(m_figure, figureRect);

            return _layoutResult.DataRect;
        }

        internal Rect GetDataRect(IAxis axis)
        {
            return _layoutResult.Axes[axis];
        }

        internal Rect GetDataRect(IPanel panel)
        {
            return _layoutResult.Panels[panel];
        }

        internal Rect GetInfo(IPanel panel)
        {
            return _layoutResult.Panels[panel];
        }
    }
}
