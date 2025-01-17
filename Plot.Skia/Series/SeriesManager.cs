using System;
using System.Collections.Generic;

namespace Plot.Skia
{
    public class SeriesManager : IDisposable
    {
        private readonly Figure m_figure;

        public SeriesManager(Figure figure)
        {
            m_figure = figure;
            Series = new List<ISeries>();
        }

        internal IList<ISeries> Series { get; }


        public SignalSeries AddSignalSeries(IXAxis x, IYAxis y)
        {
            SignalSeries sig = new SignalSeries(x, y);
            Series.Add(sig);

            return sig;
        }

        public void Dispose()
        {
            foreach (ISeries s in Series)
                s.Dispose();
        }
    }
}
