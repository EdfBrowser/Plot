using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class LayoutManager
    {
        private readonly Figure m_figure;

        internal LayoutManager(Figure figure)
        {
            m_figure = figure;
        }


        internal void Layout(PixelPanel pixelPanel)
        {
            foreach (IXAxis axis in m_figure.AxisManager.XAxes)
            {
                axis.GenerateTicks(pixelPanel.Width);
            }

            foreach (IYAxis axis in m_figure.AxisManager.YAxes)
            {
                axis.GenerateTicks(pixelPanel.Height);
            }

            Dictionary<IAxis, float> measuredAxes = Measure(m_figure.AxisManager.Axes);

            PixelPanel paddingNeeded = new PixelPanel(
                left: m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Left).Select(x => measuredAxes[x]).Sum(),
                right: m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Right).Select(x => measuredAxes[x]).Sum(),
                bottom: m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Bottom).Select(x => measuredAxes[x]).Sum(),
                top: m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Top).Select(x => measuredAxes[x]).Sum());

            float dataRectWidth = Math.Max(0, pixelPanel.Width - paddingNeeded.Horizontal);
            float dataRectHeight = Math.Max(0, pixelPanel.Height - paddingNeeded.Vertical);

            Coordinate location = new Coordinate(paddingNeeded.Left,paddingNeeded.Top);
            PixelPanel dataRect = new PixelPanel(location, dataRectWidth, dataRectHeight);

        }


        internal static Dictionary<IAxis, float> Measure(IEnumerable<IAxis> axes)
        {
            Dictionary<IAxis, float> measuredAxes = new Dictionary<IAxis, float>();

            foreach (IAxis axis in axes)
            {
                measuredAxes[axis] = axis.Measure();
            }

            return measuredAxes;
        }
    }
}
