namespace Plot.Skia
{
    public readonly struct MouseMove : IMouseAction
    {
        private readonly PointF m_point;

        public MouseMove(PointF point)
        {
            m_point = point;
        }

        public PointF Point => m_point;
    }
}
