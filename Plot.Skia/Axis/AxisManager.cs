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

        private void GenerateTicks(float axisLength, IAxis axis)
            => axis.GenerateTicks(axisLength);


        private void SetLimitsX(Range limit, IXAxis axis)
            => axis.RangeMutable.Set(limit.Low, limit.High);

        private void SetLimitsY(Range limit, IYAxis axis)
            => axis.RangeMutable.Set(limit.Low, limit.High);

        internal void PanMouse(
            float pixelDeltaX, float pixelDeltaY, Rect dataRect)
        {
            foreach (IAxis axis in Axes)
            {
                if (axis.Direction.Horizontal())
                    PanMouse(axis, pixelDeltaX, dataRect.Width);
                else
                    PanMouse(axis, pixelDeltaY, dataRect.Height);
            }
        }

        private void PanMouse(IAxis axis, float delta, float axisLength)
        {
            double pxPerUnit = axisLength / axis.RangeMutable.Span;
            double units = delta / pxPerUnit;
            axis.RangeMutable.Pan(units);
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
