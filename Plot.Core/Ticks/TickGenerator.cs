using System;
using System.Collections.Generic;

namespace Plot.Core.Ticks
{
    public class TickGenerator
    {
        internal Tick[] TicksMajor { get; private set; }
        internal Tick[] TicksMinor { get; private set; }

        internal bool IsVertical { get; set; } = true;

        [Obsolete]
        private readonly float m_pixelsPerTick = 70;

        internal float TickSpacingPx { get; set; } = 100;


        internal void GetTicks(PlotDimensions dims)
        {
            float span, pxSize, min, max;
            if (IsVertical)
            {
                span = dims.m_ySpan;
                pxSize = dims.m_plotHeight;
                min = dims.m_yMin;
                max = dims.m_yMax;
            }
            else
            {
                span = dims.m_xSpan;
                pxSize = dims.m_plotWidth;
                min = dims.m_xMin;
                max = dims.m_xMax;
            }

            TicksMajor = Calculate(pxSize, span, min, max, true);
            TicksMinor = Calculate(pxSize, span, min, max, false);
        }

        private Tick[] Calculate(float pxSize, float span, float min, float max, bool major = true)
        {
            List<Tick> ticks = new List<Tick>();

            //if (pxSize < TickSpacingPx / 2) return Array.Empty<Tick>();

            float minimumTickCount = pxSize / TickSpacingPx;
            if (major == false) minimumTickCount *= 5;

            float tickSpacing = (float)GetIdealTickSpacing(span, (int)minimumTickCount);
            int tickCount = (int)(span / tickSpacing) + 1;

            // To get an integer scale
            // For example, spacing = 1, min = 5.7
            // min % spacing = 0.7 , Therefore, the first scale will move from 5.7 to 0.7,
            // becoming an integer scale
            float tickOffsetFromMin = min % tickSpacing;

            for (int i = 0; i < tickCount; i++)
            {
                float tickDelta = i * tickSpacing - tickOffsetFromMin;
                float posUnit = min + tickDelta;

                if (posUnit >= min && posUnit <= max)
                {
                    ticks.Add(new Tick(posUnit, posUnit, tickSpacing));
                }
            }

            return ticks.ToArray();
        }

        private double GetIdealTickSpacing(float span, int tickCountTarget)
        {
            double tickSpacing = 0;
            for (int powerOfTen = 10; powerOfTen > -10; powerOfTen--)
            {
                tickSpacing = Math.Pow(10, powerOfTen);

                if (tickSpacing > span) continue;

                double tickCount = span / tickSpacing;
                if (tickCount >= tickCountTarget)
                {
                    // a good tick density
                    if (tickCount >= tickCountTarget * 5) return tickSpacing * 5;
                    if (tickCount >= tickCountTarget * 2) return tickSpacing * 2;
                    return tickSpacing;
                }
            }

            return 0;
        }


        [Obsolete("use GetTicks instead")]
        public void RecalculateTicks(PlotDimensions dims)
        {
            float tick_density = 0;
            if (IsVertical)
            {
                tick_density = dims.m_plotHeight / m_pixelsPerTick;
            }
            else
            {
                tick_density = dims.m_plotWidth / m_pixelsPerTick;
            }
            TicksMinor = AutoCalculate(dims, (int)(tick_density * 5));
            TicksMajor = AutoCalculate(dims, (int)(tick_density * 1));
        }

        [Obsolete]
        private Tick[] AutoCalculate(PlotDimensions dims, int targetTickCount)
        {
            float span, pxSize, unitsPerPx, min, max;
            if (IsVertical)
            {
                span = dims.m_ySpan;
                pxSize = dims.m_plotHeight;
                unitsPerPx = dims.m_unitsPerPxY;
                min = dims.m_yMin;
                max = dims.m_yMax;
            }
            else
            {
                span = dims.m_xSpan;
                pxSize = dims.m_plotWidth;
                unitsPerPx = dims.m_unitsPerPxX;
                min = dims.m_xMin;
                max = dims.m_xMax;
            }

            return GenerateTicks(span, pxSize, unitsPerPx, min, max, targetTickCount);
        }
        [Obsolete]
        private Tick[] GenerateTicks(float span, float pxSize, float unitsPerPx,
            float min, float max, float targetTickCount)
        {
            if (targetTickCount <= 0)
                return Array.Empty<Tick>();

            List<Tick> ticks = new List<Tick>();

            // Size value of every tick
            double tickSize = RoundNumberNear(span / targetTickCount * 1.5);
            int lastTick = 123456789;
            // 
            for (int i = 0; i < pxSize; i++)
            {
                float thisPos = i * unitsPerPx + min;
                // 
                int thisTick = (int)(thisPos / tickSize);

                if (lastTick != thisTick)
                {
                    lastTick = thisTick;

                    float thisPosRounded = (float)(thisTick * tickSize);
                    if (thisPosRounded > min && thisPosRounded < max)
                    {
                        ticks.Add(new Tick(thisPosRounded, thisPosRounded, span));
                    }
                }
            }

            return ticks.ToArray();
        }

        [Obsolete]
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
}
