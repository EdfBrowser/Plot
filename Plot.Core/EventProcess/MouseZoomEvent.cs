namespace Plot.Core.EventProcess
{
    public class MouseZoomEvent : IPlotEvent
    {
        private readonly Figure m_figure;
        private InputState m_inputState;

        public MouseZoomEvent(Figure m_figure, InputState inputState)
        {
            this.m_figure = m_figure;
            m_inputState = inputState;
        }

        public void Process()
        {

            // 按住shift列缩放，按住control行缩放
            float x = m_inputState.m_shiftPressed ? m_figure.X : m_inputState.m_x;
            float y = m_inputState.m_controlPressed ? m_figure.Y : m_inputState.m_y;

            // 都按住行和列都进行缩放
            //if (m_inputState.m_shiftPressed && m_inputState.m_controlPressed)
            //{
            //    float dx = m_inputState.m_x - m_figure.X;
            //    float dy = m_inputState.m_y - m_figure.Y;
            //    float delta = Math.Max(dx, dy);
            //    x = m_figure.X + delta;
            //    y = m_figure.Y + delta;
            //}

            // TODO: 有两种情况，第一中情况是根据原图像的中心点进行缩放，第二种情况是根据鼠标的位置进行缩放

            // 现在只进行第二种情况
            m_figure.ZoomCenter(x, y);

        }
    }
}
