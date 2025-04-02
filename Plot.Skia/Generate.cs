using System;

namespace Plot.Skia
{
    public static class Generate
    {
        private static readonly Random _random = new Random();

        public static double[] Sin(int count = 51, double mult = 1, double offset = 0, double oscillations = 1, double phase = 0)
        {
            double sinScale = 2 * Math.PI * oscillations / (count - 1);
            double[] ys = new double[count];
            for (int i = 0; i < ys.Length; i++)
                ys[i] = Math.Sin(i * sinScale + phase * Math.PI * 2) * mult + offset;
            return ys;
        }

        public static Color RandomColor()
        {
            byte r = RandomByte();
            byte g = RandomByte();
            byte b = RandomByte();
            return new Color(r, g, b);
        }

        private static byte RandomByte()
        {
            return (byte)_random.Next(256);
        }
    }
}
