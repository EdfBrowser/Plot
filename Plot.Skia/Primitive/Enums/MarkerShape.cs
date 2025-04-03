using System;

namespace Plot.Skia
{
    public enum MarkerShape : byte
    {
        None, FilledCircle, OpenCircle
    }

    internal static class MarkerShapeExtensions
    {
        internal static IMarkerShape GetMarker(this MarkerShape shape)
        {
            switch (shape)
            {
                case MarkerShape.FilledCircle:
                    return new FilledCircle();
                case MarkerShape.OpenCircle:
                    return new OpenCircle();
                default: throw new NotImplementedException(shape.ToString());
            };
        }
    }
}
