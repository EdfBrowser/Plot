using System;

namespace Plot.Core.EventProcess
{
    public class EventManager
    {
        private readonly Figure m_figure;

        private bool m_leftPressed = false;
        private bool m_rightPressed = false;
        private bool m_ctrlPressed = false;
        private bool m_shiftPressed = false;
        private bool m_altPressed = false;


        public EventManager(Figure figure)
        {
            m_figure = figure;
        }

        public float X { get; private set; }
        public float Y { get; private set; }

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
            ResumeLimits();
            plotEvent.Process();
            OnMouseEventCompleted();
        }

        private void SuspendLimits()
        {
            foreach (var axis in m_figure.Axes)
                axis.Dims.SuspendLimits();
        }

        private void ResumeLimits()
        {
            foreach (var axis in m_figure.Axes)
                axis.Dims.ResumeLimits();
        }

        public void PanAll(float x, float y)
        {
            foreach (var axis in m_figure.Axes)
            {
                if (axis.IsHorizontal)
                    axis.Dims.PanPx(x);
                else
                    axis.Dims.PanPx(y);
            }
        }

        public void ZoomCenter(float xfrac, float yfrac, float x, float y)
        {
            foreach (var axis in m_figure.Axes)
            {
                float frac = axis.IsHorizontal ? xfrac : yfrac;
                float centerPx = axis.IsHorizontal ? x : y;
                float center = axis.Dims.GetUnit(centerPx);

                axis.Dims.Zoom(frac, center);
            }
        }

        public void ZoomCenter(float x, float y)
        {
            foreach (var axis in m_figure.Axes)
            {
                float deltaPx = axis.IsHorizontal ? x - X : Y - y;
                float delta = deltaPx * axis.Dims.UnitsPerPx;

                float deltaFrac = delta / (Math.Abs(delta) + axis.Dims.Center);

                float frac = (float)Math.Pow(10, deltaFrac);

                axis.Dims.Zoom(frac);
            }
        }

        public void MouseDown(InputState inputState)
        {
            X = inputState.m_x;
            Y = inputState.m_y;
            m_leftPressed = inputState.m_leftPressed;
            m_rightPressed = inputState.m_rightPressed;
            m_ctrlPressed = inputState.m_controlPressed;
            m_shiftPressed = inputState.m_shiftPressed;
            m_altPressed = inputState.m_altPressed;

            SuspendLimits();
        }

        public void MouseUp(InputState inputState)
        {
            X = float.NaN;
            Y = float.NaN;

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
