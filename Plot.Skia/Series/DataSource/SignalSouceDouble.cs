using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class SignalSouceDouble : ISignalSource
    {
        private readonly IList<double> m_ys;
        private int MinRenderringIndex => Math.Max(0, MinimumIndex);
        private int MaxRenderringIndex => Math.Min(Length - 1, MaximumIndex);

        public SignalSouceDouble(IList<double> ys, double sampleInterval)
        {
            m_ys = ys;
            SampleInterval = sampleInterval;
            MinimumIndex = 0;
            MaximumIndex = int.MaxValue;
        }

        public int Length => m_ys.Count;
        public double SampleInterval { get; set; }
        public int MinimumIndex { get; set; }
        public int MaximumIndex { get; set; }
        public double XOffset { get; set; }

        public void Add(double val)
        {
            m_ys.Add(val);
        }

        public void AddRange(IEnumerable<double> vals)
        {
            foreach (double val in vals)
                Add(val);
        }

        public int GetIndex(double x)
        {
            // 第0个单位需要0个点
            // 第1个单位需要1 * sampleRate个点....
            int i = (int)(x / SampleInterval);

            {
                i = Math.Max(i, MinRenderringIndex);
                i = Math.Min(i, MaxRenderringIndex);
            }

            return i;
        }

        public double GetX(int index)
            // index个点过去了多少单位
            => index * SampleInterval;

        public double GetY(int index)
            => m_ys[index];

        public IEnumerable<double> GetYs()
            => GetYs(MinRenderringIndex, MaxRenderringIndex);

        public IEnumerable<double> GetYs(int startIndex, int endIndex)
        {
            startIndex = Math.Max(startIndex, MinRenderringIndex);
            endIndex = Math.Min(endIndex, MaxRenderringIndex);

            for (int i = startIndex; i <= endIndex; i++)
                yield return m_ys[i];
        }

        public RangeMutable GetXLimit()
           // 1000个点，1hz的频率（时间 = 1/1hz)，需要1000个单位
           => new RangeMutable(MinRenderringIndex * SampleInterval,
               MaxRenderringIndex * SampleInterval);

        public RangeMutable GetYLimit()
            => GetYLimit(MinRenderringIndex, MaxRenderringIndex);

        // TODO: 优化速度
        public RangeMutable GetYLimit(int startIndex, int endIndex)
        {
            double min = double.PositiveInfinity, max = double.NegativeInfinity;
            for (int i = startIndex; i <= endIndex; i++)
            {
                min = Math.Min(min, m_ys[i]);
                max = Math.Max(max, m_ys[i]);
            }

            return new RangeMutable(min, max);
        }

    }
}
