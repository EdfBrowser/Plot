namespace Plot.Skia
{
    public abstract class BasePanel : IPanel
    {
        protected BasePanel(Edge direction)
        {
            Direction = direction;
            Width = 30f;
        }

        public Edge Direction { get; }
        public float Width { get; set; }

        public abstract float Measure();
        public abstract void Render(RenderContext rc);
        public abstract void Dispose();

        public Rect GetDataRect(Rect dataRect, float delta, float size)
        {
            return Direction.Vertical()
                ? GetVerticalRect(dataRect, delta, size)
                : GetHorizontalRect(dataRect, delta, size);
        }

        private Rect GetHorizontalRect(Rect dataRect, float delta, float size)
        {
            float left = dataRect.Left;
            float right = dataRect.Right;
            float top = dataRect.Top;
            float bottom = dataRect.Bottom;

            return Direction == Edge.Top
                ? new Rect(left, right, top - Width - delta, top - delta)
                : new Rect(left, right, bottom + delta, bottom + Width + delta);
        }

        private Rect GetVerticalRect(Rect dataRect, float delta, float size)
        {
            float left = dataRect.Left;
            float right = dataRect.Right;
            float top = dataRect.Top;
            float bottom = dataRect.Bottom;

            return Direction == Edge.Left
                ? new Rect(left - Width - delta, left - delta, top, bottom)
                : new Rect(right + delta, right + delta + Width, top, bottom);
        }
    }
}
