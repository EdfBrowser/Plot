namespace Plot.Skia
{
    public class DateTimeBottomAxis : BaseXAxis
    {
        public DateTimeBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new DateTimeAutoGenerator();
        }

        public override Edge Direction { get; }

        public override ITickGenerator TickGenerator { get; }
    }
}
