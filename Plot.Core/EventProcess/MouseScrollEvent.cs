namespace Plot.Core.EventProcess
{
    internal class MouseScrollEvent : PlotEvent
    {
        private Figure m_figure;
        private InputState m_inputState;

        private float m_frac = 0.15f;

        internal MouseScrollEvent(Figure m_figure, InputState inputState)
        {
            this.m_figure = m_figure;
            m_inputState = inputState;
        }

        internal override void Process()
        {
            float increment = 1.0f + m_frac;
            float decrement = 1.0f - m_frac;


            float xFrac = m_inputState.m_scrollUp ? increment : decrement;
            float yFrac = m_inputState.m_scrollUp ? increment : decrement;

            m_figure.ZoomCenter(xFrac, yFrac, m_figure.X, m_figure.Y);
        }
    }
}
