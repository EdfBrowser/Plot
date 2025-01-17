using System;

namespace Plot.Skia
{
    // TODO: ISeries是否需要一个GetDataRect来限制？
    public interface ISeries : IDisposable
    {
        IXAxis X { get; }
        IYAxis Y { get; }

        RangeMutable GetXLimit();
        RangeMutable GetYLimit();
        void Render(RenderContext rc);
    }
}
