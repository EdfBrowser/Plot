using Plot.Core.Renderables.Axes;

namespace Plot.Core.EventProcess
{
    public class MouseZoomEvent : IPlotEvent
    {
        private readonly EventManager m_manager;
        private readonly InputState m_inputState;

        public MouseZoomEvent(EventManager eventManager, InputState inputState)
        {
            m_manager = eventManager;
            m_inputState = inputState;
        }

        public void Process(AxisManager axisManager)
        {

            // 按住shift列缩放，按住control行缩放
            float x = m_inputState.m_shiftPressed ? m_inputState.m_x : m_manager.OldestX;
            float y = m_inputState.m_controlPressed ? m_inputState.m_y : m_manager.OldestY;

            // 都按住行和列都进行缩放
            //if (m_inputState.m_shiftPressed && m_inputState.m_controlPressed)
            //{
            //    float dx = m_inputState.m_x - m_figure.OldestX;
            //    float dy = m_inputState.m_y - m_figure.OldestY;
            //    float delta = Math.Max(dx, dy);
            //    x = m_figure.OldestX + delta;
            //    y = m_figure.OldestY + delta;
            //}

            // TODO: 有两种情况，第一中情况是根据原图像的中心点进行缩放，第二种情况是根据鼠标的位置进行缩放

            // 现在只进行第二种情况
            axisManager.ZoomByXY(x - m_manager.OldestX, y - m_manager.OldestY);
        }
    }
}
