namespace Plot.Skia
{
    public class NumericTopAxis : BaseXAxis
    {
        internal NumericTopAxis()
        {
            Direction = Edge.Top;
            TickGenerator = new AutoNumericGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
