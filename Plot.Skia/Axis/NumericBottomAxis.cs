namespace Plot.Skia
{
    public class NumericBottomAxis : BaseXAxis
    {
        public NumericBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new NumericAutoGenerator();
            LabelFormat = TickLabelFormat.Numeric;
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
        public override TickLabelFormat LabelFormat { get; set; }
    }
}
