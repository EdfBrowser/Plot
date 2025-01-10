namespace Plot.Skia
{
    internal class DefaultLimits : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;

            foreach (IAxis axis in axisManager.Axes)
            {
                if (!axis.Range.HasBeenSet)
                {
                   AxisManager.SetLimits(PixelRange.Default, axis);
                }
            }
        }
    }
}
