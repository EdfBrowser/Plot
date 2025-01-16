using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class AxisManager : IDisposable
    {
        private readonly Figure m_figure;

        internal AxisManager(Figure figure)
        {
            m_figure = figure;

            XAxes = new List<IXAxis>();
            YAxes = new List<IYAxis>();

            IXAxis xPrimary = new NumericBottomAxis();
            IYAxis yPrimary = new NumericLeftAxis();
            XAxes.Add(xPrimary);
            YAxes.Add(yPrimary);

            DefaultGrid = new DefaultGrid(xPrimary, yPrimary);
        }

        internal IList<IXAxis> XAxes { get; }
        internal IList<IYAxis> YAxes { get; }

        internal IEnumerable<IAxis> Axes => XAxes.Cast<IAxis>().Concat(YAxes);

        internal IXAxis Top => XAxes.First(x => x.Direction == Edge.Top);
        internal IXAxis Bottom => XAxes.First(x => x.Direction == Edge.Bottom);
        internal IYAxis Left => YAxes.First(y => y.Direction == Edge.Left);
        internal IYAxis Right => YAxes.First(z => z.Direction == Edge.Right);

        internal DefaultGrid DefaultGrid { get; }


        internal void AddYAxis(IYAxis axis) => YAxes.Add(axis);
        internal void AddXAxis(IXAxis axis) => XAxes.Add(axis);

        internal IEnumerable<IAxis> GetAxes(Edge direction)
            => Axes.Where(x => x.Direction == direction);

        internal void GenerateTicks(Rect dataRect,
            Dictionary<IAxis, (float, float)> axesInfo)
        {
            foreach (IAxis axis in Axes)
            {
                (float delta, float size) = axesInfo[axis];
                Rect rect = axis.GetDataRect(dataRect, delta, size);

                if (axis.Direction.Horizontal())
                    GenerateTicks(rect.Width, axis);
                else
                    GenerateTicks(rect.Height, axis);
            }
        }

        internal void GenerateTicks(float xAxisLength, float yAxisLength)
        {
            foreach (IAxis axis in Axes)
            {
                if (axis.Direction.Horizontal())
                    GenerateTicks(xAxisLength, axis);
                else
                    GenerateTicks(yAxisLength, axis);
            }
        }

        internal void GenerateTicks(float axisLength, IAxis axis)
            => axis.GenerateTicks(axisLength);


        internal void SetLimitsX(Range limit, IXAxis axis)
            => axis.RangeMutable.Set(limit.Low, limit.High);

        internal void SetLimitsY(Range limit, IYAxis axis)
            => axis.RangeMutable.Set(limit.Low, limit.High);

        internal void PanMouse(IAxis axis, float delta, float axisLength)
        {
            double pxPerUnit = axisLength / axis.RangeMutable.Span;
            double units = delta / pxPerUnit;
            axis.RangeMutable.Pan(units);
        }

        internal void ZoomMouse(IAxis axis, float delta, float axisLength)
        {
            double frac = delta / (Math.Abs(delta) + axisLength);
            double pow = Math.Pow(10, frac);
            axis.RangeMutable.Zoom(pow, axis.RangeMutable.Center);
        }

        internal void ZoomMouse(
            IAxis axis, double frac, float px, Rect dataRect)
        {
            double unit = axis.GetWorld(px, dataRect);
            axis.RangeMutable.Zoom(frac, unit);
        }

        internal IAxis HitAxis(Rect dataRect,
            Dictionary<IAxis, (float, float)> axesInfo, PointF p)
        {
            IAxis closestAxis = null;
            float minDistance = float.MaxValue;

            foreach (IAxis axis in Axes)
            {
                (float delta, float size) = axesInfo[axis];
                Rect rect = axis.GetDataRect(dataRect, delta, size);

                if (rect.Contains(p))
                {
                    float distance = 0;

                    switch (axis.Direction)
                    {
                        case Edge.Bottom:
                            distance = p.Y - rect.Bottom;
                            break;
                        case Edge.Top:
                            distance = rect.Top - p.Y;
                            break;
                        case Edge.Left:
                            distance = rect.Left - p.X;
                            break;
                        case Edge.Right:
                            distance = p.X - rect.Right;
                            break;
                    }

                    if (Math.Abs(distance) < minDistance)
                    {
                        minDistance = Math.Abs(distance);
                        closestAxis = axis;
                    }
                }
            }

            return closestAxis;
        }

        public void Dispose()
        {
            foreach (IAxis axis in Axes)
            {
                axis.Dispose();
            }
        }

        #region PUBLIC
        public void Remove(Edge direction)
        {
            foreach (IAxis axis in GetAxes(direction).ToArray())
            {
                if (axis is IXAxis xAxis)
                    XAxes.Remove(xAxis);
                if (axis is IYAxis yAxis)
                    YAxes.Remove(yAxis);
            }
        }

        public NumericLeftAxis AddNumericLeftAxis()
        {
            NumericLeftAxis axis = new NumericLeftAxis();
            YAxes.Add(axis);
            return axis;
        }

        public NumericBottomAxis AddNumericBottomAxis()
        {
            NumericBottomAxis axis = new NumericBottomAxis();
            XAxes.Add(axis);
            return axis;
        }

        public DateTimeBottomAxis AddDateTimeBottomAxis()
        {
            DateTimeBottomAxis axis = new DateTimeBottomAxis();
            XAxes.Add(axis);
            return axis;
        }

        public void SetLimits(Range limit, IAxis axis)
        {
            if (axis.Direction.Vertical())
                SetLimitsY(limit, (IYAxis)axis);
            else
                SetLimitsX(limit, (IXAxis)axis);
        }

        #endregion
    }
}
