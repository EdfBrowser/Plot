namespace Plot.Skia
{
    internal class RenderAxis : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (IAxis axis in rc.Figure.AxisManager.Axes)
            {
                (float delta, float size) = rc.AxesInfo[axis];
                axis.Render(rc, delta, size);
            }
        }
    }
}
