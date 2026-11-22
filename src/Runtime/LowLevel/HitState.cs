namespace EniGUI
{
    public readonly struct HitState
    {
        private readonly GUIHitTracker.Hit m_Current;
        private readonly GUIHitTracker.Hit m_Previous;

        public HitState(GUIHitTracker.Hit current, GUIHitTracker.Hit previous)
        {
            m_Current = current;
            m_Previous = previous;
        }

        public uint id => m_Current.Hover;

        public bool Hover(uint id) => id != 0 && m_Current.Hover == id;
        public bool Active(uint id) => id != 0 && m_Current.Active == id;
        public bool Entered(uint id) => Hover(id) && m_Previous.Hover != id;
        public bool Exited(uint id) => !Hover(id) && m_Previous.Hover == id;
    }
}