namespace Plot.Skia
{
    public readonly struct RightMouseUp : IMouseButtonAction
    {
        private readonly PointF m_point;

        public RightMouseUp(PointF point)
        {
            m_point = point;
        }

        public string ButtonName => StandardMouseButtons.m_right;
        public PointF Point => m_point;
        public bool Pressed => false;
    }
}
