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


        internal Layout GetLayout(PixelPanel figurePanel)
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

            (float l, float r, float t, float b) =
                CalculateEdgeTickLabel(m_figure.AxisManager.Axes);

            SetAxisSpacing(m_figure.AxisManager.Axes);

            IEnumerable<IAxis> leftAxes
                = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Left);
            IEnumerable<IAxis> rightAxes
                = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Right);
            IEnumerable<IAxis> bottomAxes
                = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Bottom);
            IEnumerable<IAxis> topAxes
                = m_figure.AxisManager.Axes.Where(x => x.Direction == Edge.Top);


            float left = leftAxes.Any()
                ? leftAxes.Select(x => measuredAxes[x]).Max() : l;
            float right = rightAxes.Any()
                ? rightAxes.Select(x => measuredAxes[x]).Max() : r;
            float bottom = bottomAxes.Any()
                ? bottomAxes.Select(x => measuredAxes[x]).Max() : b;
            float top = topAxes.Any()
                ? topAxes.Select(x => measuredAxes[x]).Max() : t;


            float dataRectWidth
                = Math.Max(0, figurePanel.Width - (left + right));
            float dataRectHeight
                = Math.Max(0, figurePanel.Height - (bottom + top));

            PanelSize dataSize = new PanelSize(dataRectWidth, dataRectHeight);
            PointF location = new PointF(left, top);
            PixelPanel dataPanel = new PixelPanel(location, dataSize)
                .WithPan(figurePanel.Left, figurePanel.Top);


            Dictionary<IAxis, (float, float)> panelDeltas
                = ArrangeAxes(m_figure.AxisManager.Axes, dataPanel);

            return new Layout(figurePanel, dataPanel, panelDeltas, measuredAxes);
        }

        private void CalculateOffsets(IEnumerable<IAxis> axes,
            PixelPanel dataPanel, Dictionary<IAxis, (float, float)> panelDeltas)
        {
            int axisCount = axes.Count();
            if (axisCount == 0) return;

            float availableSize, plotSize, totalSpacing, lastOffset = 0;

            foreach (IAxis axis in axes)
            {
                totalSpacing = axis.AxisSpacing * (axisCount - 1);

                if (axis.Direction.Horizontal())
                {
                    availableSize = dataPanel.Width - totalSpacing;
                    plotSize = availableSize / axisCount;
                }
                else
                {
                    availableSize = dataPanel.Height - totalSpacing;
                    plotSize = availableSize / axisCount;
                }

                panelDeltas[axis] = (lastOffset, plotSize);
                lastOffset += plotSize + axis.AxisSpacing;
            }
        }

        private Dictionary<IAxis, (float, float)> ArrangeAxes(
            IEnumerable<IAxis> axes, PixelPanel dataPanel)
        {
            Dictionary<IAxis, (float, float)> panelDeltas = new Dictionary<IAxis, (float, float)>();

            CalculateOffsets(axes.Where(x => x.Direction == Edge.Left),
                dataPanel, panelDeltas);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Right),
                dataPanel, panelDeltas);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Bottom),
                dataPanel, panelDeltas);
            CalculateOffsets(axes.Where(x => x.Direction == Edge.Top),
                dataPanel, panelDeltas);

            return panelDeltas;
        }

        private static Dictionary<IAxis, float> Measure(IEnumerable<IAxis> axes)
        {
            Dictionary<IAxis, float> measuredAxes = new Dictionary<IAxis, float>();

            foreach (IAxis axis in axes)
            {
                measuredAxes[axis] = axis.Measure();
            }

            return measuredAxes;
        }

        private static (float, float, float, float)
            CalculateEdgeTickLabel(IEnumerable<IAxis> axes)
        {

            float left = 0f, right = 0f, top = 0f, bottom = 0f;
            foreach (IAxis axis in axes)
            {
                Tick first = axis.TickGenerator.Ticks.First(t => t.MajorPos);
                Tick last = axis.TickGenerator.Ticks.Last(t => t.MajorPos);

                if (axis.Direction.Horizontal())
                {
                    left = Math.Max(left,
                        axis.TickLabelStyle.Measure(first.Label).Width);
                    right = Math.Max(right,
                        axis.TickLabelStyle.Measure(last.Label).Width);
                }
                else
                {
                    bottom = Math.Max(bottom,
                        axis.TickLabelStyle.Measure(first.Label).Height);
                    top = Math.Max(top,
                        axis.TickLabelStyle.Measure(last.Label).Height);
                }
            }

            return (left, right, top, bottom);
        }

        private static void SetAxisSpacing(IEnumerable<IAxis> axes)
        {
            (float l, float r, float t, float b)
                = CalculateEdgeTickLabel(axes);

            foreach (IAxis axis in axes)
            {
                if (axis.Direction.Horizontal())
                    axis.AxisSpacing = Math.Max(l, r);
                else
                    axis.AxisSpacing = Math.Max(t, b);
            }
        }
    }
}
