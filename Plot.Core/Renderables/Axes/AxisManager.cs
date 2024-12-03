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
        private List<Axis> Axes { get; } = new List<Axis>();
        private List<Axis> TopAxes => Axes.Where(x => x.Edge == Edge.Top).ToList();
        private List<Axis> BottomAxes => Axes.Where(x => x.Edge == Edge.Bottom).ToList();
        private List<Axis> LeftAxes => Axes.Where(x => x.Edge == Edge.Left).ToList();
        private List<Axis> RightAxes => Axes.Where(x => x.Edge == Edge.Right).ToList();

        public Axis DefaultXAxis => BottomAxes[0];
        public Axis DefaultYAxis => LeftAxes[0];

        public float AxisSpace { get; set; } = 20;

        public void CreateDefaultAxes()
        {
            Axes.Add(CreateAxis(Edge.Top));
            Axes.Add(CreateAxis(Edge.Bottom));
            Axes.Add(CreateAxis(Edge.Left));
            Axes.Add(CreateAxis(Edge.Right));
        }

        public Axis AddAxes(Edge edge)
        {
            Axis axis = CreateAxis(edge);
            Axes.Add(axis);
            return axis;
        }


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


        // TODO: 减少PlotDimensions复制次数，能否跟Series那样
        public void RenderAxes(Bitmap bmp, bool lowQuality, float scale)
        {
            foreach (var axis in Axes)
            {
                PlotDimensions dims = axis.IsHorizontal ? axis.CreatePlotDimensions(LeftAxes[0], scale)
                    : BottomAxes[0].CreatePlotDimensions(axis, scale);

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
            foreach (var axis in Axes)
                axis.Dims.SuspendLimits();
        }

        public void ResumeLimits()
        {
            foreach (var axis in Axes)
                axis.Dims.ResumeLimits();
        }

        public void PanAll(double x, double y)
        {
            foreach (var axis in Axes)
            {
                if (axis.IsHorizontal)
                    axis.Dims.PanPx(x);
                else
                    axis.Dims.PanPx(y);
            }
        }

        public void ZoomByFrac(double xfrac, double yfrac, float x, float y)
        {
            foreach (var axis in Axes)
            {
                double frac = axis.IsHorizontal ? xfrac : yfrac;
                float centerPx = axis.IsHorizontal ? x : y;
                double center = axis.Dims.GetUnit(centerPx);

                axis.Dims.Zoom(frac, center);
            }
        }

        public void ZoomByXY(float x, float y, float oldestX, float oldestY)
        {
            foreach (var axis in Axes)
            {
                float deltaPx = axis.IsHorizontal ? x - oldestX : oldestY - y;
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

            SizeF figureSizPx = new SizeF(width, height);

            {
                foreach (Axis axis in Axes)
                {
                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dimsFull = new PlotDimensions(figureSizPx, figureSizPx, figureSizPx,
                                                new PointF(0, 0), new PointF(0, 0),
                                                (xAxis.Dims.RationalLimits(), yAxis.Dims.RationalLimits()),
                                                1f, xAxis.Dims.IsInverted, yAxis.Dims.IsInverted);
                    CalculatePadding(dimsFull, axis);
                }

                RecalculateDataPadding(width, height);
            }

            {
                foreach (Axis axis in Axes)
                {
                    GetPrimayAxis(axis, out Axis xAxis, out Axis yAxis);

                    PlotDimensions dims = xAxis.CreatePlotDimensions(yAxis, 1f);
                    CalculatePadding(dims, axis);
                }

                RecalculateDataPadding(width, height);
            }
        }

        private void GetPrimayAxis(Axis axis, out Axis xAxis, out Axis yAxis)
        {
            if (axis.IsHorizontal)
            {
                xAxis = axis;
                yAxis = DefaultYAxis;
            }
            else if (axis.IsVertical)
            {
                xAxis = DefaultXAxis;
                yAxis = axis;
            }
            else
            {
                throw new NotImplementedException($"unsupported edge type {axis.Edge}");
            }
        }

        private void CalculatePadding(PlotDimensions dims, Axis axis)
        {
            axis.RecalculateTickPositions(dims);
            axis.ReCalculateAxisSize();
        }

        private void RecalculateDataPadding(float width, float height)
        {
            float PadLeft = LeftAxes.Select(t => t.GetSize()).Max();
            float PadRight = RightAxes.Select(t => t.GetSize()).Max();
            float PadTop = TopAxes.Select(t => t.GetSize()).Max();
            float PadBottom = BottomAxes.Select(t => t.GetSize()).Max();

            ArrangeAxes(width, TopAxes, PadLeft, PadRight);
            ArrangeAxes(width, BottomAxes, PadLeft, PadRight);
            ArrangeAxes(height, LeftAxes, PadTop, PadBottom);
            ArrangeAxes(height, RightAxes, PadTop, PadBottom);
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
        #endregion
    }

}
