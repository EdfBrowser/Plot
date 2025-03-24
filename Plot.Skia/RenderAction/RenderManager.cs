using SkiaSharp;
using System;
using System.Collections.Generic;

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
                new ScaleAxis(),
                new CalculateLayout(),
                new FigureBackground(),
                new GenerateTick(),
                new DataBackground(),
                new RenderSeries(),
                new RenderAxis(),
                new RenderPanel(),
            };
        }

        internal bool FitY { get; set; }
        internal IEnumerable<IRenderAction> RenderOrders { get; }
        internal RenderContext LastRC { get; private set; }

        internal void Render(SKCanvas canvas, Rect figureRect)
        {
            RenderContext rc = new RenderContext(m_figure, canvas, figureRect);

            foreach (IRenderAction action in RenderOrders)
            {
                rc.Canvas.Save();
                action.Render(rc);
                // 恢复画布的变化矩阵以及clip等绘制属性到上一个保存状态
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
        public void Fit() => FitY = true;
        public EventHandler<RenderContext> SizeChangedEventHandler { get; set; }
        #endregion
    }
}
