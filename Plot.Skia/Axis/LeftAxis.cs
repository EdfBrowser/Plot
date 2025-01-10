namespace Plot.Skia
{
    internal class LeftAxis : BaseYAxis
    {
        public LeftAxis()
        {
            Direction = Edge.Left;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
