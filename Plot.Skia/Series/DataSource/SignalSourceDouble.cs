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

        private readonly int _chunkSize;
        private const int MaxChunks = 2;

        private readonly LinkedList<DataChunk> _chunks = new LinkedList<DataChunk>();
        // 当前存储的最旧数据全局索引
        private int _globalStartIndex = 0;
        // 当前块中存储数据的索引
        private int _totalCount = 0;
        private double _globalMin = double.PositiveInfinity;
        private double _globalMax = double.NegativeInfinity;

        private int MinRenderringIndex => Math.Max(_globalStartIndex, MinimumIndex);
        private int MaxRenderringIndex => Math.Min(_globalStartIndex + _totalCount - 1, MaximumIndex);

        public SignalSourceDouble(double sampleInterval)
        {
            SampleInterval = sampleInterval;
            MinimumIndex = 0;
            MaximumIndex = int.MaxValue;

            _chunkSize = (int)(1.0 / sampleInterval);
        }

        public int Length => _globalStartIndex + _totalCount;
        public double SampleInterval { get; set; }
        public int MinimumIndex { get; set; }
        public int MaximumIndex { get; set; }

        // ==============Add data==============
        public void AddRange(IEnumerable<double> vals)
        {
            IEnumerable<DataChunk> chunks = CreateChunks(vals);

            foreach (DataChunk chunk in chunks)
            {
                _chunks.AddLast(chunk);
                _totalCount += chunk.Count;

                MaintainChunkCount(true);
                UpdateGlobalExtremes(chunk);
            }
        }

        // ==============Prepend data===========
        public void PrependRange(IEnumerable<double> vals)
        {
            var reversedChunks = CreateChunks(vals.Reverse());

            foreach (var chunk in reversedChunks)
            {
                _chunks.AddFirst(chunk);
                _totalCount += chunk.Count;

                MaintainChunkCount(false);
                UpdateGlobalExtremes(chunk);
            }
        }


        private IEnumerable<DataChunk> CreateChunks(IEnumerable<double> vals)
        {
            List<double> buffer = new List<double>();

            foreach (double val in vals)
            {
                buffer.Add(val);

                if (buffer.Count == _chunkSize)
                {
                    yield return CreateChunk(buffer);
                    buffer.Clear();
                }
            }

            // 
            if (buffer.Count > 0)
            {
                yield return CreateChunk(buffer);
            }
        }

        private DataChunk CreateChunk(List<double> data)
        {
            DataChunk chunk = new DataChunk(_chunkSize);
            data.CopyTo(chunk.Data);
            chunk.Count = data.Count;
            chunk.Min = data.Min();
            chunk.Max = data.Max();

            return chunk;
        }


        private void MaintainChunkCount(bool isAdd)
        {
            if (_chunks.Count > MaxChunks)
            {
                LinkedListNode<DataChunk> nodeToRemove = isAdd
                    ? _chunks.First
                    : _chunks.Last;

                DataChunk removedChunk = nodeToRemove.Value;
                _chunks.Remove(removedChunk);

                _totalCount -= removedChunk.Count;
                if (isAdd)
                {
                    _globalStartIndex += removedChunk.Count;
                }
                else
                {
                    _globalStartIndex -= removedChunk.Count;
                    _globalStartIndex = _globalStartIndex < 0 ? 0 : _globalStartIndex;
                }

                if (removedChunk.Min == _globalMin || removedChunk.Max == _globalMax)
                    RecalculateGlobalMinMax();
            }
        }

        private void UpdateGlobalExtremes(DataChunk chunk)
        {
            _globalMin = Math.Min(_globalMin, chunk.Min);
            _globalMax = Math.Max(_globalMax, chunk.Max);
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
            int chunkIndex = internalIndex / _chunkSize;
            int chunkOffset = internalIndex % _chunkSize;

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


        public RangeMutable GetXLimit() => new RangeMutable(
            MinRenderringIndex * SampleInterval,
            MaxRenderringIndex * SampleInterval
        );

        public RangeMutable GetYLimit() => GetYLimit(MinRenderringIndex, MaxRenderringIndex);

        public RangeMutable GetYLimit(int startIndex, int endIndex)
        {
            // 当前索引是一整块，直接返回缓存极值
            if (startIndex <= _globalStartIndex && endIndex >= _globalStartIndex + _totalCount - 1)
                return new RangeMutable(_globalMin, _globalMax);

            // 当前索引在一整块内或者占据在不同块，局部范围实时计算
            double min = double.PositiveInfinity;
            double max = double.NegativeInfinity;

            int startChunk = (startIndex - _globalStartIndex) / _chunkSize;
            int endChunk = (endIndex - _globalStartIndex) / _chunkSize;

            //
            foreach (DataChunk chunk in _chunks.Skip(startChunk).Take(endChunk - startChunk + 1))
            {
                int chunkStart = Math.Max(startIndex - _globalStartIndex - startChunk * _chunkSize, 0);
                // chunksize - 1的原因是多块情况下，一块一块的读取最值。所以取chunsize为结束索引
                int chunkEnd = Math.Min(endIndex - _globalStartIndex - startChunk * _chunkSize, _chunkSize - 1);

                for (int i = chunkStart; i <= chunkEnd; i++)
                {
                    double val = chunk.Data[i];
                    min = Math.Min(min, val);
                    max = Math.Max(max, val);
                }
            }

            return new RangeMutable(min, max);
        }
    }
}
