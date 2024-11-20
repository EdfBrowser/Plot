namespace Plot.Core
{
    public struct AxisLimits
    {
        public double XMin;
        public double XMax;
        public double YMin;
        public double YMax;

        public AxisLimits(double xMin, double xMax, double yMin, double yMax)
        {
            (XMin, XMax, YMin, YMax) = (xMin, xMax, yMin, yMax);
        }
    }
}
