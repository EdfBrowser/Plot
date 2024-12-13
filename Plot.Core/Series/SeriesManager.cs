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
        public IEnumerable<VLinePlotSeries> GetVLinePlotSeries() => m_seriesList.OfType<VLinePlotSeries>();

        public void Clear() => m_seriesList.Clear();

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


    }
}
