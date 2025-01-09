namespace Plot.Skia
{
    public class BackgroundManager
    {
        private readonly Figure m_figure;

        internal BackgroundManager(Figure figure)
        {
            m_figure = figure;
            FigureBackground = new BackgroundStyle();
            DataBackground = new BackgroundStyle();
        }


        internal BackgroundStyle FigureBackground { get; }
        internal BackgroundStyle DataBackground { get; }
    }
}
