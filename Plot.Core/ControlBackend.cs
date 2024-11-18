using System;
using System.Collections.Generic;
using System.Drawing;

namespace Plot.Core
{
    public class ControlBackend
    {
        private Bitmap m_bmp;
        private Queue<Bitmap> m_oldBitmaps = new Queue<Bitmap>();

        private long m_bitmapRenderCount = 0;

        public ControlBackend(int width, int height)
        {
            Plt = new Figure();
            Resize(width, height);
        }

        public void Resize(int width, int height)
        {
            if (width < 1 || height < 1) return;

            if (m_bmp?.Width == width && m_bmp?.Height == height) return;

            Plt.Resize(width, height);

            if (m_bmp != null)
                m_oldBitmaps.Enqueue(m_bmp);

            m_bmp = new Bitmap(width, height);


            m_bitmapRenderCount = 0;

            Render();
        }

        public Bitmap GetLatestBitmap()
        {
            while (m_oldBitmaps.Count > 3)
            {
                m_oldBitmaps.Dequeue()?.Dispose();
            }

            return m_bmp;
        }

        public void Render()
        {
            if (m_bmp == null) return;

            Plt.Render(m_bmp);

            m_bitmapRenderCount += 1;

            if (m_bitmapRenderCount == 1)
            {
                OnBitmapChanged?.Invoke(null, null);
            }
            else
            {
                OnBitmapUpdated?.Invoke(null, null);
            }
        }

        public event EventHandler OnBitmapChanged;
        public event EventHandler OnBitmapUpdated;

        public Figure Plt { get; private set; }
    }
}
