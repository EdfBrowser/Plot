using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class PanelManager : IDisposable
    {
        private readonly Figure m_figure;

        internal PanelManager(Figure figure)
        {
            m_figure = figure;
            Panels = new List<IPanel>();
        }

        internal IList<IPanel> Panels { get; }

        public ColorBar AddColorBar(IHasColorBar source, Edge direction)
        {
            ColorBar colorBar = new ColorBar(source, direction);
            Panels.Add(colorBar);
            return colorBar;
        }

        public void Dispose()
        {
            foreach (IPanel panel in Panels)
                panel.Dispose();
        }
    }
}
