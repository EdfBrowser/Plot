using Plot.Skia.Interaction;

namespace Plot.Skia
{
    public readonly struct RightMouseDown : IMouseButtonAction
    {
        private readonly PointF m_point;

        public RightMouseDown(PointF point)
        {
            m_point = point;
        }

        public string ButtonName => StandardMouseButtons.m_right;
        public PointF Point => m_point;
        public bool Pressed => true;
    }
}
