using Plot.Core.Renderables.Axes;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;

namespace Plot.Core.Series
{
    public class SeriesManager
    {
        private readonly List<IPlotSeries> m_seriesList;

        public SeriesManager()
        {
            m_seriesList = new List<IPlotSeries>();
        }

        public IEnumerable<StreamerPlotSeries> GetStreamerPlotSeries() => m_seriesList.OfType<StreamerPlotSeries>(); 
        public void Clear() => m_seriesList.Clear();

        public void GetLimitFromSeries()
        {
            double xmin = double.MaxValue, xmax = double.MinValue;
            double ymin = double.MaxValue, ymax = double.MinValue;
            var limits = m_seriesList
                .Where(x => !x.XAxis.Dims.HasBeenSet || !x.YAxis.Dims.HasBeenSet)
                .Select(X => X.GetAxisLimits());

            foreach (var limit in limits)
            {
                xmin = xmin == double.MaxValue ? limit.m_xMin : Math.Min(xmin, limit.m_xMin);
                xmax = xmax == double.MinValue ? limit.m_xMax : Math.Max(xmax, limit.m_xMax);
                ymin = ymin == double.MaxValue ? limit.m_yMin : Math.Min(ymin, limit.m_yMin);
                ymax = ymax == double.MinValue ? limit.m_yMax : Math.Max(ymax, limit.m_yMax);
            }

            if (xmin == double.MaxValue || xmin == double.MinValue || ymin == double.MaxValue || ymin == double.MinValue)
                return;

            foreach (var series in m_seriesList)
            {
                AxisLimits limit = new AxisLimits(xmin, xmax, ymin, ymax);
                series.XAxis.SetAxisLimits(series.YAxis, limit);
            }
        }

        public void RenderSeries(Bitmap bmp, bool lowQuality, float scale)
        {
            foreach (var series in m_seriesList)
            {
                series.ValidateData();

                try
                {
                    series.Plot(bmp, lowQuality, scale);
                }
                catch (OverflowException)
                {
                    Debug.WriteLine($"OverflowException plotting: {series}");
                }
            }
        }

        public void AddSeries(IPlotSeries series) => m_seriesList.Add(series);

        public StreamerPlotSeries AddStreamerPlotSeries(Axis xAxis, Axis yAxis, int sampleRate)
        {
            StreamerPlotSeries series = new StreamerPlotSeries(xAxis, yAxis, sampleRate);
            AddSeries(series);
            return series;
        }

        public SignalPlotSeries AddSignalPlotSeries(Axis xAxis, Axis yAxis)
        {
            SignalPlotSeries series = new SignalPlotSeries(xAxis, yAxis);
            AddSeries(series);
            return series;
        }
    }
}
