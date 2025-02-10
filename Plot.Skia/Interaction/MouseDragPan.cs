using System;

namespace Plot.Skia
{
    internal class MouseDragPan : IUserActionResponse
    {
        private readonly string m_buttonName;

        internal MouseDragPan(string buttonName)
        {
            m_buttonName = buttonName;
        }

        private PointF MouseDownPoint { get; set; }
        private RememberedAxesLimit RememberedLimits { get; set; }

        public CursorType CursorType { get; private set; }

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
                double dX = Math.Abs(mouseAction.Point.X - MouseDownPoint.X);
                double dY = Math.Abs(mouseAction.Point.Y - MouseDownPoint.Y);
                double maxDragDistance = Math.Max(dX, dY);
                if (maxDragDistance < 5)
                    return false;

                RememberedLimits.Recall();

                SetRules(figure, MouseDownPoint, mouseAction.Point);

                return true;
            }

            return false;
        }

        private void SetRules(Figure figure, PointF down, PointF now)
        {
            // TODO: 是否添加键位设置
            DragPan(figure, down, now);
            figure.FigureControl.SetCursor(CursorType);
        }

        private void DragPan(Figure figure, PointF down, PointF now)
        {
            IFigureControl control = figure.FigureControl ?? throw new NullReferenceException();

            float deltaX = -(now.X - down.X);
            float deltaY = now.Y - down.Y;

            float scaledDeltaX = deltaX / control.DisplayScale;
            float scaledDeltaY = deltaY / control.DisplayScale;

            IAxis axisUnderMouse = figure.AxisManager.HitAxis(down);
            // set cursor type
            if (axisUnderMouse is IXAxis)
                CursorType = CursorType.SizeWE;
            if (axisUnderMouse is IYAxis)
                CursorType = CursorType.SizeNS;

            if (axisUnderMouse != null)
            {
                Rect dataRect = figure.RenderManager.LastRC.GetDataRect(axisUnderMouse);
                float scaledDelta = axisUnderMouse.Direction.Horizontal()
                     ? scaledDeltaX : scaledDeltaY;
                float axisLength = axisUnderMouse.Direction.Horizontal()
                    ? dataRect.Width : dataRect.Height;
                figure.AxisManager.PanMouse(axisUnderMouse, scaledDelta, axisLength);
            }
        }

        public void Reset(Figure figure)
        {
            figure.FigureControl.SetCursor(CursorType.Default);
            RememberedLimits = null;
            MouseDownPoint = PointF.NoSet;
        }
    }
}
