using System.Collections.Generic;

namespace Plot.Skia
{
    public readonly struct Layout
    {
        private readonly PixelPanel m_figurePanel;
        private readonly PixelPanel m_dataPanel;

        private readonly Dictionary<IAxis, (float, float)> m_panelDeltas;
        private readonly Dictionary<IAxis, float> m_panelMargins;

        internal Layout(PixelPanel figurePanel, PixelPanel dataPanel,
            Dictionary<IAxis, (float, float)> panelDeltas, Dictionary<IAxis, float> panelMargins)
        {
            m_figurePanel = figurePanel;
            m_dataPanel = dataPanel;
            m_panelDeltas = panelDeltas;
            m_panelMargins = panelMargins;
        }

        internal PixelPanel FigurePanel => m_figurePanel;
        public PixelPanel DataPanel => m_dataPanel;
        public Dictionary<IAxis, (float, float)> PanelDeltas => m_panelDeltas;
        public Dictionary<IAxis, float> PanelMargins => m_panelMargins;
    }
}
