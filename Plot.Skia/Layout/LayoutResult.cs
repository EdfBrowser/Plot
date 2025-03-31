using System.Collections.Generic;

namespace Plot.Skia
{
    internal class LayoutResult
    {
        internal LayoutResult(Rect dataRect,
            IReadOnlyDictionary<IAxis, Rect> axes,
            IReadOnlyDictionary<IPanel, Rect> panels)
        {
            DataRect = dataRect;
            Axes = axes;
            Panels = panels;
        }

        internal Rect DataRect { get; }
        internal IReadOnlyDictionary<IAxis, Rect> Axes { get; }
        internal IReadOnlyDictionary<IPanel, Rect> Panels { get; }
    }
}
