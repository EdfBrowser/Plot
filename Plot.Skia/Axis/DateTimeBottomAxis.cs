namespace Plot.Skia
{
    public class DateTimeBottomAxis : BaseXAxis
    {
        internal DateTimeBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new DateTimeAutoGenerator();
            LabelFormat = TickLabelFormat.DateTime;
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }
        public override TickLabelFormat LabelFormat { get; }
    }
}
