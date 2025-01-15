using Plot.Skia.Interaction;
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

        internal PointF MouseDownPoint { get; private set; }
        internal RememberedAxesLimit RememberedLimits { get; private set; }

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
                RememberedLimits = null;

                ApplyToPoint(figure, MouseDownPoint, mouseUpAction.Point);
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

                ApplyToPoint(figure, MouseDownPoint, mouseAction.Point);
                return true;
            }

            return false;
        }

        private void ApplyToPoint(Figure figure, PointF down, PointF now)
        {
            // TODO: 添加键位设置
            DragPan(figure, down, now);
        }

        private void DragPan(Figure figure, PointF down, PointF now)
        {
            // TODO: 定位具体的轴
            //PointF mouseDown = p1.Divide(figure.ScaleFactorF);
            //PointF mouseNow = p2.Divide(plot.ScaleFactorF);
            IFigureControl control = figure.FigureControl ?? throw new NullReferenceException();

            float pixelDeltaX = -(now.X - down.X);
            float pixelDeltaY = now.Y - down.Y;

            figure.AxisManager.PanMouse(pixelDeltaX, pixelDeltaY, figure.RenderManager.LastRC.DataRect);
        }
    }
}
