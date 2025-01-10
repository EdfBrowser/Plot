namespace Plot.Skia
{
    internal class BottomAxis : BaseXAxis
    {
        internal BottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
