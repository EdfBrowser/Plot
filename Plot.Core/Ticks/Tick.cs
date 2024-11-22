namespace Plot.Core.Ticks
{
    public struct Tick
    {
        public float m_posUnit;
        public float m_posPixel;
        public float m_spanUnit;

        public Tick(float posUnit, float posPixel, float spanUnit)
        {
            m_posUnit = posUnit;
            m_posPixel = posPixel;
            m_spanUnit = spanUnit;
        }

        public string Label
        {
            get
            {
                if (m_spanUnit < .001) return string.Format("{0:0.0000}", m_posUnit);
                if (m_spanUnit < .01) return string.Format("{0:0.000}", m_posUnit);
                if (m_spanUnit < .1) return string.Format("{0:0.00}", m_posUnit);
                if (m_spanUnit < 1) return string.Format("{0:0.0}", m_posUnit);
                return string.Format("{0:0}", m_posUnit);
            }
        }
    }
}
