using System.Drawing;

namespace Plot.Core
{
    // TODO: 修改成Class? 
    public struct PlotDimensions
    {
        // figure dimensions (pixel)
        internal float m_figureWidth;
        internal float m_figureHeight;

        // TODO: 修改成更好的方式来管理布局
        // Draw area dimensions (pixel)
        // DataSize = multiple of PlotSize
        internal float m_plotWidth;
        internal float m_plotHeight;
        internal float m_plotOffsetX;
        internal float m_plotOffsetY;

        // Figure size - DataOffset = DataSize
        internal float m_dataWidth;
        internal float m_dataHeight;
        internal float m_dataOffsetX;
        internal float m_dataOffsetY;

        // data limits (units)
        internal float m_xMin;
        internal float m_xMax;
        internal float m_yMin;
        internal float m_yMax;
        internal float m_xSpan;
        internal float m_ySpan;
        internal float m_xCenter;
        internal float m_yCenter;

        internal float m_pxPerUnitX;
        internal float m_pxPerUnitY;
        internal float m_unitsPerPxX;
        internal float m_unitsPerPxY;

        // Reverse axis direction
        internal bool m_isReverseX;
        internal bool m_isReverseY;

        // rendering options
        internal float m_scaleFactor;


        internal PlotDimensions(SizeF figureSize, SizeF dataSize, SizeF plotSize, PointF plotOffset, PointF dataOffset,
            ((float xMin, float xMax), (float yMin, float yMax)) limits,
            float scaleFactor, bool is_reverse_x = false, bool is_reverse_y = false)
        {
            (m_figureWidth, m_figureHeight) = (figureSize.Width, figureSize.Height);
            (m_plotWidth, m_plotHeight) = (dataSize.Width, dataSize.Height);
            (m_plotOffsetX, m_plotOffsetY) = (plotOffset.X, plotOffset.Y);
            (m_dataOffsetX, m_dataOffsetY) = (dataOffset.X, dataOffset.Y);
            (m_dataWidth, m_dataHeight) = (plotSize.Width, plotSize.Height);

            var (xLimits, yLimits) = limits;
            (m_xMin, m_xMax) = (xLimits.xMin, xLimits.xMax);
            (m_yMin, m_yMax) = (yLimits.yMin, yLimits.yMax);
            (m_xSpan, m_ySpan) = (m_xMax - m_xMin, m_yMax - m_yMin);
            (m_xCenter, m_yCenter) = ((m_xMin + m_xMax) / 2, (m_yMin + m_yMax) / 2);
            (m_pxPerUnitX, m_pxPerUnitY) = (m_plotWidth / m_xSpan, m_plotHeight / m_ySpan);
            (m_unitsPerPxX, m_unitsPerPxY) = (m_xSpan / m_plotWidth, m_ySpan / m_plotHeight);

            m_scaleFactor = scaleFactor;
            m_isReverseX = is_reverse_x;
            m_isReverseY = is_reverse_y;
        }

        internal (float xPx, float yPx) GetPixel((float xUnit, float yUnit) unit)
        {
            return (GetPixelX(unit.xUnit), GetPixelY(unit.yUnit));
        }

        internal float GetPixelX(float position)
        {
            return m_isReverseX
                ? (float)(m_plotOffsetX + ((m_xMax - position) * m_pxPerUnitX))
                : (float)(m_plotOffsetX + ((position - m_xMin) * m_pxPerUnitX));
        }

        internal float GetPixelY(float position)
        {
            return m_isReverseY
                ? (float)(m_plotOffsetY + ((m_yMax - position) * m_pxPerUnitY))
                : (float)(m_plotOffsetY + ((position - m_yMin) * m_pxPerUnitY));
        }
    }
}
