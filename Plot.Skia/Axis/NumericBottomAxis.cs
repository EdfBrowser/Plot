namespace Plot.Skia
{
    public class NumericBottomAxis : BaseXAxis
    {
        internal NumericBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new NumericAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
