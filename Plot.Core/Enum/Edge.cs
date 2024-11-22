namespace Plot.Core.Enum
{
    public enum Edge
    {
        Left, Right, Top, Bottom
    }

    public static class EdgeExtensions
    {
        public static bool IsHorizontal(this Edge edge)
        {
            return edge == Edge.Top || edge == Edge.Bottom;
        }

        public static bool IsVertical(this Edge edge)
        {
            return edge == Edge.Left || edge == Edge.Right;
        }
    }
}
