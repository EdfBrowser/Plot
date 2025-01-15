namespace Plot.Skia
{
    internal class GenerateTicks : IRenderAction
    {
        public void Render(RenderContext rc)
            => rc.Figure.AxisManager.GenerateTicks(rc.DataRect, rc.AxesInfo);
    }
}
