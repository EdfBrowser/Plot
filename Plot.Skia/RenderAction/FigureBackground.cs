namespace Plot.Skia
{
    internal class FigureBackground : IRenderAction
    {
        public void Render(RenderContext rc)
            => rc.Figure.BackgroundManager.FigureBackground.Render(rc.Canvas, rc.Layout.FigurePanel);
    }
}
