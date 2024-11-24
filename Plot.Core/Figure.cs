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

    public class Figure
    {
        private Bitmap m_bmp;
        private readonly Bitmap[] m_oldBitmaps = new Bitmap[3]; // 固定大小为3的数组
        private int m_currentIndex = 0; // 当前索引

        private readonly EventFactory m_eventFactory;

        private readonly System.Diagnostics.Stopwatch m_stopwatch;

        private List<Axis> Axes { get; } = new List<Axis>()
        {
           new TopAxis() { Title = "Top",},
           new BottomAxis(){ Title = "Bottom",},
           new LeftAxis(){ Title = "Left",},
           new RightAxis(){ Title = "Right",},
        };

        public List<Axis> TopAxes => Axes.Where(x => x.Edge == Edge.Top).ToList();
        public List<Axis> BottomAxes => Axes.Where(x => x.Edge == Edge.Bottom).ToList();
        public List<Axis> LeftAxes => Axes.Where(x => x.Edge == Edge.Left).ToList();
        public List<Axis> RightAxes => Axes.Where(x => x.Edge == Edge.Right).ToList();

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
        public float AxisSpace { get; set; } = 5;

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;

        public Axis AddAxes(Edge edge)
        {
            Axis axis = AxisFactory.CreateAxis(edge);
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
            //if (width - PadLeft - PadRight < 1) width = PadLeft + PadRight + AxisSpace * (XAxes.Count - 1) + 100;
            //if (height - PadTop - PadBottom < 1) height = PadTop + PadBottom + AxisSpace * (YAxes.Count - 1) + 100;
            if (width < 10 || height < 10) return;

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
                var primaryDims = GetDimensions(BottomAxes[0], LeftAxes[0], scale);

                AutoScaleByPlot();
                CalculateTicks(primaryDims);

                RenderClear(m_bmp, lowQuality, primaryDims);
                RenderBeforePlot(m_bmp, lowQuality, primaryDims);
                RenderPlot(m_bmp, lowQuality, primaryDims);
                RenderAfterPlot(m_bmp, lowQuality, primaryDims);

                OnBitmapUpdated?.Invoke(null, null);
            }
        }

        private void Layout(float width, float height)
        {
            // TODO: 自动计算PadLeft, PadRight, PadTop, PadBottom
            ArrangeAxes(width, TopAxes, PadLeft, PadRight);
            ArrangeAxes(width, BottomAxes, PadLeft, PadRight);
            ArrangeAxes(height, LeftAxes, PadTop, PadBottom);
            ArrangeAxes(height, RightAxes, PadTop, PadBottom);
        }

        private void AutoScaleByPlot()
        {
            double xmin = double.MaxValue, xmax = double.MinValue;
            double ymin = double.MaxValue, ymax = double.MinValue;
            var limits = SeriesList
                .Where(x => !x.XAxis.HasBeenSet() || !x.YAxis.HasBeenSet())
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
                series.XAxis.SetLimit((float)xmin, (float)xmax);
                series.YAxis.SetLimit((float)ymin, (float)ymax);
            }
        }

        private void CalculateTicks(PlotDimensions dims)
        {
            foreach (var axis in Axes)
            {
                PlotDimensions dims2 = axis.IsHorizontal ? GetDimensions(axis, LeftAxes[0], dims.m_scaleFactor) :
                    GetDimensions(BottomAxes[0], axis, dims.m_scaleFactor);
                axis.GetTicks(dims2);
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
                axis.Resize(px, plotSize, dataSize, p1, plotOffset);
                plotOffset += plotSize + AxisSpace;
            }
        }

        // TODO: 多个x轴和y轴应该有一个对应关系
        private static PlotDimensions GetDimensions(Axis xAxis, Axis yAxis, float scale)
        {
            SizeF figureSize = new SizeF(xAxis.FigureSizePx, yAxis.FigureSizePx);
            SizeF plotSize = new SizeF(xAxis.DataSizePx, yAxis.DataSizePx);
            SizeF dataSize = new SizeF(xAxis.PlotSizePx, yAxis.PlotSizePx);
            PointF plotOffset = new PointF(xAxis.PlotOffsetPx, yAxis.PlotOffsetPx);
            PointF dataOffset = new PointF(xAxis.DataOffsetPx, yAxis.DataOffsetPx);

            (float xMin, float xMax) = xAxis.GetLimit();
            (float yMin, float yMax) = yAxis.GetLimit();


            return new PlotDimensions(figureSize,
                dataSize,
                plotSize,
                plotOffset,
                dataOffset,
                ((xMin, xMax), (yMin, yMax)),
                scale,
                xAxis.IsInverted, yAxis.IsInverted);

        }







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
                axis.MouseDown(inputState.m_x, inputState.m_y);
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

        public void MouseDoubleClick(InputState inputState)
        {
        }

        public void MouseWheel(InputState inputState)
        {
            foreach (var axis in Axes)
            {
                axis.MouseDown(inputState.m_x, inputState.m_y);
            }

            PlotEvent plotEvent = m_eventFactory.CreateMouseScrollEvent(inputState);
            ProcessEvent(plotEvent);
        }

        internal void PanAll(float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.PanAll(x, y);
            }
        }

        internal void ZoomCenter(float xfrac, float yfrac, float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.ZoomByPosition(xfrac, yfrac, x, y);
            }
        }

        internal void ZoomCenter(float x, float y)
        {
            foreach (var axis in Axes)
            {
                axis.ZoomByCenter(x, y);
            }
        }


    }
}
