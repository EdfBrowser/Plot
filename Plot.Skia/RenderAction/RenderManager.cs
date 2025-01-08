using SkiaSharp;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class RenderManager
    {
        private readonly Figure m_figure;

        internal RenderManager(Figure figure)
        {
            m_figure = figure;
            RenderOrders = new List<IRenderAction>()
            {
                new ClearCanvas(),
                new AutoScale(),
                new CalculateLayout(),
            };

            ClearCanvasBeforeRendering = true;
        }

        internal bool ClearCanvasBeforeRendering { get; set; }
        internal IEnumerable<IRenderAction> RenderOrders { get; }

        internal void Render(SKCanvas canvas, PixelPanel pixelPanel)
        {
            //canvas.Scale(m_figure.ScaleFactor);

            RenderContext rc = new RenderContext(m_figure, canvas, pixelPanel);

            foreach (IRenderAction action in RenderOrders)
            {
                action.Render(rc);
            }
        }
    }
}
