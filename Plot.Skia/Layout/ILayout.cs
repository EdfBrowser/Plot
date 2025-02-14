using System.Collections.Generic;

namespace Plot.Skia
{
    internal interface ILayout
    {
        (Rect, Dictionary<IAxis, (float, float)>) GetLayout(Figure figure, Rect figureRect);
    }
}
