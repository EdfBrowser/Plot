
using System;

namespace Plot.Core
{
    internal class MouseAxes
    {
        public Axis XAxStart, YAxStart;
        public int XMouseStart, YMouseStart;
        public double X1, X2, Y1, Y2;

        public MouseAxes(Axis xAxis, Axis yAxis, int mouseX, int mouseY)
        {
            XAxStart = new Axis(xAxis.Min, xAxis.Max, xAxis.PxSize, xAxis.Inverted);
            YAxStart = new Axis(yAxis.Min, yAxis.Max, yAxis.PxSize, yAxis.Inverted);
            XMouseStart = mouseX;
            YMouseStart = mouseY;
            Pan(0, 0);
        }

        public void Pan(int mouseX, int mouseY)
        {
            int dx = mouseX - XMouseStart;
            int dy = mouseY - YMouseStart;

            X1 = XAxStart.Min + dx * XAxStart.UnitsPerPx;
            X2 = XAxStart.Max + dx * XAxStart.UnitsPerPx;
            Y1 = YAxStart.Min + dy * YAxStart.UnitsPerPx;
            Y2 = YAxStart.Max + dy * YAxStart.UnitsPerPx;
        }

        public void Zoom(int mouseX, int mouseY)
        {
            double dx = (mouseX - XMouseStart) * XAxStart.UnitsPerPx;
            double dy = (mouseY - YMouseStart) * YAxStart.UnitsPerPx;

            double dXFrac = dx / (Math.Abs(dx) + XAxStart.Span);
            double dYFrac = dy / (Math.Abs(dy) + YAxStart.Span);

            double xNewSpan = XAxStart.Span / Math.Pow(10, dXFrac);
            double yNewSpan = YAxStart.Span / Math.Pow(10, dYFrac);

            double xNewCenter = XAxStart.Center;
            double yNewCenter = YAxStart.Center;

            X1 = xNewCenter - xNewSpan / 2;
            X2 = xNewCenter + xNewSpan / 2;

            Y1 = yNewCenter - yNewSpan / 2;
            Y2 = yNewCenter + yNewSpan / 2;
        }

    }
}
