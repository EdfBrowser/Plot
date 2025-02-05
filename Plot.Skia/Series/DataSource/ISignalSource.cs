using System.Collections.Generic;

namespace Plot.Skia
{
    public interface ISignalSource
    {
        double SampleInterval { get; set; }
        int MaximumIndex { get; set; }
        int MinimumIndex { get; set; }

        int GetIndex(double x);
        double GetX(int index);
        double GetY(int index);
        IEnumerable<double> GetYs();
        IEnumerable<double> GetYs(int startIndex, int endIndex);

        RangeMutable GetXLimit();
        RangeMutable GetYLimit();
        RangeMutable GetYLimit(int startIndex, int endIndex);
    }
}
