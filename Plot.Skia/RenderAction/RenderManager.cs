using SkiaSharp;
using System;
using System.Collections.Generic;
using static System.Collections.Specialized.BitVector32;

namespace Plot.Skia
{
    public class RenderManager
    {
        private readonly Figure m_figure;
        private Rect? m_oldFigureRect;

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
        }

        internal IEnumerable<IRenderAction> RenderOrders { get; }
        internal RenderContext LastRC { get; private set; }

        internal void Render(SKCanvas canvas, Rect figureRect)
        {
            RenderContext rc = new RenderContext(m_figure, canvas, figureRect);

            foreach (IRenderAction action in RenderOrders)
            {
                rc.Canvas.Save();
                action.Render(rc);
                rc.Canvas.Restore();
            }

            LastRC = rc;

            if (m_oldFigureRect == null
                || !m_oldFigureRect.Equals(figureRect))
            {
                SizeChangedEventHandler?.Invoke(this, rc);
                m_oldFigureRect = figureRect;
            }
        }

        #region PUBLIC
        public EventHandler<RenderContext> SizeChangedEventHandler { get; set; }
        #endregion
    }
}
