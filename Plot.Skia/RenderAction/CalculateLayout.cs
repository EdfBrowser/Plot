namespace Plot.Skia
{
    internal class CalculateLayout : IRenderAction
    {
        private PixelPanel m_scaledFigurePanel;

        public void Render(RenderContext rc)
        {
            m_scaledFigurePanel = new PixelPanel(
                left: rc.FigurePanel.Left / rc.Figure.ScaleFactor,
                right: rc.FigurePanel.Right / rc.Figure.ScaleFactor,
                top: rc.FigurePanel.Top / rc.Figure.ScaleFactor,
                bottom: rc.FigurePanel.Bottom / rc.Figure.ScaleFactor);

            rc.Figure.LayoutManager.Layout(m_scaledFigurePanel);
        }
    }
}
