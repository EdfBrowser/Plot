namespace Plot.Skia
{
    internal class LayoutManager
    {
        private readonly Figure m_figure;
        private readonly ILayout m_layout;

        internal LayoutManager(Figure figure)
        {
            m_figure = figure;

            m_layout = new AutomaticLayout();
        }

        internal ILayout Layout => m_layout;
    }
}
