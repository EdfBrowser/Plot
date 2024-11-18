using System;
using System.Collections.Generic;

namespace Plot.Core
{
    public class Axis
    {
        public Axis(double min, double max, int pxSize, bool inverted, int axisIndex)
        {
            Min = min;
            Max = max;
            PxSize = pxSize;
            Inverted = inverted;
            AxisIndex = axisIndex;
        }

        public double UnitsPerPx => Span / PxSize;
        public double PxsPerUnit => PxSize / Span;
        public double Span => Max - Min;
        public double Center => (Max + Min) / 2.0;

        public Tick[] TicksMajor { get; private set; }
        public Tick[] TicksMinor { get; private set; }

        public double Min { get; set; }
        public double Max { get; set; }
        public int PxSize { get; set; }
        public bool Inverted { get; set; }

        public int AxisIndex { get; set; }

        public string Label { get; set; }

        /// <summary>
        /// Resizes the axis to the given pixel size.
        /// </summary>
        /// <param name="sizePx"></param>
        public void Resize(int sizePx)
        {
            PxSize = sizePx;
            RecalculateTicks();
        }

        /// <summary>
        /// Shift the axis by the given amount.
        /// </summary>
        /// <param name="shift"></param>
        public void Pan(double shift)
        {
            Min += shift;
            Max += shift;
            RecalculateTicks();
        }

        /// <summary>
        /// Zoom in on the center of Axis by a fraction.
        /// </summary>
        /// <param name="frac"></param>
        public void Zoom(double frac)
        {
            double newSpan = Span / frac;
            double center = Center;
            Min = center - newSpan / 2.0;
            Max = center + newSpan / 2.0;
            RecalculateTicks();
        }

        public void AxisLimits(double? min, double? max)
        {
            if (min.HasValue) Min = min.Value;
            if (max.HasValue) Max = max.Value;
            RecalculateTicks();
        }

        /// <summary>
        /// Returns the pixel position corresponding to the given unit value on the axis.
        /// </summary>
        /// <param name="unit">position (units)</param>
        /// <returns></returns>
        public int GetPixel(double unit)
        {
            double px = (unit - Min) * PxsPerUnit;
            if (Inverted) px = PxSize - px;
            return (int)px;
        }

        /// <summary>
        /// Returns the unit value corresponding to the given pixel value on the axis.
        /// </summary>
        /// <param name="pixel">position (pixels)</param>
        /// <returns></returns>
        public double GetUnit(int pixel)
        {
            if (Inverted) pixel = PxSize - pixel;
            double unit = pixel * UnitsPerPx + Min;
            return unit;
        }


        public int GetOffsetPixel()
        {
            return AxisIndex * PxSize;
        }


        private readonly double m_pixelsPerTick = 70;
        private void RecalculateTicks()
        {
            double tick_density = PxSize / m_pixelsPerTick;
            TicksMinor = GenerateTicks((int)(tick_density * 5));
            TicksMajor = GenerateTicks((int)(tick_density * 1));
        }

        private Tick[] GenerateTicks(int targetTickCount)
        {
            if (targetTickCount <= 0)
                return new Tick[0];

            List<Tick> ticks = new List<Tick>();

            // Size value of every tick
            double tickSize = RoundNumberNear(Span / targetTickCount * 1.5);
            int lastTick = 123456789;
            // 
            for (int i = 0; i < PxSize; i++)
            {
                double thisPos = i * UnitsPerPx + Min;
                // 
                int thisTick = (int)(thisPos / tickSize);

                if (lastTick != thisTick)
                {
                    lastTick = thisTick;

                    double thisPosRounded = (double)(thisTick * tickSize);
                    if (thisPosRounded > Min && thisPosRounded < Max)
                    {
                        ticks.Add(new Tick(thisPosRounded, GetPixel(thisPosRounded) + GetOffsetPixel(), Span));
                    }
                }
            }

            return ticks.ToArray();
        }


        private double RoundNumberNear(double target)
        {
            target = Math.Abs(target);
            int lastDivision = 2;
            double round = 1000000000000;
            while (round > 0.00000000001)
            {
                if (round <= target) return round;
                round /= lastDivision;
                if (lastDivision == 2) lastDivision = 5;
                else lastDivision = 2;
            }
            return 0;
        }
    }


    public class Tick
    {
        public double PosUnit { get; set; }
        public int PosPixel { get; set; }
        public double SpanUnit { get; set; }

        public Tick(double posUnit, int posPixel, double spanUnit)
        {
            PosUnit = posUnit;
            PosPixel = posPixel;
            SpanUnit = spanUnit;
        }


        public string Label
        {
            get
            {
                if (SpanUnit < .01) return string.Format("{0:0.0000}", PosUnit);
                if (SpanUnit < .1) return string.Format("{0:0.000}", PosUnit);
                if (SpanUnit < 1) return string.Format("{0:0.00}", PosUnit);
                if (SpanUnit < 10) return string.Format("{0:0.0}", PosUnit);
                return string.Format("{0:0}", PosUnit);
            }
        }
    }
}
