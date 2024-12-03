using System.Drawing;

namespace Plot.Core
{
    public class PlotDimensions
    {
        // figure dimensions (pixel)
        public float m_figureWidth;
        public float m_figureHeight;

        // TODO: 修改成更好的方式来管理布局
        // Draw area dimensions (pixel)
        // DataSize = multiple of PlotSize
        public float m_plotWidth;
        public float m_plotHeight;
        public float m_plotOffsetX;
        public float m_plotOffsetY;

        // Figure size - DataOffset = DataSize
        public float m_dataWidth;
        public float m_dataHeight;
        public float m_dataOffsetX;
        public float m_dataOffsetY;

        // data limits (units)
        public double m_xMin;
        public double m_xMax;
        public double m_yMin;
        public double m_yMax;
        public double m_xSpan;
        public double m_ySpan;
        public double m_xCenter;
        public double m_yCenter;

        public double m_pxPerUnitX;
        public double m_pxPerUnitY;
        public double m_unitsPerPxX;
        public double m_unitsPerPxY;

        // Reverse axis direction
        public bool m_isReverseX;
        public bool m_isReverseY;

        // rendering options
        public float m_scaleFactor;


        public PlotDimensions(SizeF figureSize, SizeF dataSize, SizeF plotSize, PointF plotOffset, PointF dataOffset,
            ((double, double), (double, double)) limits,
            float scaleFactor, bool is_reverse_x = false, bool is_reverse_y = false)
        {
            (m_figureWidth, m_figureHeight) = (figureSize.Width, figureSize.Height);
            (m_plotWidth, m_plotHeight) = (dataSize.Width, dataSize.Height);
            (m_plotOffsetX, m_plotOffsetY) = (plotOffset.X, plotOffset.Y);
            (m_dataOffsetX, m_dataOffsetY) = (dataOffset.X, dataOffset.Y);
            (m_dataWidth, m_dataHeight) = (plotSize.Width, plotSize.Height);

            ((m_xMin, m_xMax), (m_yMin, m_yMax)) = limits;
            (m_xSpan, m_ySpan) = (m_xMax - m_xMin, m_yMax - m_yMin);
            (m_xCenter, m_yCenter) = ((m_xMin + m_xMax) / 2, (m_yMin + m_yMax) / 2);
            (m_pxPerUnitX, m_pxPerUnitY) = (m_plotWidth / m_xSpan, m_plotHeight / m_ySpan);
            (m_unitsPerPxX, m_unitsPerPxY) = (m_xSpan / m_plotWidth, m_ySpan / m_plotHeight);

            m_scaleFactor = scaleFactor;
            m_isReverseX = is_reverse_x;
            m_isReverseY = is_reverse_y;
        }

        public float GetPixelX(double position)
        {
            return m_isReverseX
                ? (float)(m_plotOffsetX + ((m_xMax - position) * m_pxPerUnitX))
                : (float)(m_plotOffsetX + ((position - m_xMin) * m_pxPerUnitX));
        }

        public float GetPixelY(double position)
        {
            return m_isReverseY
                ? (float)(m_plotOffsetY + ((m_yMax - position) * m_pxPerUnitY))
                : (float)(m_plotOffsetY + ((position - m_yMin) * m_pxPerUnitY));
        }
    }
}
