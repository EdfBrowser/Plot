
using System;

namespace Plot.Skia
{
    public class BackgroundManager : IDisposable
    {
        private readonly Figure m_figure;

        internal BackgroundManager(Figure figure)
        {
            m_figure = figure;
            FigureBackground = new BackgroundStyle();
            DataBackground = new BackgroundStyle();
        }


        public BackgroundStyle FigureBackground { get; }
        public BackgroundStyle DataBackground { get; }

        public void Dispose()
        {
            FigureBackground.Dispose();
            DataBackground.Dispose();
        }
    }
}
