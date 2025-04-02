using System;
using System.Collections.Generic;
using System.Linq;

namespace Plot.Skia
{
    internal class DateTimeAutoGenerator : BaseTickGenerator, ITickGenerator
    {
        public DateTimeAutoGenerator()
        {
            OriginDateTime = new DateTime(1899, 12, 31, 0, 0, 0);
            DateTimeFormat = "yyyy/MM/dd\nHH:mm:ss";
        }

        internal DateTime OriginDateTime { get; set; }
        internal string DateTimeFormat { get; set; }

        public IEnumerable<Tick> Ticks { get; private set; }

        public void Generate(Range range, Edge direction, float axisLength, LabelStyle tickLabelStyle)
        {
            Ticks = GenerateTicks(range, direction, axisLength, 12f, tickLabelStyle);
        }

        private IEnumerable<Tick> GenerateTicks(Range range, Edge direction,
            float axisLength, float labelLength, LabelStyle tickLabelStyle)
        {
            float labelWidth = Math.Max(0, labelLength);

            IEnumerable<double> tickPositions
                = GenerateDateTimeTickPositions(range, axisLength, labelWidth);

            //if (axisLength <= tickLabelStyle.Measure(DateTimeFormat).Width)
            //{
            //    DateTimeFormat = "hh:mm:ss";
            //}
            //else
            //{
            //    DateTimeFormat = "yyyy/MM/dd\nHH:mm:ss";
            //}

            IEnumerable<string> tickLabels = tickPositions
               .Select(GetPositionLabel);

            (string largestText, float actualMaxLength) = direction.Vertical()
                ? MeasureString(tickLabels, x => tickLabelStyle.Measure(x, force: true).Height)
                : MeasureString(tickLabels, x => tickLabelStyle.Measure(x, force: true).Width);

            return actualMaxLength > labelLength
                ? GenerateTicks(range, direction, axisLength, actualMaxLength, tickLabelStyle)
                : GenerateFinalTicks(tickPositions, tickLabels, range);
        }

        protected override string GetPositionLabel(double value)
            => OriginDateTime.AddSeconds(value).ToString(DateTimeFormat);
    }
}
