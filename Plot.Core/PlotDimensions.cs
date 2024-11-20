using System.Drawing;

namespace Plot.Core
{
    public struct PlotDimensions
    {
        // figure dimensions (pixel)
        public float FigureWidth;
        public float FigureHeight;

        // TODO: 修改成更好的方式来管理布局
        // Draw area dimensions (pixel)
        // DataSize = multiple of PlotSize
        public float PlotWidth;
        public float PlotHeight;
        public float PlotOffsetX;
        public float PlotOffsetY;

        // Figure size - DataOffset = DataSize
        public float DataWidth;
        public float DataHeight;
        public float DataOffsetX;
        public float DataOffsetY;

        // data limits (units)
        public float XMin;
        public float XMax;
        public float YMin;
        public float YMax;
        public float XSpan;
        public float YSpan;
        public float XCenter;
        public float YCenter;

        public float PxPerUnitX;
        public float PxPerUnitY;
        public float UnitsPerPxX;
        public float UnitsPerPxY;

        // Reverse axis direction
        public bool IsReverseX;
        public bool IsReverseY;

        // rendering options
        public float ScaleFactor;


        public PlotDimensions(SizeF figureSize, SizeF dataSize, SizeF plotSize, PointF plotOffset, PointF dataOffset,
            ((float xMin, float xMax), (float yMin, float yMax)) limits,
            float scaleFactor, bool is_reverse_x = false, bool is_reverse_y = false)
        {
            (FigureWidth, FigureHeight) = (figureSize.Width, figureSize.Height);
            (PlotWidth, PlotHeight) = (dataSize.Width, dataSize.Height);
            (PlotOffsetX, PlotOffsetY) = (plotOffset.X, plotOffset.Y);
            (DataOffsetX, DataOffsetY) = (dataOffset.X, dataOffset.Y);
            (DataWidth, DataHeight) = (plotSize.Width, plotSize.Height);

            var (xLimits, yLimits) = limits;
            (XMin, XMax) = (xLimits.xMin, xLimits.xMax);
            (YMin, YMax) = (yLimits.yMin, yLimits.yMax);
            (XSpan, YSpan) = (XMax - XMin, YMax - YMin);
            (XCenter, YCenter) = ((XMin + XMax) / 2, (YMin + YMax) / 2);
            (PxPerUnitX, PxPerUnitY) = (PlotWidth / XSpan, PlotHeight / YSpan);
            (UnitsPerPxX, UnitsPerPxY) = (XSpan / PlotWidth, YSpan / PlotHeight);

            ScaleFactor = scaleFactor;
            IsReverseX = is_reverse_x;
            IsReverseY = is_reverse_y;
        }

        public (float xPx, float yPx) GetPixel((float xUnit, float yUnit) unit)
        {
            return (GetPixelX(unit.xUnit), GetPixelY(unit.yUnit));
        }

        public float GetPixelX(float position)
        {
            return IsReverseX
                ? (float)(PlotOffsetX + ((XMax - position) * PxPerUnitX))
                : (float)(PlotOffsetX + ((position - XMin) * PxPerUnitX));
        }

        public float GetPixelY(float position)
        {
            return IsReverseY
                ? (float)(PlotOffsetY + ((YMax - position) * PxPerUnitY))
                : (float)(PlotOffsetY + ((position - YMin) * PxPerUnitY));
        }
    }
}
