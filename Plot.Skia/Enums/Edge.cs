namespace Plot.Skia
{
    internal enum Edge : byte
    {
        Left, Right, Top, Bottom
    }

    internal static class EdgeExt
    {
        internal static bool Vertical(this Edge edge)
        {
            return (edge == Edge.Left || edge == Edge.Right);
        }
        internal static bool Horizontal(this Edge edge)
        {
            return (edge == Edge.Top || edge == Edge.Bottom);
        }
    }
}
