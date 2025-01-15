namespace Plot.Skia
{
    public class NumericBottomAxis : BaseXAxis
    {
        internal NumericBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new NumericAutoGenerator();
            LabelFormat = TickLabelFormat.Numeric;
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
        public override TickLabelFormat LabelFormat { get; }
    }
}
