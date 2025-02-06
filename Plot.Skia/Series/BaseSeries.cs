using System;

namespace Plot.Skia
{
    public abstract class BaseSeries : ISeries
    {
        protected BaseSeries(IXAxis x, IYAxis y)
        {
            X = x;
            Y = y;
        }

        public IXAxis X { get; }
        public IYAxis Y { get; }

        public abstract void Dispose();
        public abstract RangeMutable GetXLimit();
        public abstract RangeMutable GetYLimit();
        public abstract void Render(RenderContext rc);

        public virtual Rect GetDataRect(RenderContext rc, IXAxis x, IYAxis y)
        {
            Rect xRect = rc.GetDataRect(x);
            Rect yRect = rc.GetDataRect(y);

            float left = Math.Max(xRect.Left, yRect.Left);
            float right = Math.Min(xRect.Right, yRect.Right);

            float top = Math.Max(xRect.Top, yRect.Top);
            float bottom = Math.Min(xRect.Bottom, yRect.Bottom);

            Rect unionRect = new Rect(left, right, top, bottom);

            return unionRect;
        }
    }
}
