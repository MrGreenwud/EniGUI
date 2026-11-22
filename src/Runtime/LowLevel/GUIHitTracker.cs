namespace EniGUI
{
    public sealed class GUIHitTracker : RingedHistory<GUIHitTracker.Hit>
    {
        public struct Hit
        {
            public uint Hover;
            public uint Active;

            public Hit(uint hover, uint active)
            {
                Hover = hover;
                Active = active;
            }
        }

        private const int HISTORY_SIZE = 4;

        public GUIHitTracker() : base(HISTORY_SIZE) { }
    }
}