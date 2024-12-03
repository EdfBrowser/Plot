using Plot.Core.Renderables.Axes;

namespace Plot.Core.EventProcess
{
    public class MouseScrollEvent : IPlotEvent
    {
        private readonly EventManager m_manager;
        private readonly InputState m_inputState;

        private readonly double m_frac = 0.15;

        public MouseScrollEvent(EventManager eventManager, InputState inputState)
        {
            m_manager = eventManager;
            m_inputState = inputState;
        }

        public void Process(AxisManager axisManager)
        {
            double increment = 1.0 + m_frac;
            double decrement = 1.0 - m_frac;


            double xFrac = m_inputState.m_scrollUp ? increment : decrement;
            double yFrac = m_inputState.m_scrollUp ? increment : decrement;

            axisManager.ZoomByFrac(xFrac, yFrac, m_inputState.m_x, m_inputState.m_y);
        }
    }
}
