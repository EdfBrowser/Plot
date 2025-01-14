namespace Plot.Skia
{
    public class NumericLeftAxis : BaseYAxis
    {
        public NumericLeftAxis()
        {
            Direction = Edge.Left;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
