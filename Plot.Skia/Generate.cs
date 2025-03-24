using System;

namespace Plot.Skia
{
    public static class Generate
    {
        public static double[] Sin(int count = 51, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
        {
            double sinScale = 2 * Math.PI * oscillations / (count - 1);
            double[] ys = new double[count];
            for (int i = 0; i < ys.Length; i++)
                ys[i] = Math.Sin(i * sinScale + phase * Math.PI * 2) * mult + offset;
            return ys;
        }
    }
}
