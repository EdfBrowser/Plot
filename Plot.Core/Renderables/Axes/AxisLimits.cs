namespace Plot.Core.Renderables.Axes
{
    public struct AxisLimits
    {
        internal double m_xMin;
        internal double m_xMax;
        internal double m_yMin;
        internal double m_yMax;

        public AxisLimits(double xMin, double xMax, double yMin, double yMax)
        {
            (m_xMin, m_xMax, m_yMin, m_yMax) = (xMin, xMax, yMin, yMax);
        }
    }
}
