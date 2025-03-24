namespace Plot.Skia
{
    internal class GenerateTick : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;
            foreach (IAxis axis in axisManager.Axes)
            {
                Rect rect = rc.GetDataRect(axis);

                if (axis.Direction.Horizontal())
                    axisManager.GenerateTicks(rect.Width, axis);
                else
                    axisManager.GenerateTicks(rect.Height, axis);
            }
        }
    }
}
