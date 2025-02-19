using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class SeriesManager : IDisposable
    {
        private readonly Figure m_figure;

        internal SeriesManager(Figure figure)
        {
            m_figure = figure;
            Series = new List<ISeries>();
        }

        public IList<ISeries> Series { get; }

        public SignalSeries AddSignalSeries(IXAxis x, IYAxis y, ISignalSource signalSource)
        {
            SignalSeries sig = new SignalSeries(x, y, signalSource);
            Series.Add(sig);

            return sig;
        }

        public SignalSeries AddSignalSeries(IXAxis x, IYAxis y, double sampleInterval = 1.0)
        {
            ISignalSource source = new SignalSourceDouble(sampleInterval);
            return AddSignalSeries(x, y, source);
        }

        public HeatMapSeries AddHeatMapSeries(IXAxis x, IYAxis y,
            double[,] intensity)
        {
            HeatMapSeries hms = new HeatMapSeries(x, y, intensity);
            Series.Add(hms);

            return hms;
        }

        public void Dispose()
        {
            foreach (ISeries s in Series)
                s.Dispose();
        }
    }
}
