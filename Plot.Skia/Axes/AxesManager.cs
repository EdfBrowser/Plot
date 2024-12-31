using System.Collections.Generic;

namespace Plot.Skia.Axes
{
    internal class AxesManager
    {
        private Figure m_figure;
        public AxesManager(Figure figure)
        {
            m_figure = figure;
        }

        internal IList<IXAxis> XAxes { get; }
        internal IList<IYAxis> YAxes { get; }
    }
}
