namespace Plot.Core
{
    public struct InputState
    {
        public float m_x;
        public float m_y;
        public bool m_leftPressed;
        public bool m_rightPressed;
        public bool m_middlePressed;
        public bool m_shiftPressed;
        public bool m_controlPressed;
        public bool m_altPressed;
        public bool m_scrollUp;
        public bool m_scrollDown;

        public InputState((int, int) key, (bool, bool, bool) btn, (bool, bool, bool) keys, int delta)
        {
            (m_x, m_y) = key;
            (m_leftPressed, m_rightPressed, m_middlePressed) = btn;
            (m_shiftPressed, m_controlPressed, m_altPressed) = keys;
            (m_scrollUp, m_scrollDown) = (delta > 0, delta < 0);
        }

        public bool MousePressed => m_leftPressed || m_rightPressed || m_middlePressed;
        public static InputState Default => new InputState();
    }
}
