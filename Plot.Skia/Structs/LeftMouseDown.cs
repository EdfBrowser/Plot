using Plot.Skia.Interaction;

namespace Plot.Skia
{
    public readonly struct LeftMouseDown : IMouseButtonAction
    {
        private readonly PointF m_point;

        public LeftMouseDown(PointF point)
        {
            m_point = point;
        }

        public string ButtonName => StandardMouseButtons.m_left;
        public PointF Point => m_point;
        public bool Pressed => true;
    }
}
