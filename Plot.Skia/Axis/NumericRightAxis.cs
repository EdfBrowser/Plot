namespace Plot.Skia
{
    public class NumericRightAxis : BaseYAxis
    {
        internal NumericRightAxis()
        {
            Direction = Edge.Right;
            TickGenerator = new AutoNumericGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
    }
}
