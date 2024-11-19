using System;
using System.Drawing;

namespace Plot.Core
{
    public class DataStreamSeries : BaseSeries
    {
        public DataStreamSeries(int xIndex, int yIndex, Figure figure, double[] data)
            : base(xIndex, yIndex)
        {
            Data = data;
            Figure = figure;
        }

        public Figure Figure { get; }
        public double[] Data { get; }

        public double OffsetX { get; set; } = 0;
        public double OffsetY { get; set; } = 0;

        public double XSpacing { get; set; } = 1.0;
        public double SampleRate
        {
            get => 1.0 / XSpacing;
            set => XSpacing = 1.0 / value;
        }


        public int NextIndex { get; private set; } = 0;

        public double DataMin { get; private set; } = double.PositiveInfinity;
        public double DataMax { get; private set; } = double.NegativeInfinity;

        public void Add(double value)
        {
            Data[NextIndex++] = value;

            if (NextIndex >= Data.Length)
                NextIndex = 0;

            DataMin = Math.Min(DataMin, value);
            DataMax = Math.Max(DataMax, value);
        }

        public void AxisAuto()
        {
            double xMin = OffsetX;
            double xMax = Data.Length * XSpacing + OffsetX;
            double yMin = DataMin + OffsetY;
            double yMax = DataMax + OffsetY;

            //Figure.XAxis.AxisLimits(xMin, xMax);
            //if (yMin != double.PositiveInfinity && yMax != double.NegativeInfinity)
            //    Figure.Axes[YIndex].AxisLimits(yMin, yMax);
        }

        public override void Render(Bitmap bmp)
        {
            //// axis auto scaling
            //AxisAuto();

            //// render data
            //int newestCount = NextIndex;
            //int oldestCount = Data.Length - newestCount;
            //double xMax = Data.Length * XSpacing + OffsetX;
            //PointF[] newest = new PointF[newestCount];
            //PointF[] oldest = new PointF[oldestCount];

            //for (int i = 0; i < newest.Length; i++)
            //{
            //    double xPos = i * XSpacing + OffsetX;
            //    float x = Figure.XAxis.GetPixel(xPos);
            //    float y = Figure.Axes[YIndex].GetPixel(Data[i] + OffsetY) + Figure.Axes[YIndex].GetOffsetPixel();
            //    newest[i] = new PointF(x, y);
            //}

            //for (int i = 0; i < oldest.Length; i++)
            //{
            //    double xPos = (i + NextIndex) * XSpacing + OffsetX;
            //    float x = Figure.XAxis.GetPixel(xPos);
            //    float y = Figure.Axes[YIndex].GetPixel(Data[i + NextIndex] + OffsetY) + Figure.Axes[YIndex].GetOffsetPixel();
            //    oldest[i] = new PointF(x, y);
            //}

            //using (var gfx = GDI.Graphics(m_bmp))
            //using (var pen = GDI.Pen(Color, LineWidth, 1))
            //{
            //    if (oldest.Length > 1)
            //        gfx.DrawLines(pen, oldest);
            //    if (newest.Length > 1)
            //        gfx.DrawLines(pen, newest);
            //}
        }
    }
}
