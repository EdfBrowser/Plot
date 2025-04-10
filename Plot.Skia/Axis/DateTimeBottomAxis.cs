using System;

namespace Plot.Skia
{
    public class DateTimeBottomAxis : BaseXAxis
    {
        internal DateTimeBottomAxis()
        {
            Direction = Edge.Bottom;
            TickGenerator = new AutoDateTimeGenerator();
        }

        public override Edge Direction { get; }
        public override ITickGenerator TickGenerator { get; }

        public void SetOriginDateTime(DateTime dateTime)
            => (TickGenerator as AutoDateTimeGenerator).OriginDateTime = dateTime;

        public void SetDateTimeFormat(string format)
            => (TickGenerator as AutoDateTimeGenerator).DateTimeFormat = format;

    }
}
