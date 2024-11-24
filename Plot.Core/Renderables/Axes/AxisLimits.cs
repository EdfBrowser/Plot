namespace Plot.Core.Renderables.Axes
{
    public struct AxisLimits
    {
        public double m_xMin;
        public double m_xMax;
        public double m_yMin;
        public double m_yMax;

        public double m_xSpan => m_xMax - m_xMin;
        public double m_ySpan => m_yMax - m_yMin;
        public double m_xCenter => m_xSpan / 2.0;
        public double m_yCenter => m_ySpan / 2.0;


        public AxisLimits(double xMin, double xMax, double yMin, double yMax)
        {
            (m_xMin, m_xMax, m_yMin, m_yMax) = (xMin, xMax, yMin, yMax);
        }
    }
}
