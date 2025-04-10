namespace Plot.Skia
{
    internal interface ILayoutStrategy
    {
        LayoutResult CalculateLayout(Rect figureRect);
    }
}
