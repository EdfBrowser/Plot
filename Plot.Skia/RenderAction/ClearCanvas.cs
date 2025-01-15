namespace Plot.Skia
{
    internal class ClearCanvas : IRenderAction
    {
        public void Render(RenderContext rc)
            => rc.Canvas.Clear();
    }
}
