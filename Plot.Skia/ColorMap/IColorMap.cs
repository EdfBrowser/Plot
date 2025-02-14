using System;
using System.Collections.Generic;
using System.Linq;

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

        internal static IEnumerable<uint> ToColorArray(this IColorMap colormap, bool vertical)
        {
            for (int x = 0; x < 256; x++)
            {
                double val = x / 255d;
                if (vertical)
                    val = (255 - x) / 255d;

                yield return colormap.GetColor(val).UnPremulARGB;
            }
        }
    }
}
