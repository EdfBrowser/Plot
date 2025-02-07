using System.Linq;

namespace Plot.Skia
{
    public abstract class CMP : IColorMap
    {
        private readonly Color[] m_colors;

        protected CMP(string name, uint[] rgbs)
        {
            Name = name;

            // 0xFF << 24 = 255
            // “|” 将255与rgb组成argb
            m_colors = rgbs.Select(rgb => unchecked((uint)(0xFF << 24) | (uint)rgb))
                .Select(Color.FromARGB)
                .ToArray();
        }

        public string Name { get; }

        public Color GetColor(double position)
        {
            position = position.Clamp(0, 1);
            // 255 * (0-1)之间的数
            int length = m_colors.Length - 1;
            int index = (int)(length * position);

            return m_colors[index];
        }
    }
}
