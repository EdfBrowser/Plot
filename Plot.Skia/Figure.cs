using Plot.Skia.Axes;
namespace Plot.Skia
{
    internal class Figure
    {
        private AxesManager m_axesManager;

        public Figure()
        {
            m_axesManager = new AxesManager(this);
        }
    }
}
