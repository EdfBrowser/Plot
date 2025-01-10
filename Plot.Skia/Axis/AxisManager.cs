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

            IXAxis xPrimary = new BottomAxis();
            IYAxis yPrimary = new LeftAxis();
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

        internal LeftAxis AddLeftAxis()
        {
            LeftAxis axis = new LeftAxis();
            YAxes.Add(axis);
            return axis;
        }

        internal static void SetLimitsX(PixelRange limit, IXAxis axis)
            => axis.Range.Set(limit.Low, limit.High);

        internal static void SetLimitsY(PixelRange limit, IYAxis axis)
            => axis.Range.Set(limit.Low, limit.High);

        public void Dispose()
        {
            foreach (IAxis axis in Axes)
            {
                axis.Dispose();
            }
        }
    }
}
