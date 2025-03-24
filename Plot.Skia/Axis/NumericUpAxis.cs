namespace Plot.Skia
{
    public class NumericTopAxis : BaseXAxis
    {
        internal NumericTopAxis()
        {
            Direction = Edge.Top;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
