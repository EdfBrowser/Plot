namespace Plot.Skia
{
    public interface IRenderable
    {
        void Render(RenderContext rc);
        void Render(RenderContext rc, Rect dataRect);
    }
}
