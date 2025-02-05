namespace Plot.Skia
{
    internal class RenderSeries : IRenderAction
    {
        public void Render(RenderContext rc)
        {
            foreach (ISeries series in rc.Figure.SeriesManager.Series)
            {
                Rect dataRect = series.GetDataRect(rc, series.X, series.Y);

                rc.Canvas.Save();
                rc.Canvas.ClipRect(dataRect.ToSKRect());
                series.Render(rc);
                rc.Canvas.Restore();
            }
        }
    }
}
