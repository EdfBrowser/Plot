
namespace Plot.Skia
{
    public abstract class BaseSeries : ISeries
    {
        protected Rect _dataRect;

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

        protected virtual Rect GetDataRect(RenderContext rc)
        {
            Rect xRect = rc.GetDataRect(X);
            Rect yRect = rc.GetDataRect(Y);

            Rect unionRect = new Rect(xRect.Left, xRect.Right, yRect.Top, yRect.Bottom);

            return unionRect;
        }
    }
}
