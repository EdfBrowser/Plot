namespace Plot.Skia
{
    public class NumericLeftAxis : BaseYAxis
    {
        internal NumericLeftAxis()
        {
            Direction = Edge.Left;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
