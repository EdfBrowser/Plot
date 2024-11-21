using Plot.Core.EventProcess;
using Plot.Core.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plot.Core
{
    public class Figure
    {
        private Bitmap m_bmp;
        private readonly Bitmap[] m_oldBitmaps = new Bitmap[3]; // 固定大小为3的数组
        private int m_currentIndex = 0; // 当前索引

        private readonly EventFactory m_eventFactory;

        private readonly System.Diagnostics.Stopwatch m_stopwatch;

        private List<Axis> Axes { get; } = new List<Axis>()
        {
            new Axis(Edge.Bottom, 0),
            new Axis(Edge.Left, 0)
        };

        public List<Axis> XAxes => Axes.Where(x => x.IsHorizontal).ToList();
        public List<Axis> YAxes => Axes.Where(x => x.IsVertical).ToList();

        private List<SampleDataSeries> SeriesList { get; set; } = new List<SampleDataSeries>();

        // TODO: 修改成c#的锁机制
        private readonly object m_lockObj = new object();

        public Figure()
        {
            m_stopwatch = new System.Diagnostics.Stopwatch();

            m_eventFactory = new EventFactory(this);
        }

        // TODO: Draw Title
        public string LabelTitle { get; set; }

        public float PadLeft { get; set; } = 50;
        public float PadRight { get; set; } = 50;
        public float PadTop { get; set; } = 47;
        public float PadBottom { get; set; } = 47;
        public float AxisSpace { get; set; } = 10;

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;


        public bool ExistXAxis(int index) => XAxes.Exists(x => x.AxisIndex == index);
        public bool ExistYAxis(int index) => YAxes.Exists(x => x.AxisIndex == index);

        public Axis GetXAxis(int index)
        {
            Axis[] axes = XAxes.Where(x => x.AxisIndex == index).ToArray();

            if (axes.Length == 0)
                throw new InvalidOperationException($"There no X axes with an axis index of {index}");
            return axes[0];
        }

        public Axis GetYAxis(int index)
        {
            Axis[] axes = YAxes.Where(x => x.AxisIndex == index).ToArray();

            if (axes.Length == 0)
                throw new InvalidOperationException($"There no X axes with an axis index of {index}");
            return axes[0];
        }

        public Axis AddYAxis(int? index = null)
        {
            index = index ?? (YAxes.Max(i => i.AxisIndex) + 1);
            Axis axis = new Axis(Edge.Left, index.Value);

            Axes.Add(axis);
            return axis;
        }

        public Axis AddXAxis(int? index = null)
        {
            index = index ?? (XAxes.Max(i => i.AxisIndex) + 1);
            Axis axis = new Axis(Edge.Bottom, index.Value);

            Axes.Add(axis);
            return axis;
        }


        public Bitmap GetLatestBitmap() => m_bmp;

        private void AddBitmap(Bitmap bitmap)
        {
            if (m_oldBitmaps[m_currentIndex] != null)
            {
                m_oldBitmaps[m_currentIndex].Dispose();
            }

            m_oldBitmaps[m_currentIndex] = bitmap;
            m_currentIndex = (m_currentIndex + 1) % m_oldBitmaps.Length;
        }

        public void Resize(float width, float height)
        {
            // TODO: 自动计算PadLeft, PadRight, PadTop, PadBottom
            if (width - PadLeft - PadRight < 1) width = PadLeft + PadRight + AxisSpace * (XAxes.Count - 1) + 100;
            if (height - PadTop - PadBottom < 1) height = PadTop + PadBottom + AxisSpace * (YAxes.Count - 1) + 100;

            if (m_bmp != null)
            {
                if (m_bmp.Width == width && m_bmp.Height == height) return;

                AddBitmap(m_bmp);
            }

            m_bmp = new Bitmap((int)width, (int)height);

            OnBitmapChanged?.Invoke(null, null);

            Render();
        }

        public void Render(bool lowQuality = false, float scale = 1.0f)
        {
            lock (m_lockObj)
            {
                if (m_bmp == null) return;

                Layout(m_bmp.Width / scale, m_bmp.Height / scale);
                var primaryDims = GetDimensions(0, 0, scale);

                AutoScale();
                CalculateTicks(primaryDims);

                RenderClear(m_bmp, lowQuality, primaryDims);
                RenderBeforePlot(m_bmp, lowQuality, primaryDims);
                RenderPlot(m_bmp, lowQuality, primaryDims);
                RenderAfterPlot(m_bmp, lowQuality, primaryDims);

                OnBitmapUpdated?.Invoke(null, null);
            }
        }

        public void AutoScale()
        {
            Axis[] xAxes = XAxes.Where(x => !x.Dims.HasBeenSet).ToArray();
            Axis[] yAxes = YAxes.Where(x => !x.Dims.HasBeenSet).ToArray();
            foreach (var axis in xAxes)
            {
                AxisAutoX(axis);
            }
            foreach (var axis in yAxes)
            {
                AxisAutoY(axis);
            }
        }

        public void AxisAutoX(Axis axis)
        {
            double xMin = double.MaxValue, xMax = double.MinValue;
            var limits = SeriesList
                .Where(x => x.YIndex == axis.AxisIndex)
                .Select(x => x.GetAxisLimits());
            foreach (var limit in limits)
            {
                xMin = double.MaxValue == limit.m_xMin ? limit.m_xMin : Math.Min(xMin, limit.m_xMin);
                xMax = double.MinValue == limit.m_xMax ? limit.m_xMax : Math.Max(xMax, limit.m_xMax);
            }

            if (double.MaxValue == xMin || double.MinValue == xMax)
                return;

            axis.Dims.SetLimits(xMin, xMax);
        }

        public void AxisAutoY(Axis axis)
        {
            double yMin = double.MaxValue, yMax = double.MinValue;
            var limits = SeriesList
                .Where(x => x.YIndex == axis.AxisIndex)
                .Select(x => x.GetAxisLimits());
            foreach (var limit in limits)
            {
                yMin = double.MaxValue == limit.m_yMin ? limit.m_yMin : Math.Min(yMin, limit.m_yMin);
                yMax = double.MinValue == limit.m_yMax ? limit.m_yMax : Math.Max(yMax, limit.m_yMax);
            }

            if (double.MaxValue == yMin || double.MinValue == yMax)
                return;

            axis.Dims.SetLimits(yMin, yMax);
        }

        internal void SetAxisLimits(AxisLimits axisLimits, int xIndex, int yIndex)
        {
            GetXAxis(xIndex).Dims.SetLimits(axisLimits.m_xMin, axisLimits.m_xMax);
            GetYAxis(yIndex).Dims.SetLimits(axisLimits.m_yMin, axisLimits.m_yMax);
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

            float plotSize = availableSize / axisCount;

            float plotOffset = 0;
            plotOffset += PadTop;
            foreach (var axis in YAxes)
            {
                axis.Dims.Resize(height, plotSize, dataSize, PadTop, plotOffset);
                plotOffset += plotSize + AxisSpace;
            }
        }

        private void LayoutX(float width)
        {
            int axisCount = XAxes.Count;
            if (axisCount == 0) return;

            float totalSpacing = AxisSpace * (axisCount - 1);
            float dataSize = width - PadLeft - PadRight;
            float availableSize = dataSize - totalSpacing;

            float plotSize = availableSize / axisCount;

            float plotOffset = 0;
            plotOffset += PadLeft;
            foreach (var axis in XAxes)
            {
                axis.Dims.Resize(width, plotSize, dataSize, PadLeft, plotOffset);
                plotOffset += plotSize + AxisSpace;
            }
        }

        // TODO: 多个x轴和y轴应该有一个对应关系
        private PlotDimensions GetDimensions(int xIndex, int yIndex, float scale)
        {
            var xAxis = GetXAxis(xIndex);
            var yAxis = GetYAxis(yIndex);

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

        private void CalculateTicks(PlotDimensions dims)
        {
            foreach (var axis in Axes)
            {
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis.AxisIndex, 0, dims.m_scaleFactor) :
                    GetDimensions(0, axis.AxisIndex, dims.m_scaleFactor);
                axis.Generator.GetTicks(dims2);
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

        // TODO: IDEA 是否可以渲染成多个表
        private void RenderBeforePlot(Bitmap bmp, bool lowQuality, PlotDimensions dims)
        {
            Color dataAreaColor = Color.White;
            // set the background of data area
            using (var brush = GDI.Brush(dataAreaColor, 1))
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
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis.AxisIndex, 0, dims.m_scaleFactor) :
                   GetDimensions(0, axis.AxisIndex, dims.m_scaleFactor);

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

                PlotDimensions dims2 = GetDimensions(series.XIndex, series.YIndex, dims.m_scaleFactor);

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












        public SampleDataSeries AddDataStreamer(int xIndex, int yIndex)
        {
            if (!ExistXAxis(xIndex))
                AddXAxis(xIndex);
            if (!ExistYAxis(yIndex))
                AddYAxis(yIndex);

            SampleDataSeries dataStreamSeries = new SampleDataSeries(xIndex, yIndex, this);
            SeriesList.Add(dataStreamSeries);
            return dataStreamSeries;
        }

        public void ClearSeries()
        {
            SeriesList.Clear();
        }









        public float X { get; private set; }
        public float Y { get; private set; }

        private bool m_leftPressed = false;
        private bool m_rightPressed = false;
        private bool m_ctrlPressed = false;
        private bool m_shiftPressed = false;
        private bool m_altPressed = false;


        /////////////////////PlotEvent//////////////////////
        public void MouseDown(InputState inputState)
        {
            X = inputState.m_x;
            Y = inputState.m_y;
            m_leftPressed = inputState.m_leftPressed;
            m_rightPressed = inputState.m_rightPressed;
            m_ctrlPressed = inputState.m_controlPressed;
            m_shiftPressed = inputState.m_shiftPressed;
            m_altPressed = inputState.m_altPressed;

            foreach (var axis in Axes)
            {
                axis.Dims.Remember();
            }
        }

        public void MouseUp(InputState inputState)
        {
            m_leftPressed = false;
            m_rightPressed = false;
            m_ctrlPressed = false;
            m_shiftPressed = false;
            m_altPressed = false;
        }

        public void MouseMove(InputState inputState)
        {
            PlotEvent plotEvent = null;
            if (m_leftPressed)
                plotEvent = m_eventFactory.CreateMousePanEvent(inputState);
            else if (m_rightPressed)
                plotEvent = m_eventFactory.CreateMouseZoomEvent(inputState);


            if (plotEvent != null)
                ProcessEvent(plotEvent);
        }

        private void ProcessEvent(PlotEvent plotEvent)
        {
            plotEvent.Process();

            Render();
        }

        internal void PanAll(float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.Dims.Recall();

                // do something
                if (axis.IsHorizontal)
                    axis.Dims.PanPx(x - X);
                else
                    axis.Dims.PanPx(y - Y);
            }
        }

        internal void ZoomCenter(float xfrac, float yfrac, float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.Dims.Recall();

                float frac = axis.IsHorizontal ? xfrac : yfrac;
                float centerPx = axis.IsHorizontal ? x : y;
                float center = axis.Dims.GetUnit(centerPx);
                if (float.IsNaN(frac) || frac == 1.0f || float.IsNaN(center))
                    return;

                axis.Dims.Zoom(frac, center);
            }
        }

        internal void ZoomCenter(float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.Dims.Recall();

                // do something
                // TODO: 
                float deltaPx = axis.IsHorizontal ? x - X : Y - y;
                float delta = deltaPx * axis.Dims.UnitsPerPx;

                float deltaFrac = delta / (Math.Abs(delta) + axis.Dims.Center);

                float frac = (float)Math.Pow(10, deltaFrac);
                if (float.IsNaN(frac) || frac == 1.0f)
                    return;

                axis.Dims.Zoom(frac);
            }
        }

        public void MouseDoubleClick(InputState inputState)
        {
        }

        public void MouseWheel(InputState inputState)
        {
            PlotEvent plotEvent = m_eventFactory.CreateMouseScrollEvent(inputState);
        }
    }
}
