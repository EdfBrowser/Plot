namespace Plot.Skia
{
    public class NumericBottomAxis : BaseXAxis
    {
        internal NumericBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new AutoNumericGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
