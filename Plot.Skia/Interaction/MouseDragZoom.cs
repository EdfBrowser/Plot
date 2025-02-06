using System;

namespace Plot.Skia
{
    internal class MouseDragZoom : IUserActionResponse
    {
        private readonly string m_buttonName;

        internal MouseDragZoom(string buttonName)
        {
            m_buttonName = buttonName;
        }

        private PointF MouseDownPoint { get; set; }
        private RememberedAxesLimit RememberedLimits { get; set; }

        public bool Execute(Figure figure, IUserAction userInput)
        {
            if (userInput is IMouseButtonAction mouseDownAction
                && mouseDownAction.ButtonName == m_buttonName
                && mouseDownAction.Pressed)
            {
                MouseDownPoint = mouseDownAction.Point;
                RememberedLimits = new RememberedAxesLimit(figure);

                return false;
            }

            if (userInput is IMouseButtonAction mouseUpAction
                && mouseUpAction.ButtonName == m_buttonName
                && !mouseUpAction.Pressed
                && RememberedLimits != null)
            {
                RememberedLimits.Recall();

                SetRules(figure, MouseDownPoint, mouseUpAction.Point);
                Reset(figure);

                return true;
            }

            if (userInput is IMouseAction mouseAction
              && RememberedLimits != null)
            {
                RememberedLimits.Recall();

                SetRules(figure, MouseDownPoint, mouseAction.Point);

                return true;
            }

            return false;
        }

        private void SetRules(Figure figure, PointF down, PointF now)
        {
            DragZoom(figure, down, now);
        }

        private void DragZoom(Figure figure, PointF down, PointF now)
        {
            IFigureControl control = figure.FigureControl ?? throw new NullReferenceException();

            float deltaX = now.X - down.X;
            float deltaY = now.Y - down.Y;

            float scaledDeltaX = deltaX / control.DisplayScale;
            float scaledDeltaY = deltaY / control.DisplayScale;

            IAxis axisUnderMouse = figure.AxisManager.HitAxis(down);
            if (axisUnderMouse != null)
            {
                Rect dataRect = figure.RenderManager.LastRC.GetDataRect(axisUnderMouse);
                float scaledDelta = axisUnderMouse.Direction.Horizontal()
                     ? scaledDeltaX : scaledDeltaY;
                float axisLength = axisUnderMouse.Direction.Horizontal()
                    ? dataRect.Width : dataRect.Height;
                figure.AxisManager.ZoomMouse(axisUnderMouse, scaledDelta, axisLength);
            }
        }

        public void Reset(Figure figure)
        {
            RememberedLimits = null;
            MouseDownPoint = PointF.NoSet;
        }
    }
}
