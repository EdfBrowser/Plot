using SkiaSharp;
using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class RenderManager
    {
        private readonly Figure m_figure;
        private PixelPanel? m_oldFigurePanel;

        internal RenderManager(Figure figure)
        {
            m_figure = figure;
            RenderOrders = new List<IRenderAction>()
            {
                new ClearCanvas(),
                new AxisLimits(),
                new CalculateLayout(),
                new FigureBackground(),
                new GenerateTicks(),
                new DataBackground(),
                new RenderAxis(),
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
                rc.Canvas.Save();
                action.Render(rc);
                rc.Canvas.Restore();

                if (action is CalculateLayout)
                {
                    if (m_oldFigurePanel == null || !m_oldFigurePanel.Equals(pixelPanel))
                    {
                        SizeChangedEventHandler?.Invoke(this, rc);
                        m_oldFigurePanel = pixelPanel;
                    }
                }
            }
        }

        #region PUBLIC
        public EventHandler<RenderContext> SizeChangedEventHandler { get; set; }
        #endregion
    }
}
