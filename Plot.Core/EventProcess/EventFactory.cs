namespace Plot.Core.EventProcess
{
    public class EventFactory
    {
        private readonly Figure m_figure;

        public EventFactory(Figure figure)
        {
            m_figure = figure;
        }

        public IPlotEvent CreateMousePanEvent(InputState inputState)
            => new MousePanEvent(m_figure, inputState);

        public IPlotEvent CreateMouseScrollEvent(InputState inputState)
            => new MouseScrollEvent(m_figure, inputState);

        public IPlotEvent CreateMouseZoomEvent(InputState inputState)
            => new MouseZoomEvent(m_figure, inputState);

    }
}
