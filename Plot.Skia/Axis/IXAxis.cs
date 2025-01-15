namespace Plot.Skia
{
    public interface IXAxis : IAxis
    {
        double Width { get; }
        AxisScrollMode ScrollMode { get; set; }
        double ScrollPosition { get; set; }
        bool Animate { get; set; }
        TickLabelFormat LabelFormat { get; }

    }
}
