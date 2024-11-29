using Plot.Core.Renderables.Axes;
using System;

namespace Plot.Core.EventProcess
{
    public class EventManager
    {
        private readonly AxisManager m_axisManager;

        private bool m_leftPressed = false;
        private bool m_rightPressed = false;
        private bool m_ctrlPressed = false;
        private bool m_shiftPressed = false;
        private bool m_altPressed = false;

        public EventManager(AxisManager axisManager)
        {
            m_axisManager = axisManager;
        }

        public float OldestX { get; private set; }
        public float OldestY { get; private set; }


        public event EventHandler MouseEventCompleted;


        #region Create Event Instance
        private IPlotEvent CreateMousePanEvent(EventManager manager, InputState inputState)
            => new MousePanEvent(manager, inputState);

        private IPlotEvent CreateMouseScrollEvent(EventManager manager, InputState inputState)
            => new MouseScrollEvent(manager, inputState);

        private IPlotEvent CreateMouseZoomEvent(EventManager manager, InputState inputState)
            => new MouseZoomEvent(manager, inputState);
        #endregion

        private void OnMouseEventCompleted()
        {
            MouseEventCompleted?.Invoke(this, EventArgs.Empty);
        }

        private void ProcessEvent(IPlotEvent plotEvent)
        {
            m_axisManager.ResumeLimits();

            plotEvent.Process(m_axisManager);
            OnMouseEventCompleted();
        }

        public void MouseDown(InputState inputState)
        {
            OldestX = inputState.m_x;
            OldestY = inputState.m_y;
            m_leftPressed = inputState.m_leftPressed;
            m_rightPressed = inputState.m_rightPressed;
            m_ctrlPressed = inputState.m_controlPressed;
            m_shiftPressed = inputState.m_shiftPressed;
            m_altPressed = inputState.m_altPressed;

            m_axisManager.SuspendLimits();
        }

        public void MouseUp(InputState inputState)
        {
            OldestX = float.NaN;
            OldestY = float.NaN;

            m_leftPressed = false;
            m_rightPressed = false;
            m_ctrlPressed = false;
            m_shiftPressed = false;
            m_altPressed = false;
        }

        public void MouseScroll(InputState inputState)
        {
            IPlotEvent plotEvent = CreateMouseScrollEvent(this, inputState);
            if (plotEvent != null)
                ProcessEvent(plotEvent);
        }

        public void MouseMove(InputState inputState)
        {
            IPlotEvent plotEvent = null;
            if (m_leftPressed)
                plotEvent = CreateMousePanEvent(this, inputState);
            else if (m_rightPressed)
                plotEvent = CreateMouseZoomEvent(this, inputState);

            if (plotEvent != null)
                ProcessEvent(plotEvent);
        }

        public void MouseDoubleClick(InputState inputState) { }
    }
}
