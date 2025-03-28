using System.Collections.Generic;

// TODO： 优化事件系统渲染次数
namespace Plot.Skia
{
    public class UserInputProcessor
    {
        private readonly IFigureControl m_figureControl;
        private readonly object m_lock;
        private readonly IReadOnlyList<IUserActionResponse> m_responses;

        public UserInputProcessor(IFigureControl figureControl)
        {
            m_figureControl = figureControl;
            m_lock = new object();

            m_responses = new List<IUserActionResponse>()
            {
                new MouseDragPan(StandardMouseButtons.m_left),
                new MouseDragZoom(StandardMouseButtons.m_right),
                new MouseWheelZoom()
            };
        }

        private Figure Figure => m_figureControl.Figure;

        public void Process(IUserAction userInput)
        {
            bool refreshNeeded = ExecuteUserInput(Figure, userInput);
            if (refreshNeeded)
                m_figureControl.Refresh();
        }

        public void ProcessLostFocus()
        {
            ResetState(Figure);
            m_figureControl.Refresh();
        }

        private void ResetState(Figure figure)
        {
            foreach (IUserActionResponse response in m_responses)
            {
                response.Reset(figure);
            }
        }

        private bool ExecuteUserInput(Figure figure, IUserAction userInput)
        {
            bool refreshNeeded = false;

            lock (m_lock)
            {
                foreach (IUserActionResponse response in m_responses)
                {
                    bool refresh = response.Execute(figure, userInput);
                    if (refresh)
                        refreshNeeded = true;
                }
            }

            return refreshNeeded;
        }
    }
}
