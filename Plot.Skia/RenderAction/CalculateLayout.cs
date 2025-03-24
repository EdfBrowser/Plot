namespace Plot.Skia
{
    internal class CalculateLayout : IRenderAction
    {
        public void Render(RenderContext rc) => rc.CalculateLayout();
    }
}
