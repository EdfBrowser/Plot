using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    public class SignalSourceDouble : ISignalSource
    {
        private class DataChunk
        {
            public double[] Data { get; }
            public int Count { get; set; }
            public double Min { get; set; } = double.PositiveInfinity;
            public double Max { get; set; } = double.NegativeInfinity;

            public DataChunk(int size)
            {
                Data = new double[size];
            }
        }

        private const int ChunkSize = 5000;
        private const int MaxChunks = 4;

        private readonly LinkedList<DataChunk> _chunks = new LinkedList<DataChunk>();
        // 当前存储的最旧数据全局索引
        private int _globalStartIndex = 0;
        // 当前块中存储数据的索引
        private int _totalCount = 0;
        private double _globalMin = double.PositiveInfinity;
        private double _globalMax = double.NegativeInfinity;

        public SignalSourceDouble(double sampleInterval)
        {
            SampleInterval = sampleInterval;
            MinimumIndex = 0;
            MaximumIndex = int.MaxValue;
        }

        public int Length => _globalStartIndex + _totalCount;
        public double SampleInterval { get; set; }
        public int MinimumIndex { get; set; }
        public int MaximumIndex { get; set; }

        public void Add(double val)
        {
            DataChunk currentChunk = _chunks.Last?.Value;
            if (currentChunk == null || currentChunk.Count >= ChunkSize)
            {
                currentChunk = new DataChunk(ChunkSize);
                _chunks.AddLast(currentChunk);

                if (_chunks.Count > MaxChunks)
                {
                    DataChunk oldestChunk = _chunks.First.Value;
                    _chunks.RemoveFirst();
                    _globalStartIndex += oldestChunk.Count;  // 更新全局起始索引
                    _totalCount -= oldestChunk.Count;

                    // 如果被删除块包含全局极值，重新计算全局极值
                    if (oldestChunk.Min == _globalMin || oldestChunk.Max == _globalMax)
                        RecalculateGlobalMinMax();
                }
            }

            // 更新当前块的极值
            currentChunk.Data[currentChunk.Count] = val;
            currentChunk.Count++;
            currentChunk.Min = Math.Min(currentChunk.Min, val);
            currentChunk.Max = Math.Max(currentChunk.Max, val);

            // 更新全局极值
            _globalMin = Math.Min(_globalMin, val);
            _globalMax = Math.Max(_globalMax, val);
            _totalCount++;
        }

        public void AddRange(IEnumerable<double> vals)
        {
            foreach (double val in vals)
                Add(val);
        }

        private void RecalculateGlobalMinMax()
        {
            _globalMin = double.PositiveInfinity;
            _globalMax = double.NegativeInfinity;
            foreach (DataChunk chunk in _chunks)
            {
                _globalMin = Math.Min(_globalMin, chunk.Min);
                _globalMax = Math.Max(_globalMax, chunk.Max);
            }
        }

        public int GetIndex(double x)
        {
            int index = (int)(x / SampleInterval);
            return NumericConversion.Clamp(index, MinRenderringIndex, MaxRenderringIndex);
        }

        public double GetX(int index) => index * SampleInterval;

        public double GetY(int index)
        {
            if (index < _globalStartIndex || index >= _globalStartIndex + _totalCount)
                throw new ArgumentOutOfRangeException(nameof(index));

            int internalIndex = index - _globalStartIndex;
            int chunkIndex = internalIndex / ChunkSize;
            int chunkOffset = internalIndex % ChunkSize;

            return _chunks
                .Skip(chunkIndex)
                .First()
                .Data[chunkOffset];
        }

        public IEnumerable<double> GetYs() => GetYs(MinRenderringIndex, MaxRenderringIndex);

        public IEnumerable<double> GetYs(int startIndex, int endIndex)
        {
            startIndex = Math.Max(startIndex, MinRenderringIndex);
            endIndex = Math.Min(endIndex, MaxRenderringIndex);

            for (int i = startIndex; i <= endIndex; i++)
                yield return GetY(i);
        }

        private int MinRenderringIndex => Math.Max(_globalStartIndex, MinimumIndex);
        private int MaxRenderringIndex => Math.Min(_globalStartIndex + _totalCount - 1, MaximumIndex);

        public RangeMutable GetXLimit() => new RangeMutable(
            MinRenderringIndex * SampleInterval,
            MaxRenderringIndex * SampleInterval
        );

        public RangeMutable GetYLimit() => GetYLimit(MinRenderringIndex, MaxRenderringIndex);

        public RangeMutable GetYLimit(int startIndex, int endIndex)
        {
            // 全局范围直接返回缓存极值
            if (startIndex <= _globalStartIndex && endIndex >= _globalStartIndex + _totalCount - 1)
                return new RangeMutable(_globalMin, _globalMax);

            // 局部范围实时计算
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            int startChunk = (startIndex - _globalStartIndex) / ChunkSize;
            int endChunk = (endIndex - _globalStartIndex) / ChunkSize;

            foreach (DataChunk chunk in _chunks.Skip(startChunk).Take(endChunk - startChunk + 1))
            {
                int chunkStart = Math.Max(startIndex - _globalStartIndex - startChunk * ChunkSize, 0);
                int chunkEnd = Math.Min(endIndex - _globalStartIndex - startChunk * ChunkSize, ChunkSize - 1);

                for (int i = chunkStart; i <= chunkEnd; i++)
                {
                    if (i >= chunk.Count) break;
                    double val = chunk.Data[i];
                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }

            return new RangeMutable(min, max);
        }
    }
}
