using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class SampleDataSeries
    {
        public SampleDataSeries(int xIndex, int yIndex, Figure figure)
        {
            XIndex = xIndex;
            YIndex = yIndex;
            Figure = figure;
        }

        public int XIndex { get; }
        public int YIndex { get; }
        public Figure Figure { get; }

        public Color Color { get; set; } = Color.Red;
        public float LineWidth { get; set; } = 1f;
        public string Label { get; set; } = null;

        public long NextIndex { get; private set; } = 0;
        public double[] Data { get; private set; }
        public double[] XAxisValues { get; private set; }

        public double DataMin { get; private set; } = double.MaxValue;
        public double DataMax { get; private set; } = double.MinValue;

        public float SampleRate { get; set; } = 1f;
        public float SampleInterval => 1f / SampleRate;


        public int AddSamples(double[] samples)
        {
            List<double> data = new List<double>(samples.Length);
            List<double> xValues = new List<double>(samples.Length); // 临时存储 X 轴值
            foreach (var sample in samples)
            {
                // TODO: Apply filters, Validators, etc.
                data.Add(sample);
                xValues.Add(NextIndex * SampleInterval); // 计算相应的 X 轴值
                NextIndex++; // 增加样本索引

                DataMin = Math.Min(DataMin, sample);
                DataMax = Math.Max(DataMax, sample);
            }

            Data = data.ToArray();
            XAxisValues = xValues.ToArray(); // 更新 X 轴值数组

            return Data.Length;
        }

        public AxisLimits GetAxisLimits()
        {
            //double xMin = XAxisValues.Min();
            //double xMax = XAxisValues.Max();
            double yMin = DataMin;
            double yMax = DataMax;

            // TODO: 限制溢出Plot范围
            // DONE: clip the outer range
            return new AxisLimits((int)(NextIndex / SampleRate) - 1, (int)(NextIndex / SampleRate), -1, 1);
        }

        public void AxisAuto()
        {
            // Axis scaling
            Figure.SetAxisLimits(GetAxisLimits(), XIndex, YIndex);
        }

        public void Render(Bitmap bmp, PlotDimensions dims, bool lowQuailty)
        {
            if (Data.Length == 0 || XAxisValues.Length == 0) return;

            var xs = XAxisValues.Select(i => dims.GetPixelX((float)i)).ToArray();
            var ys = Data.Select(i => dims.GetPixelY((float)i)).ToArray();

            List<PointF> points = new List<PointF>();
            for (int i = 0; i < xs.Length; i++)
            {
                //if (ys[i] < dims.DataOffsetY || ys[i] > (dims.PlotOffsetY + dims.PlotHeight))
                //    continue;

                //if (xs[i] < dims.DataOffsetX || xs[i] > (dims.PlotOffsetX + dims.PlotWidth))
                //    continue;

                points.Add(new PointF(xs[i], ys[i]));
            }

            using (var pen = GDI.Pen(Color, LineWidth))
            using (var gfx = GDI.Graphics(bmp, dims, lowQuailty, true))
            {
                gfx.DrawLines(pen, points.ToArray());
            }
        }
    }
}
