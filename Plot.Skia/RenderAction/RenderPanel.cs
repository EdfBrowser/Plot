namespace Plot.Skia
{
    internal class RenderPanel : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (IPanel panel in rc.Figure.PanelManager.Panels)
                panel.Render(rc);
        }
    }
}
