using Plot.Core.Renderables.Axes;

namespace Plot.Core.EventProcess
{
    public class MousePanEvent : IPlotEvent
    {
        private readonly EventManager m_manager;
        private readonly InputState m_inputState;

        public MousePanEvent(EventManager manager, InputState inputState)
        {
            m_manager = manager;
            m_inputState = inputState;
        }

        public void Process(AxisManager axisManager)
        {
            float x = m_inputState.m_shiftPressed ? m_inputState.m_x : m_manager.OldestX;
            float y = m_inputState.m_controlPressed ? m_inputState.m_y : m_manager.OldestY;

            x -= m_manager.OldestX;
            y -= m_manager.OldestY;
            axisManager.PanAll(x, y);
        }
    }
}
