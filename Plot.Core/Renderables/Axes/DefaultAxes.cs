using Plot.Core.Enum;

namespace Plot.Core.Renderables.Axes
{
    public class TopAxis : Axis
    {
        public TopAxis() : base(Edge.Top)
        {
            Tick(false);
            Label(false);
            Line(true);
        }
    }

    public class BottomAxis : Axis
    {
        public BottomAxis() : base(Edge.Bottom)
        {

        }
    }

    public class LeftAxis : Axis
    {
        public LeftAxis() : base(Edge.Left)
        {

        }
    }

    public class RightAxis : Axis
    {
        public RightAxis() : base(Edge.Right)
        {
            Tick(false);
            Label(false);
            Line(true);
        }
    }
}
