using System;

namespace Plot.Core.Ticks
{
    // TODO: 是否改成struct
    public class Tick
    {
        public float m_position;
        public string m_label;
        public bool m_isMajor;
        public bool m_isDateTime;
        public DateTime DateTime => DateTime.FromOADate(m_position);

        public Tick(float position, string label, bool isMajor, bool isDateTime)
        {
            m_position = position;
            m_label = label;
            m_isMajor = isMajor;
            m_isDateTime = isDateTime;
        }

        public override string ToString()
        {
            string tickType = m_isMajor ? "Major Tick" : "Minor Tick";
            string tickLabel = string.IsNullOrEmpty(m_label) ? "(unlabeled)" : $"labeled '{m_label}'";
            string tickPosition = m_isDateTime ? DateTime.ToString() : m_position.ToString();
            return $"{tickType} at {tickPosition} {tickLabel}";
        }
    }
}
