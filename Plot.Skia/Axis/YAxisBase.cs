namespace Plot.Skia
{
    internal abstract class YAxisBase : BaseAxis, IYAxis
    {
        public double Height => Range.Span;

        public override float GetPixel(double position, PixelPanel dataPanel)
        {
            double pxPerUnit = dataPanel.Height / Height;
            double unitsFromLeft = position - Min;
            float px = (float)(unitsFromLeft * pxPerUnit);
            return dataPanel.Bottom - px;
        }

        public override double GetWorld(float pixel, PixelPanel dataPanel)
        {
            double unitPerpx = Height / dataPanel.Height;
            float pxFromLeft = pixel - dataPanel.Bottom;
            double unitsFromLeft = pxFromLeft / unitPerpx;
            return Min - unitsFromLeft;
        }

        public override void Render(RenderContext rc)
        {
            DrawTicks(rc.Canvas, rc.AxisPanel);
            DrawLines(rc.Canvas, rc.AxisPanel);
        }
    }
}
