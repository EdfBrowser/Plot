namespace Plot.Skia
{
    internal class DataBackground : IRenderAction
    {
        public void Render(RenderContext rc)
            => rc.Figure.BackgroundManager.DataBackground.Render(rc.Canvas, rc.Layout.DataPanel);
    }
}
