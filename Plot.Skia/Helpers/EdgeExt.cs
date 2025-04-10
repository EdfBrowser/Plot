namespace Plot.Skia
{
    internal static class EdgeExt
    {
        internal static bool Vertical(this Edge direction)
        {
            return (direction == Edge.Left || direction == Edge.Right);
        }
        internal static bool Horizontal(this Edge direction)
        {
            return (direction == Edge.Top || direction == Edge.Bottom);
        }
    }
}
