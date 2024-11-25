namespace Plot.Core.EventProcess
{
    public class MouseScrollEvent : IPlotEvent
    {
        private readonly EventManager m_manager;
        private InputState m_inputState;

        private readonly float m_frac = 0.15f;

        public MouseScrollEvent(EventManager eventManager, InputState inputState)
        {
            m_manager = eventManager;
            m_inputState = inputState;
        }

        public void Process()
        {
            float increment = 1.0f + m_frac;
            float decrement = 1.0f - m_frac;


            float xFrac = m_inputState.m_scrollUp ? increment : decrement;
            float yFrac = m_inputState.m_scrollUp ? increment : decrement;

            m_manager.ZoomCenter(xFrac, yFrac, m_inputState.m_x, m_inputState.m_y);
        }
    }
}
