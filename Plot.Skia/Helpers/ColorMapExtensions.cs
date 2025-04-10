using System.Collections.Generic;

namespace Plot.Skia
{
    internal static class ColorMapExtensions
    {
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
