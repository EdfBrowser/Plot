namespace Plot.Core.EventProcess
{
    internal class EventFactory
    {
        private readonly Figure m_figure;

        internal EventFactory(Figure figure)
        {
            m_figure = figure;
        }

        internal PlotEvent CreateMousePanEvent(InputState inputState)
            => new MousePanEvent(m_figure, inputState);

        internal PlotEvent CreateMouseScrollEvent(InputState inputState)
            => new MouseScrollEvent(m_figure, inputState);

        internal PlotEvent CreateMouseZoomEvent(InputState inputState)
            => new MouseZoomEvent(m_figure, inputState);

    }
}
