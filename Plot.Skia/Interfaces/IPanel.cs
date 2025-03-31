using System;

namespace Plot.Skia
{
    public interface IPanel : IRenderable, IMeasureable, IDisposable
    {
        Edge Direction { get; }
        float PanelSpace { get; set; }
    }
}
