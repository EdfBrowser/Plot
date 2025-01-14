namespace Plot.Skia
{
    internal class GenerateTicks : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            PixelPanel dataPanel = rc.Layout.DataPanel;
            foreach (IXAxis axis in rc.Figure.AxisManager.XAxes)
            {
                (float delta, float size) = rc.Layout.PanelDeltas[axis];
                axis.GenerateTicks(axis.GetPanel(dataPanel, delta, size).Width);
            }

            foreach (IYAxis axis in rc.Figure.AxisManager.YAxes)
            {
                (float delta, float size) = rc.Layout.PanelDeltas[axis];
                axis.GenerateTicks(axis.GetPanel(dataPanel, delta, size).Height);
            }
        }
    }
}
