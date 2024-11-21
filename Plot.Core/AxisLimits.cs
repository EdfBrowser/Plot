namespace Plot.Core
{
    internal struct AxisLimits
    {
        internal double m_xMin;
        internal double m_xMax;
        internal double m_yMin;
        internal double m_yMax;

        internal AxisLimits(double xMin, double xMax, double yMin, double yMax)
        {
            (m_xMin, m_xMax, m_yMin, m_yMax) = (xMin, xMax, yMin, yMax);
        }
    }
}
