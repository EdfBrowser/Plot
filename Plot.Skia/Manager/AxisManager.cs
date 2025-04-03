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


        internal DefaultGrid DefaultGrid { get; }


        internal void AddYAxis(IYAxis axis) => YAxes.Add(axis);
        internal void AddXAxis(IXAxis axis) => XAxes.Add(axis);

        internal IEnumerable<IAxis> GetAxes(Edge direction)
            => Axes.Where(x => x.Direction == direction);

        internal void GenerateTicks(Rect dataRect)
        {
            foreach (IAxis axis in Axes)
            {
                GenerateTick(axis, dataRect);
            }
        }

        internal void GenerateTick(IAxis axis, Rect dataRect)
        {
            float axisLength = axis.Direction.Horizontal() ? dataRect.Width : dataRect.Height;
            axis.GenerateTicks(axisLength);
        }

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

        internal IAxis HitAxis(PointF p)
        {
            RenderContext rc = m_figure.RenderManager.LastRC;

            IAxis closestAxis = null;
            float minDistance = float.MaxValue;

            foreach (IAxis axis in Axes)
            {
                Rect rect = rc.GetDataRect(axis);

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

        public NumericRightAxis AddNumericRightAxis()
        {
            NumericRightAxis axis = new NumericRightAxis();
            YAxes.Add(axis);
            return axis;
        }

        public NumericBottomAxis AddNumericBottomAxis()
        {
            NumericBottomAxis axis = new NumericBottomAxis();
            XAxes.Add(axis);
            return axis;
        }

        public NumericTopAxis AddNumericTopAxis()
        {
            NumericTopAxis axis = new NumericTopAxis();
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

        public IXAxis DefaultTop
            => XAxes.FirstOrDefault(x => x.Direction == Edge.Top);
        public IXAxis DefaultBottom
            => XAxes.FirstOrDefault(x => x.Direction == Edge.Bottom);
        public IYAxis DefaultLeft
            => YAxes.FirstOrDefault(y => y.Direction == Edge.Left);
        public IYAxis DefaultRight
            => YAxes.FirstOrDefault(z => z.Direction == Edge.Right);

        #endregion
    }
}
