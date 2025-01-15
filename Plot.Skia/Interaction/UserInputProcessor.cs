using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class UserInputProcessor
    {
        private readonly IFigureControl m_figureControl;
        private readonly object m_lock;
        private readonly IReadOnlyList<IUserActionResponse> m_userActionResponses;

        public UserInputProcessor(IFigureControl figureControl)
        {
            m_figureControl = figureControl;
            m_lock = new object();

            m_userActionResponses = new List<IUserActionResponse>()
            {
                new MouseDragPan(StandardMouseButtons.m_left)
            };
        }


        public void Process(IUserAction userInput)
        {
            bool refreshNeeded
                = ExecuteUserInput(m_figureControl.Figure, userInput);
            if (refreshNeeded)
                m_figureControl.Refresh();
        }

        private bool ExecuteUserInput(Figure figure, IUserAction userInput)
        {
            bool refreshNeeded = false;

            lock (m_lock)
            {
                foreach (IUserActionResponse response in m_userActionResponses)
                {
                    refreshNeeded = response.Execute(figure, userInput);
                }
            }

            return refreshNeeded;
        }
    }
}
