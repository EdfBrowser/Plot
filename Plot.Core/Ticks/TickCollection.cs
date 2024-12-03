using System;

namespace Plot.Core.Ticks
{
    public class TickCollection
    {
        public double[] Major { get; }
        public double[] Minor { get; }
        public string[] Labels { get; }

        public TickCollection(double[] major, double[] minor, string[] labels)
        {
            if (major.Length != labels.Length)
                throw new InvalidOperationException($"{nameof(major)} must have the same length as {nameof(labels)}");

            Major = major ?? Array.Empty<double>();
            Minor = minor ?? Array.Empty<double>();
            Labels = labels ?? Array.Empty<string>();
        }

        public static TickCollection Empty => new TickCollection(
            Array.Empty<double>(),
            Array.Empty<double>(),
            Array.Empty<string>());
    }
}
