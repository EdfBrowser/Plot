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


        internal (PixelPanel dataPanel, Dictionary<IAxis, PixelPanel>) Layout(PixelPanel figurePanel)
        {
            foreach (IXAxis axis in m_figure.AxisManager.XAxes)
            {
                axis.GenerateTicks(figurePanel.Width);
            }

            foreach (IYAxis axis in m_figure.AxisManager.YAxes)
            {
                axis.GenerateTicks(figurePanel.Height);
            }

            Dictionary<IAxis, float> measuredAxes = Measure(m_figure.AxisManager.Axes);

            IEnumerable<IAxis> leftAxes = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Left);
            IEnumerable<IAxis> rightAxes = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Right);
            IEnumerable<IAxis> bottomAxes = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Bottom);
            IEnumerable<IAxis> topAxes = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Top);


            PixelPanel paddingNeeded = new PixelPanel(
                left: leftAxes.Any() ? leftAxes.Select(x => measuredAxes[x]).Max() : 10f,
                right: rightAxes.Any() ? rightAxes.Select(x => measuredAxes[x]).Max() : 10f,
                bottom: bottomAxes.Any() ? bottomAxes.Select(x => measuredAxes[x]).Max() : 10f,
                top: topAxes.Any() ? topAxes.Select(x => measuredAxes[x]).Max() : 10f);

            float dataRectWidth = Math.Max(0, figurePanel.Width - paddingNeeded.Hoffset);
            float dataRectHeight = Math.Max(0, figurePanel.Height - paddingNeeded.Voffset);

            PointF location = new PointF(paddingNeeded.Left, paddingNeeded.Top);
            PixelPanel dataPanel = new PixelPanel(location, dataRectWidth, dataRectHeight)
                .WithPan(figurePanel.Left, figurePanel.Top);


            return (dataPanel, ArrangeAxes(m_figure.AxisManager.Axes, dataPanel));
        }

        private void CalculateOffsets(IEnumerable<IAxis> axes,
            PixelPanel dataPanel, Dictionary<IAxis, PixelPanel> offsetedAxes)
        {
            int axisCount = axes.Count();
            if (axisCount == 0) return;

            float totalSpacing = m_figure.AxisSpace * (axisCount - 1);
            float availableSize, plotSize, lastOffset = 0;

            foreach (var axis in axes)
            {
                if (axis.Direction.Horizontal())
                {
                    availableSize = dataPanel.Width - totalSpacing;
                    plotSize = availableSize / axisCount;
                    offsetedAxes[axis] =
                        new PixelPanel(dataPanel.Left + lastOffset,
                        dataPanel.Left + lastOffset + plotSize, dataPanel.Top, dataPanel.Bottom);
                    lastOffset += plotSize;
                }
                else
                {
                    availableSize = dataPanel.Height - totalSpacing;
                    plotSize = availableSize / axisCount;
                    offsetedAxes[axis] =
                        new PixelPanel(dataPanel.Left, dataPanel.Right,
                        dataPanel.Top + lastOffset, dataPanel.Top + lastOffset + plotSize);
                    lastOffset += plotSize;
                }

                lastOffset += m_figure.AxisSpace;
            }
        }

        private Dictionary<IAxis, PixelPanel> ArrangeAxes(IEnumerable<IAxis> axes, PixelPanel dataPanel)
        {
            Dictionary<IAxis, PixelPanel> offsetedAxes = new Dictionary<IAxis, PixelPanel>();

            CalculateOffsets(axes.Where(x => x.Direction == Edge.Left),
                dataPanel, offsetedAxes);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Right),
                dataPanel, offsetedAxes);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Bottom),
                dataPanel, offsetedAxes);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Top),
                dataPanel, offsetedAxes);

            return offsetedAxes;
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
