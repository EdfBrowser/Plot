namespace Plot.Core.EventProcess
{
    internal class MousePanEvent : PlotEvent
    {
        private readonly Figure m_figure;
        private InputState m_inputState;

        internal MousePanEvent(Figure figure, InputState inputState)
        {
            m_figure = figure;
            m_inputState = inputState;
        }

        internal override void Process()
        {
            float x = m_inputState.m_shiftPressed ? m_figure.X : m_inputState.m_x;
            float y = m_inputState.m_controlPressed ? m_figure.Y : m_inputState.m_y;

            m_figure.PanAll(x, y);
        }
    }
}
