using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core
{
    public class Figure
    {
        private Bitmap m_bmp;
        private readonly Queue<Bitmap> m_oldBitmaps = new Queue<Bitmap>();
        private long m_bitmapRenderCount = 0;

        private readonly System.Diagnostics.Stopwatch m_stopwatch;

        private List<BaseSeries> SeriesList { get; set; } = new List<BaseSeries>();

        // TODO: 修改成c#的锁机制
        private object m_lockObj = new object();

        public Figure()
        {
            m_stopwatch = new System.Diagnostics.Stopwatch();
        }

        public List<Axis> Axes { get; set; } = new List<Axis>()
        {
            new Axis(Edge.Bottom, 0) {AxisLabel = "X Axis"},
            new Axis(Edge.Left, 0) {AxisLabel = "Y1 Axis"},
            new Axis(Edge.Left, 1) {AxisLabel = "Y2 Axis"},
        };

        public List<Axis> YAxes => Axes.Where(i => i.IsVertical).ToList();
        public List<Axis> XAxes => Axes.Where(i => i.IsHorizontal).ToList();
        public string LabelTitle { get; set; }

        public float PadLeft { get; set; } = 50;
        public float PadRight { get; set; } = 50;
        public float PadTop { get; set; } = 47;
        public float PadBottom { get; set; } = 47;
        public float AxisSpace { get; set; } = 10;

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;

        public Bitmap GetLatestBitmap()
        {
            while (m_oldBitmaps.Count > 3)
            {
                m_oldBitmaps.Dequeue()?.Dispose();
            }

            return m_bmp;
        }

        public void Resize(float width, float height)
        {
            // TODO: 自动计算PadLeft, PadRight, PadTop, PadBottom
            if (width - PadLeft - PadRight < 1) width = PadLeft + PadRight + AxisSpace * (XAxes.Count - 1) + 100;
            if (height - PadTop - PadBottom < 1) height = PadTop + PadBottom + AxisSpace * (YAxes.Count - 1) + 100;

            if (m_bmp != null)
            {
                if (m_bmp.Width == width && m_bmp.Height == height) return;

                m_oldBitmaps.Enqueue(m_bmp);
            }


            m_bmp = new Bitmap((int)width, (int)height);

            m_bitmapRenderCount = 0;

            Render();
        }

        public void Render(bool lowQuality = false, float scale = 1.0f)
        {
            lock (m_lockObj)
            {
                if (m_bmp == null) return;

                Layout(m_bmp.Width / scale, m_bmp.Height / scale);
                var primaryDims = GetDimensions(0, 0, scale);

                CalculateTicks(primaryDims);

                RenderClear(m_bmp, lowQuality, primaryDims);
                RenderBeforePlot(m_bmp, lowQuality, primaryDims);
                RenderPlot(m_bmp, lowQuality, primaryDims);
                RenderAfterPlot(m_bmp, lowQuality, primaryDims);

                m_bitmapRenderCount += 1;

                if (m_bitmapRenderCount == 1)
                {
                    OnBitmapChanged?.Invoke(null, null);
                }
                else
                {
                    OnBitmapUpdated?.Invoke(null, null);
                }
            }
        }

        private void Layout(float width, float height)
        {
            // TODO: 自动计算PadLeft, PadRight, PadTop, PadBottom
            LayoutX(width);
            LayoutY(height);
        }

        private void LayoutY(float height)
        {
            int axisCount = YAxes.Count;
            if (axisCount == 0) return;

            float totalSpacing = AxisSpace * (axisCount - 1);
            float dataSize = height - PadTop - PadBottom;
            float availableSize = dataSize - totalSpacing;

            float axisSize = availableSize / axisCount;

            float offset = 0;
            offset += PadTop;
            foreach (var axis in YAxes)
            {
                axis.Dims.Resize(height, axisSize, offset, dataSize);
                offset += axisSize + AxisSpace;
            }
        }

        private void LayoutX(float width)
        {
            float m_spacing = 10;

            int axisCount = XAxes.Count;
            if (axisCount == 0) return;

            float totalSpacing = m_spacing * (axisCount - 1);
            float dataSize = width - PadLeft - PadRight;
            float availableSize = dataSize - totalSpacing;

            float axisSize = availableSize / axisCount;

            float offset = 0;
            offset += PadTop;
            foreach (var axis in XAxes)
            {
                axis.Dims.Resize(width, axisSize, offset, dataSize);
                offset += axisSize + m_spacing;
            }
        }

        // TODO: 多个x轴和y轴应该有一个对应关系
        private PlotDimensions GetDimensions(int xIndex, int yIndex, float scale)
        {
            var yAxis = YAxes[yIndex];
            var xAxis = XAxes[xIndex];

            SizeF figureSize = new SizeF(xAxis.Dims.FigureSizePx, yAxis.Dims.FigureSizePx);
            SizeF plotSize = new SizeF(xAxis.Dims.PlotSizePx, yAxis.Dims.PlotSizePx);
            SizeF dataSize = new SizeF(xAxis.Dims.DataSizePx, yAxis.Dims.DataSizePx);
            PointF offset = new PointF(xAxis.Dims.DataOffsetPx, yAxis.Dims.DataOffsetPx);

            (float xMin, float xMax) = xAxis.Dims.RationalLimits();
            (float yMin, float yMax) = yAxis.Dims.RationalLimits();


            return new PlotDimensions(figureSize,
                dataSize,
                plotSize,
                offset,
                ((xMin, xMax), (yMin, yMax)),
                scale,
                xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);

        }

        private void CalculateTicks(PlotDimensions dims)
        {
            foreach (var axis in Axes)
            {
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis.AxisIndex, 0, dims.ScaleFactor) :
                    GetDimensions(0, axis.AxisIndex, dims.ScaleFactor);
                axis.Generator.RecalculateTicks(dims2);
            }
        }

        private void RenderClear(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            Color figureColor = Color.LightGray;
            // clear and set the background of figure
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                gfx.Clear(figureColor);
            }
        }


        private void RenderBeforePlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            Color dataAreaColor = Color.White;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor, 1))
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                var dataRect = new RectangleF(
                      x: dims.DataOffsetX,
                      y: dims.DataOffsetY,
                      width: dims.PlotWidth,
                      height: dims.PlotHeight);

                gfx.FillRectangle(brush, dataRect);
            }

            foreach (var axis in Axes)
            {
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis.AxisIndex, 0, dims.ScaleFactor) :
                   GetDimensions(0, axis.AxisIndex, dims.ScaleFactor);
                axis.Render(bmp, dims2, lowQuality);
            }
        }


        private void RenderAfterPlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
        }

        private void RenderPlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
        }
















        public DataStreamSeries AddDataStreamer(int xIndex, int yIndex, int length)
        {
            double[] data = new double[length];
            return AddDataStreamer(xIndex, yIndex, data);
        }

        public DataStreamSeries AddDataStreamer(int xIndex, int yIndex, double[] values)
        {
            DataStreamSeries dataStreamSeries = new DataStreamSeries(xIndex, yIndex, this, values);
            SeriesList.Add(dataStreamSeries);
            return dataStreamSeries;
        }

        public void ClearSeries()
        {
            SeriesList.Clear();
        }
    }
}
