using System.Drawing;

namespace Plot.Core
{
    internal class PlotSignalSeries
    {
        public PlotSignalSeries(Axis axisX, Axis axisY, Color lineColor,
            float lineWidth, SeriesPoint[] data = null)
        {
            AxisX = axisX;
            AxisY = axisY;
            LineColor = lineColor;
            LineWidth = lineWidth;
            Data = data;
        }

        public Axis AxisX { get; set; }
        public Axis AxisY { get; set; }

        public Color LineColor { get; set; }
        public float LineWidth { get; set; }

        public SeriesPoint[] Data { get; set; }

        public Point[] GetScreenPoints()
        {
            Point[] points = new Point[Data.Length];

            for (int i = 0; i < Data.Length; i++)
            {
                points[i] = new Point(Data[i].X, Data[i].Y + AxisY.GetOffsetPixel());
            }

            return points; 
        }
    }

    internal struct SeriesPoint
    {
        public SeriesPoint(int x, int y, object tag = null)
        {
            X = x;
            Y = y;
            Tag = tag;
        }

        public int X;
        public int Y;
        public object Tag;
    }
}
