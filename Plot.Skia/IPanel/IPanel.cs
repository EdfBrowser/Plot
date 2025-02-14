using System;

namespace Plot.Skia
{
    // TODO: 起别的名字
    internal interface IPanel : IDisposable
    {
        Edge Direction { get; }
        float Measure();
        void Render(RenderContext rc);

        Rect GetDataRect(Rect dataRect, float delta, float size);
    }
}
