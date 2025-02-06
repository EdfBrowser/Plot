namespace Plot.Skia
{
    internal class RenderSeries : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (ISeries series in rc.Figure.SeriesManager.Series)
                series.Render(rc);
        }
    }
}
