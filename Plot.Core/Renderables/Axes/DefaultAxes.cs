using Plot.Core.Enum;

namespace Plot.Core.Renderables.Axes
{
    public class TopAxis : Axis
    {
        public TopAxis() : base(Edge.Top)
        {
            AxisTick.Visible = false;
            AxisLabel.Visible = false;
            AxisLine.Visible = true;
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
            AxisTick.Visible = false;
            AxisLabel.Visible = false;
            AxisLine.Visible = true;
        }
    }
}
