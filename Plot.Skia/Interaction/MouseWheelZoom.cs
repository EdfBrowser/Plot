using System;

namespace Plot.Skia
{
    internal class MouseWheelZoom : IUserActionResponse
    {
        public MouseWheelZoom()
        {
            ZoomFraction = 0.15;
        }

        private double ZoomInFraction => 1 + ZoomFraction;
        private double ZoomOutFraction => 1 / ZoomInFraction;

        internal double ZoomFraction { get; set; }
        public CursorType CursorType { get; private set; }

        public bool Execute(Figure figure, IUserAction userInput)
        {
            if (userInput is MouseWheelDown mouseDownAction)
            {
                double xFrac = ZoomInFraction;
                double yFrac = ZoomInFraction;
                WheelZoom(figure, xFrac, yFrac, mouseDownAction.Point);

                return true;
            }

            if (userInput is MouseWheelUp mouseUpAction)
            {
                double xFrac = ZoomOutFraction;
                double yFrac = ZoomOutFraction;
                WheelZoom(figure, xFrac, yFrac, mouseUpAction.Point);

                return true;
            }


            return false;
        }

        private void WheelZoom(Figure figure,
            double xFrac, double yFrac, PointF down)
        {
            IFigureControl control = figure.FigureControl ?? throw new NullReferenceException();

            IAxis axisUnderMouse = figure.AxisManager.HitAxis(down);
            if (axisUnderMouse != null)
            {
                Rect dataRect = figure.RenderManager.LastRC.GetDataRect(axisUnderMouse);
                double frac = axisUnderMouse.Direction.Horizontal()
                     ? xFrac : yFrac;
                float px = axisUnderMouse.Direction.Horizontal()
                    ? down.X : down.Y;
                figure.AxisManager.ZoomMouse(axisUnderMouse, frac, px, dataRect);
            }
        }

        public void Reset(Figure figure)
        {

        }
    }
}
