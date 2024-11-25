namespace Plot.Core.EventProcess
{
    public class MousePanEvent : IPlotEvent
    {
        private EventManager m_manager;
        private InputState m_inputState;

        public MousePanEvent(EventManager manager, InputState inputState)
        {
            m_manager = manager;
            m_inputState = inputState;
        }

        public void Process()
        {
            float x = m_inputState.m_shiftPressed ? m_manager.X : m_inputState.m_x;
            float y = m_inputState.m_controlPressed ? m_manager.Y : m_inputState.m_y;

            m_manager.PanAll(x, y);
        }
    }
}
