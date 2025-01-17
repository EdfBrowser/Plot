namespace Plot.Skia
{
    internal class RenderSeries : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (ISeries series in rc.Figure.SeriesManager.Series)
            {
                Rect dataRect = rc.GetDataRect(series.X);
                rc.Canvas.ClipRect(dataRect.ToSKRect());
                series.Render(rc);
            }
        }
    }
}
