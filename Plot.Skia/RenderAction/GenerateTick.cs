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

                axisManager.GenerateTick(axis, rect);
            }
        }
    }
}
