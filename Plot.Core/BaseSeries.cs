using System.Drawing;

namespace Plot.Core
{
    public abstract class BaseSeries
    {
        public int XIndex { get; protected set; }
        public int YIndex { get; protected set; }

        public Color Color { get; protected set; } = Color.Red;
        public float LineWidth { get; protected set; } = 1f;
        public string Label { get; protected set; } = null;

        public BaseSeries(int xIndex, int yIndex)
        {
            XIndex = xIndex;
            YIndex = yIndex;
        }


        public abstract void Render(Bitmap bmp);
    }
}
