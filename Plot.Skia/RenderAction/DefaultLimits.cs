namespace Plot.Skia
{
    internal class DefaultLimits : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            AxisManager axisManager = rc.Figure.AxisManager;
            if (!axisManager.Bottom.Range.HasBeenSet)
                AxisManager.SetLimitsX(PixelRange.Default, axisManager.Bottom);

            if (!axisManager.Left.Range.HasBeenSet)
                AxisManager.SetLimitsY(PixelRange.Default, axisManager.Left);
        }
    }
}
