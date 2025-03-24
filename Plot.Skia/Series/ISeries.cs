using System;

namespace Plot.Skia
{
    public interface ISeries : IDisposable
    {
        IXAxis X { get; }
        IYAxis Y { get; }

        RangeMutable GetXLimit();
        RangeMutable GetYLimit();
        void Render(RenderContext rc);

        Rect GetDataRect(RenderContext rc, IXAxis x, IYAxis y);
    }
}
