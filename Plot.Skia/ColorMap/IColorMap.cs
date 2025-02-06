using System;

namespace Plot.Skia
{
    public interface IColorMap
    {
        string Name { get; }

        Color GetColor(double position);
    }

    internal static class ColorMapExtensions
    {
        internal static T Clamp<T>(this T value, T min, T max) where T : IComparable
        {
            if (value.CompareTo(min) < 0) return min;
            if (value.CompareTo(max) > 0) return max;

            return value;
        }
    }
}
