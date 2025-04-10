namespace Plot.Skia
{
    internal class DataBackground : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            Rect dataRect = rc.GetDataRect();
            rc.Figure.BackgroundManager.DataBackground.Render(rc.Canvas, dataRect);
        }
    }
}
