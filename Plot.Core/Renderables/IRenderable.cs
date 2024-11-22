using System.Drawing;

namespace Plot.Core.Renderables
{
    public interface IRenderable
    {
        bool Visible { get; set; }
        void Render(Bitmap bmp, PlotDimensions dims, bool lowQuality);
    }
}
