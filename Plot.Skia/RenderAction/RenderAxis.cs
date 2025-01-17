namespace Plot.Skia
{
    internal class RenderAxis : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (IAxis axis in rc.Figure.AxisManager.Axes)
            {
                axis.Render(rc);
            }
        }
    }
}
