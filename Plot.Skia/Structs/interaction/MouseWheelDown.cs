
namespace Plot.Skia
{
    public readonly struct MouseWheelDown : IMouseButtonAction
    {
        private readonly PointF m_point;

        public MouseWheelDown(PointF point)
        {
            m_point = point;
        }

        public string ButtonName => StandardMouseButtons.m_wheel;
        public PointF Point => m_point;
        public bool Pressed => true;
    }
}
