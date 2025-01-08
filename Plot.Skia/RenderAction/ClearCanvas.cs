namespace Plot.Skia
{
    internal class ClearCanvas : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            if (rc.Figure.RenderManager.ClearCanvasBeforeRendering)
                rc.Canvas.Clear();
        }
    }
}
