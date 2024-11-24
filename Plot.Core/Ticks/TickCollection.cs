using System;

namespace Plot.Core.Ticks
{
    public class TickCollection
    {
        public float[] Major { get; }
        public float[] Minor { get; }
        public string[] Labels { get; }

        public TickCollection(float[] major, float[] minor, string[] labels)
        {
            if (major.Length != labels.Length)
                throw new InvalidOperationException($"{nameof(major)} must have the same length as {nameof(labels)}");

            Major = major ?? Array.Empty<float>();
            Minor = minor ?? Array.Empty<float>();
            Labels = labels ?? Array.Empty<string>();
        }

        public static TickCollection Empty =>new TickCollection(
            Array.Empty<float>(),
            Array.Empty<float>(),
            Array.Empty<string>());
    }
}
