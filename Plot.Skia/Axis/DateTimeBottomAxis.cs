using System;

namespace Plot.Skia
{
    public class DateTimeBottomAxis : BaseXAxis
    {
        internal DateTimeBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new DateTimeAutoGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }

        public void SetOriginDateTime(DateTime dateTime)
            => (TickGenerator as DateTimeAutoGenerator).OriginDateTime = dateTime;

        public void SetDateTimeFormat(string format)
            => (TickGenerator as DateTimeAutoGenerator).DateTimeFormat = format;

    }
}
