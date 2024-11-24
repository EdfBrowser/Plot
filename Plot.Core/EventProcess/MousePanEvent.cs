namespace Plot.Core.EventProcess
{
    public class MousePanEvent : IPlotEvent
    {
        private readonly Figure m_figure;
        private InputState m_inputState;

        public MousePanEvent(Figure figure, InputState inputState)
        {
            m_figure = figure;
            m_inputState = inputState;
        }

        public void Process()
        {
            float x = m_inputState.m_shiftPressed ? m_figure.X : m_inputState.m_x;
            float y = m_inputState.m_controlPressed ? m_figure.Y : m_inputState.m_y;

            m_figure.PanAll(x, y);
        }
    }
}
