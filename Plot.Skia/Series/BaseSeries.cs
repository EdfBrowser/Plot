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

    }
}
