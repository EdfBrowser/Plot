using System;
using System.Collections.Generic;

namespace Plot.Chart
{
    public class Axis
    {
        private double m_min;
        private double m_max;
        private int m_pxSize;
        private bool m_inverted;

        public Axis(double min, double max, int pxSize, bool inverted)
        {
            m_min = min;
            m_max = max;
            m_pxSize = pxSize;
            m_inverted = inverted;
        }

        public double UnitsPerPx => Span / m_pxSize;
        public double PxsPerUnit => m_pxSize / Span;
        public double Span => m_max - m_min;
        public double Center => (m_max + m_min) / 2.0;

        public Tick[] TicksMajor { get; private set; }
        public Tick[] TicksMinor { get; private set; }


        /// <summary>
        /// Resizes the axis to the given pixel size.
        /// </summary>
        /// <param name="sizePx"></param>
        public void Resize(int sizePx)
        {
            m_pxSize = sizePx;
            RecalculateTicks();
        }

        /// <summary>
        /// Shift the axis by the given amount.
        /// </summary>
        /// <param name="shift"></param>
        public void Pan(double shift)
        {
            m_min += shift;
            m_max += shift;
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
            m_min = center - newSpan / 2.0;
            m_max = center + newSpan / 2.0;
            RecalculateTicks();
        }

        /// <summary>
        /// Returns the pixel position corresponding to the given unit value on the axis.
        /// </summary>
        /// <param name="unit">position (units)</param>
        /// <returns></returns>
        public int GetPixel(double unit)
        {
            double px = (unit - m_min) * PxsPerUnit;
            if (m_inverted) px = m_pxSize - px;
            return (int)px;
        }

        /// <summary>
        /// Returns the unit value corresponding to the given pixel value on the axis.
        /// </summary>
        /// <param name="pixel">position (pixels)</param>
        /// <returns></returns>
        public double GetUnit(int pixel)
        {
            if (m_inverted) pixel = m_pxSize - pixel;
            double unit = pixel * UnitsPerPx + m_min;
            return unit;
        }


        private double m_pixelsPerTick = 70;
        private void RecalculateTicks()
        {
            double tick_density = m_pxSize / m_pixelsPerTick;
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
            for (int i = 0; i < m_pxSize; i++)
            {
                double thisPos = i * UnitsPerPx + m_min;
                // 
                int thisTick = (int)(thisPos / tickSize);

                if (lastTick != thisTick)
                {
                    lastTick = thisTick;

                    double thisPosRounded = (double)(thisTick * tickSize);
                    if (thisPosRounded > m_min && thisPosRounded < m_max)
                    {
                        ticks.Add(new Tick(thisPosRounded, GetPixel(thisPosRounded), Span));
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
