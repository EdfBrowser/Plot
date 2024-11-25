using Plot.Core.Draws;
using Plot.Core.Enum;
using Plot.Core.EventProcess;
using Plot.Core.Renderables.Axes;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plot.Core
{
    public static class AxisFactory
    {
        public static Axis CreateAxis(Edge edge)
        {
            switch (edge)
            {
                case Edge.Left: return new LeftAxis();
                case Edge.Right: return new RightAxis();
                case Edge.Top: return new TopAxis();
                case Edge.Bottom: return new BottomAxis();
                default: throw new NotImplementedException($"Unsupported edge type: {edge}");
            }
        }
    }

    public partial class Figure
    {
        #region Bitmap Manager
        private class BitmapManager
        {
            private Bitmap m_bmp;
            private readonly Bitmap[] m_oldBitmaps = new Bitmap[3]; // 固定大小为3的数组
            private int m_currentIndex = 0; // 当前索引

            private void StoreBitmap()
            {
                if (m_oldBitmaps[m_currentIndex] != null)
                {
                    Bitmap bmp = m_oldBitmaps[m_currentIndex];
                    bmp.Dispose();
                    bmp = null;
                }

                m_oldBitmaps[m_currentIndex++ % m_oldBitmaps.Length] = m_bmp;
            }

            public Bitmap GetLatestBitmap => m_bmp;

            public void CreateBitmap(int width, int height)
            {
                // 旧的bitmap
                if (m_bmp != null)
                {
                    // if the size don`t changed, return immediately
                    if (m_bmp.Width == width && m_bmp.Height == height) return;

                    StoreBitmap();
                }

                m_bmp = new Bitmap(width, height);
            }
        }

#endregion

        private BitmapManager m_bitmapManager = new BitmapManager();
        private EventManager m_eventManager;
        private readonly Stopwatch m_stopwatch;

        private List<SampleDataSeries> SeriesList { get; set; } = new List<SampleDataSeries>();

        // TODO: 修改成c#的锁机制
        private readonly object m_lockObj = new object();

      

        public Figure()
        {
            m_stopwatch = new Stopwatch();
            m_eventManager = new EventManager(this);
            m_eventManager.MouseEventCompleted += OnMouseEventCompleted;

            Axes.Add(AxisFactory.CreateAxis(Edge.Top));
            Axes.Add(AxisFactory.CreateAxis(Edge.Bottom));
            Axes.Add(AxisFactory.CreateAxis(Edge.Left));
            Axes.Add(AxisFactory.CreateAxis(Edge.Right));
        }

        public List<Axis> Axes { get; } = new List<Axis>();
       
        public List<Axis> TopAxes => Axes.Where(x => x.Edge == Edge.Top).ToList();
        public List<Axis> BottomAxes => Axes.Where(x => x.Edge == Edge.Bottom).ToList();
        public List<Axis> LeftAxes => Axes.Where(x => x.Edge == Edge.Left).ToList();
        public List<Axis> RightAxes => Axes.Where(x => x.Edge == Edge.Right).ToList();

        // TODO: Draw Title
        public string LabelTitle { get; set; }

        public float AxisSpace { get; set; } = 20;

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;


        public void Resize(float width, float height)
        {
            if (width < 10 || height < 10) return;

            m_bitmapManager.CreateBitmap((int)width, (int)height);
            OnBitmapChanged?.Invoke(null, null);

            Render();
        }

        public void Render(bool lowQuality = false, float scale = 1.0f)
        {
            lock (m_lockObj)
            {
                Bitmap bmp = m_bitmapManager.GetLatestBitmap;
                if (bmp == null) return;

                AutoScaleByPlot();

                Layout(bmp.Width / scale, bmp.Height / scale);

                var primaryDims = GetDimensions(BottomAxes[0], LeftAxes[0], scale);

                RenderClear(bmp, lowQuality, primaryDims);
                RenderBeforePlot(bmp, lowQuality, primaryDims);
                RenderPlot(bmp, lowQuality, primaryDims);
                RenderAfterPlot(bmp, lowQuality, primaryDims);

                OnBitmapUpdated?.Invoke(null, null);
            }
        }

        #region Axis

        public void SetAxisLimits(AxisLimits axisLimits, Axis xAxis, Axis yAxis)
        {
            xAxis.Dims.SetLimits((float)axisLimits.m_xMin, (float)axisLimits.m_xMax);
            yAxis.Dims.SetLimits((float)axisLimits.m_yMin, (float)axisLimits.m_yMax);
        }

        public AxisLimits GetAxisLimits(Axis xAxis, Axis yAxis)
        {
            (double xMin, double xMax) = xAxis.Dims.RationalLimits();
            (double yMin, double yMax) = yAxis.Dims.RationalLimits();
            return new AxisLimits(xMin, xMax, yMin, yMax);
        }


        public Axis AddAxes(Edge edge)
        {
            Axis axis = AxisFactory.CreateAxis(edge);
            Axes.Add(axis);
            return axis;
        }
        #endregion

        #region Bitmap
        public Bitmap GetLatestBitmap() => m_bitmapManager.GetLatestBitmap;
        #endregion

        #region Render
        private void Layout(float width, float height)
        {
            // tick 的密度与axis size有关
            // axis size 与 pad有关
            // pad 大小与label有关
            // label 大小与tick有关
            // 先有鸡还是先有蛋
            // 假设pad为0，没得label，先计算出 tick label的大小

            SizeF figureSizPx = new SizeF(width, height);

            {
                foreach (Axis axis in Axes)
                {
                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dimsFull = new PlotDimensions(figureSizPx, figureSizPx, figureSizPx,
                                                new PointF(0, 0), new PointF(0, 0),
                                                (xAxis.Dims.RationalLimits(), yAxis.Dims.RationalLimits()),
                                                1f, xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);
                    CalculatePadding(dimsFull, xAxis, yAxis);
                }

                RecalculateDataPadding(width, height);
            }

            {
                foreach (Axis axis in Axes)
                {
                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dims = GetDimensions(xAxis, yAxis, 1f);
                    CalculatePadding(dims, axis, yAxis);
                }

                RecalculateDataPadding(width, height);
            }
        }


        private void AutoScaleByPlot()
        {
            double xmin = double.MaxValue, xmax = double.MinValue;
            double ymin = double.MaxValue, ymax = double.MinValue;
            var limits = SeriesList
                .Where(x => !x.XAxis.Dims.HasBeenSet || !x.YAxis.Dims.HasBeenSet)
                .Select(X => X.GetAxisLimits());

            foreach (var limit in limits)
            {
                xmin = xmin == double.MaxValue ? limit.m_xMin : Math.Min(xmin, limit.m_xMin);
                xmax = xmax == double.MinValue ? limit.m_xMax : Math.Max(xmax, limit.m_xMax);
                ymin = ymin == double.MaxValue ? limit.m_yMin : Math.Min(ymin, limit.m_yMin);
                ymax = ymax == double.MinValue ? limit.m_yMax : Math.Max(ymax, limit.m_yMax);
            }

            if (xmin == double.MaxValue || xmin == double.MinValue || ymin == double.MaxValue || ymin == double.MinValue)
                return;

            foreach (var series in SeriesList)
            {
                series.XAxis.Dims.SetLimits((float)xmin, (float)xmax);
                series.YAxis.Dims.SetLimits((float)ymin, (float)ymax);
            }
        }

        private void RenderClear(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            Color figureColor = Color.White;
            // clear and set the background of figure
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                gfx.Clear(figureColor);
            }
        }

        // TODO: IDEA 是否可以渲染成多个表
        private void RenderBeforePlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            Color dataAreaColor = Color.White;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor))
            using (var gfx = GDI.Graphics(bmp, dims, lowQuality))
            {
                var dataRect = new RectangleF(
                      x: dims.m_plotOffsetX,
                      y: dims.m_plotOffsetY,
                      width: dims.m_dataWidth,
                      height: dims.m_dataHeight);

                gfx.FillRectangle(brush, dataRect);
            }

            foreach (var axis in Axes)
            {
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis, LeftAxes[0], dims.m_scaleFactor) :
                   GetDimensions(BottomAxes[0], axis, dims.m_scaleFactor);

                try
                {
                    axis.Render(bmp, dims2, lowQuality);
                }
                catch (OverflowException ex)
                {
                    Debug.WriteLine($"OverflowException plotting: {axis}");
                }
            }
        }


        private void RenderPlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            foreach (var series in SeriesList)
            {
                series.ValidateData();

                PlotDimensions dims2 = GetDimensions(series.XAxis, series.YAxis, dims.m_scaleFactor);

                try
                {
                    series.Plot(bmp, dims2, lowQuality);
                }
                catch (OverflowException ex)
                {
                    Debug.WriteLine($"OverflowException plotting: {series}");
                }
            }
        }


        private void RenderAfterPlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
        }

        #endregion

        #region Helpers
        // TODO: 多个x轴和y轴应该有一个对应关系
        private static PlotDimensions GetDimensions(Axis xAxis, Axis yAxis, float scale)
        {
            SizeF figureSize = new SizeF(xAxis.Dims.FigureSizePx, yAxis.Dims.FigureSizePx);
            SizeF plotSize = new SizeF(xAxis.Dims.DataSizePx, yAxis.Dims.DataSizePx);
            SizeF dataSize = new SizeF(xAxis.Dims.PlotSizePx, yAxis.Dims.PlotSizePx);
            PointF plotOffset = new PointF(xAxis.Dims.PlotOffsetPx, yAxis.Dims.PlotOffsetPx);
            PointF dataOffset = new PointF(xAxis.Dims.DataOffsetPx, yAxis.Dims.DataOffsetPx);

            (float xMin, float xMax) = xAxis.Dims.RationalLimits();
            (float yMin, float yMax) = yAxis.Dims.RationalLimits();


            return new PlotDimensions(figureSize,
                dataSize,
                plotSize,
                plotOffset,
                dataOffset,
                ((xMin, xMax), (yMin, yMax)),
                scale,
                xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);

        }

        private void ArrangeAxes(float px, List<Axis> axes, float p1, float p2)
        {
            int axisCount = axes.Count;
            if (axisCount == 0) return;

            float totalSpacing = AxisSpace * (axisCount - 1);
            float dataSize = px - p1 - p2;
            float availableSize = dataSize - totalSpacing;

            float plotSize = availableSize / axisCount;

            float plotOffset = 0;
            plotOffset += p1;
            foreach (var axis in axes)
            {
                axis.Dims.Resize(px, plotSize, dataSize, p1, plotOffset);
                plotOffset += plotSize + AxisSpace;
            }
        }

        private void GetPrimayAxis(Axis axis, out Axis xAxis, out Axis yAxis)
        {
            if (axis.IsHorizontal)
            {
                xAxis = axis;
                yAxis = LeftAxes[0];
            }
            else if (axis.IsVertical)
            {
                xAxis = BottomAxes[0];
                yAxis = axis;
            }
            else
            {
                throw new NotImplementedException($"unsupported edge type {axis.Edge}");
            }
        }

        private void CalculatePadding(PlotDimensions dims, Axis xAxis, Axis yAxis)
        {
            xAxis.RecalculateTickPositions(dims);
            yAxis.RecalculateTickPositions(dims);

            xAxis.ReCalculateAxisSize();
            yAxis.ReCalculateAxisSize();
        }

        private void RecalculateDataPadding(float width, float height)
        {
            float PadLeft = LeftAxes[0].GetSize();
            float PadRight = RightAxes[0].GetSize();
            float PadTop = TopAxes[0].GetSize();
            float PadBottom = BottomAxes[0].GetSize();

            ArrangeAxes(width, TopAxes, PadLeft, PadRight);
            ArrangeAxes(width, BottomAxes, PadLeft, PadRight);
            ArrangeAxes(height, LeftAxes, PadTop, PadBottom);
            ArrangeAxes(height, RightAxes, PadTop, PadBottom);
        }
        #endregion

        #region Series

        public SampleDataSeries AddDataStreamer(Axis xAxis, Axis yAxis, int sampleRate)
        {
            SampleDataSeries dataStreamSeries = new SampleDataSeries(xAxis, yAxis, this, sampleRate);
            SeriesList.Add(dataStreamSeries);
            return dataStreamSeries;
        }

        public void ClearSeries()
        {
            SeriesList.Clear();
        }

        #endregion

        #region Event
        public void MouseDown(InputState inputState) => m_eventManager.MouseDown(inputState);
        public void MouseUp(InputState inputState) => m_eventManager.MouseUp(inputState);
        public void MouseMove(InputState inputState)  => m_eventManager.MouseMove(inputState);
        public void MouseDoubleClick(InputState inputState) => m_eventManager.MouseDoubleClick(inputState);
        public void MouseWheel(InputState inputState)   => m_eventManager.MouseScroll(inputState);

        private void OnMouseEventCompleted(object sender, EventArgs e) => Render();
        #endregion
    }
}
