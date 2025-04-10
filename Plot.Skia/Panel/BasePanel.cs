namespace Plot.Skia
{
    public abstract class BasePanel : IPanel
    {
        protected BasePanel(Edge direction)
        {
            Direction = direction;
        }

        public float Space { get; set; }
        public Edge Direction { get; }

        public abstract float Measure(bool force = false);
        public abstract void Render(RenderContext rc);
        public abstract void Dispose();

        public virtual void Render(RenderContext rc, Rect dataRect) { }
    }
}
