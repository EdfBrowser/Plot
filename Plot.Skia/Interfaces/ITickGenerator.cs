using System.Collections.Generic;

namespace Plot.Skia
{
    public interface ITickGenerator
    {
        IEnumerable<Tick> Ticks { get; }

        void Generate(Range range, Edge direction, float axisLength,
            LabelStyle tickLabelStyle);
    }
}
