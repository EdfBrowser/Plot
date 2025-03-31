namespace Plot.Skia
{
    // TODO: layoutOptions,layoutContext
    internal interface ILayoutStrategy
    {
        LayoutResult CalculateLayout(Figure figure, Rect figureRect);
    }
}
