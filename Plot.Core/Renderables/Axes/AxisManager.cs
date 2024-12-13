using Plot.Core.Enum;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Renderables.Axes
{
    public class AxisManager
    {
        private readonly List<Axis> m_axes;

        public AxisManager()
        {
            m_axes = new List<Axis>();

            CreateDefaultAxes();
        }

        public float AxisSpace { get; set; } = 20;

        private void CreateDefaultAxes()
        {
            m_axes.Add(CreateAxis(Edge.Bottom));
            m_axes.Add(CreateAxis(Edge.Left));
        }

        public IEnumerable<Axis> TopAxes() => m_axes.Where(x => x.Edge == Edge.Top);
        public IEnumerable<Axis> BottomAxes() => m_axes.Where(x => x.Edge == Edge.Bottom);
        public IEnumerable<Axis> LeftAxes() => m_axes.Where(x => x.Edge == Edge.Left);
        public IEnumerable<Axis> RightAxes() => m_axes.Where(x => x.Edge == Edge.Right);

        public Axis AddAxes(Edge edge)
        {
            Axis axis = CreateAxis(edge);
            m_axes.Add(axis);
            return axis;
        }

        public void ClearYAxes() => m_axes.RemoveAll(x => x.Edge == Edge.Left);

        private static Axis CreateAxis(Edge edge)
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

        public Axis GetDefaultXAxis()
        {
            Axis axis = BottomAxes().FirstOrDefault();
            return axis ?? throw new ArgumentNullException($"{nameof(BottomAxes)} Collection don`t any items");
        }

        public Axis GetDefaultYAxis()
        {
            Axis axis = LeftAxes().FirstOrDefault();
            return axis ?? throw new ArgumentNullException($"{nameof(LeftAxes)} Collection don`t any items");
        }

        // TODO: 减少PlotDimensions复制次数，能否跟Series那样
        public void RenderAxes(Bitmap bmp, bool lowQuality, float scale)
        {
            foreach (var axis in m_axes)
            {
                PlotDimensions dims = axis.IsHorizontal ? axis.CreatePlotDimensions(GetDefaultYAxis(), scale)
                    : GetDefaultXAxis().CreatePlotDimensions(axis, scale);

                try
                {
                    axis.Render(bmp, dims, lowQuality);
                }
                catch (OverflowException)
                {
                    Debug.WriteLine($"OverflowException plotting: {axis}");
                }
            }
        }

        #region Mouse even
        public void SuspendLimits()
        {
            foreach (var axis in m_axes)
                axis.Dims.SuspendLimits();
        }

        public void ResumeLimits()
        {
            foreach (var axis in m_axes)
                axis.Dims.ResumeLimits();
        }

        public void PanAll(double x, double y)
        {
            foreach (var axis in m_axes)
            {
                if (axis.IsHorizontal)
                    axis.Dims.PanPx(x);
                else
                    axis.Dims.PanPx(y);
            }
        }

        public void ZoomByFrac(double xfrac, double yfrac, float x, float y)
        {
            foreach (var axis in m_axes)
            {
                double frac = axis.IsHorizontal ? xfrac : yfrac;
                float centerPx = axis.IsHorizontal ? x : y;
                double center = axis.Dims.GetUnit(centerPx);

                axis.Dims.Zoom(frac, center);
            }
        }

        public void ZoomByXY(float xDeltaPx, float yDeltaPx)
        {
            foreach (var axis in m_axes)
            {
                float deltaPx = axis.IsHorizontal ? xDeltaPx : yDeltaPx;
                double delta = deltaPx * axis.Dims.UnitsPerPx;

                double deltaFrac = delta / (Math.Abs(delta) + axis.Dims.Center);

                double frac = Math.Pow(10, deltaFrac);

                axis.Dims.Zoom(frac);
            }
        }
        #endregion

        // TODO: 扩展成Layout 类
        #region Layout
        public void Layout(float width, float height)
        {
            // tick 的密度与axis size有关
            // axis size 与 pad有关
            // pad 大小与label有关
            // label 大小与tick有关
            // 先有鸡还是先有蛋
            // 假设pad为0，没得label，先计算出 tick label的大小


            {
                SizeF figureSizPx = new SizeF(width, height);

                foreach (Axis axis in m_axes)
                {
                    // 初始化min和max值
                    (double min, double max) = axis.Dims.GetLimits();
                    axis.Dims.SetLimits(min, max);

                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dimsFull = new PlotDimensions(figureSizPx, figureSizPx, figureSizPx,
                                                new PointF(0, 0), new PointF(0, 0),
                                                (xAxis.Dims.GetLimits(), yAxis.Dims.GetLimits()),
                                                1f, xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);
                    CalculateOffset(dimsFull, axis);
                }

                RecalculateDataOffset(width, height);
            }

            {
                foreach (Axis axis in m_axes)
                {
                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dims = xAxis.CreatePlotDimensions(yAxis, 1f);
                    CalculateOffset(dims, axis);
                }

                RecalculateDataOffset(width, height);
            }
        }

        private void GetPrimayAxis(Axis axis, out Axis xAxis, out Axis yAxis)
        {
            if (axis.IsHorizontal)
            {
                xAxis = axis;
                yAxis = GetDefaultYAxis();
            }
            else if (axis.IsVertical)
            {
                xAxis = GetDefaultXAxis();
                yAxis = axis;
            }
            else
            {
                throw new NotImplementedException($"unsupported edge type {axis.Edge}");
            }
        }

        private void CalculateOffset(PlotDimensions dims, Axis axis)
        {
            axis.RecalculateTickPositions(dims);
            axis.ReCalculateAxisSize();
        }

        private void RecalculateDataOffset(float width, float height)
        {
            IEnumerable<Axis> left = LeftAxes();
            IEnumerable<Axis> right = RightAxes();
            IEnumerable<Axis> bottom = BottomAxes();
            IEnumerable<Axis> top = TopAxes();

            float leftOffset = 10f, rightOffset = 10f, topOffset = 10f, bottomOffset = 10f;

            if (left.Any())
                leftOffset = left.Select(t => t.GetSize()).Max();
            if (right.Any())
                rightOffset = right.Select(t => t.GetSize()).Max();
            if (bottom.Any())
                bottomOffset = bottom.Select(t => t.GetSize()).Max();
            if (top.Any())
                topOffset = top.Select(t => t.GetSize()).Max();

            ArrangeAxes(width, TopAxes(), leftOffset, rightOffset);
            ArrangeAxes(width, BottomAxes(), leftOffset, rightOffset);
            ArrangeAxes(height, LeftAxes(), topOffset, bottomOffset);
            ArrangeAxes(height, RightAxes(), topOffset, bottomOffset);
        }

        private void ArrangeAxes(float px, IEnumerable<Axis> axes, float p1, float p2)
        {
            int axisCount = axes.Count();
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

        #endregion



        public void SetGrid(bool enable)
        {
            foreach (Axis axis in m_axes)
            {
                axis.AxisTick.MajorGridVisible = enable;
                axis.AxisTick.MinorGridVisible = enable;
            }
        }
    }

}
