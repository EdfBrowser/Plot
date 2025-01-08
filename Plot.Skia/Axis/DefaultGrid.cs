namespace Plot.Skia
{
    internal class DefaultGrid
    {
        private readonly IXAxis m_xPrimary;
        private readonly IYAxis m_yPrimary;

        internal DefaultGrid(IXAxis xPrimary, IYAxis yPrimary)
        {
            m_xPrimary = xPrimary;
            m_yPrimary = yPrimary;
        }

        internal IXAxis XAxis => m_xPrimary;
        internal IYAxis YAxis => m_yPrimary;

    }
}
