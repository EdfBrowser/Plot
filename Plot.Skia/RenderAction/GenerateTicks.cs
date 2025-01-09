namespace Plot.Skia
{
    internal class GenerateTicks : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (IXAxis axis in rc.Figure.AxisManager.XAxes)
            {
                axis.GenerateTicks(rc.DataPanel.Width);
            }

            foreach (IYAxis axis in rc.Figure.AxisManager.YAxes)
            {
                axis.GenerateTicks(rc.DataPanel.Height);
            }
        }
    }
}
