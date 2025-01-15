using Plot.Skia.Interaction;

namespace Plot.Skia
{
    public readonly struct MouseWheelUp : IMouseButtonAction
    {
        private readonly PointF m_point;

        public MouseWheelUp(PointF point)
        {
            m_point = point;
        }

        public string ButtonName => StandardMouseButtons.m_wheel;
        public PointF Point => m_point;
        public bool Pressed => false;
    }
}
