namespace Plot.Skia
{
    public interface IRenderable
    {
        Edge Direction { get; }
        void Render(RenderContext rc);
        void Render(RenderContext rc, Rect dataRect);
    }
}
