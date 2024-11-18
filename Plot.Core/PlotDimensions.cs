using System.Drawing;

namespace Plot.Core
{
    public struct PlotDimensions
    {
        // figure dimensions (pixel units)
        public float FigureWidth;
        public float FigureHeight;

        // data area dimensions (pixel units)
        public float DataWidth;
        public float DataHeight;
        public float DataOffsetX;
        public float DataOffsetY;

        public PlotDimensions(SizeF figureSize, SizeF dataSize, PointF dataOffset)
        {
            (FigureWidth, FigureHeight) = (figureSize.Width, figureSize.Height);
            (DataWidth, DataHeight) = (dataSize.Width, dataSize.Height);
            (DataOffsetX, DataOffsetY) = (dataOffset.X, dataOffset.Y);
        }

        public float GetPixelX(double position)
        {
            return (float)(DataOffsetX + position);
        }

        public float GetPixelY(double position)
        {
            return (float)(DataOffsetY + position);
        }
    }
}
