using UnityEngine;

namespace EniGUI
{
    public sealed class GUIInputTracker : RingedHistory<GUIInputTracker.Input>
    {
        public unsafe struct KeySet
        {
            private fixed ulong m_Bits[4];

            public bool HasAll(in KeySet other)
            {
                for (int i = 0; i < 4; i++)
                {
                    if ((m_Bits[i] & other.m_Bits[i]) != other.m_Bits[i])
                        return false;
                }

                return true;
            }
            public bool Has(GUIKeyCode key)
            {
                return (m_Bits[(int)key >> 6] & (1UL << ((int)key & 63))) != 0;
            }
            public void Set(GUIKeyCode key)
            {
                m_Bits[(int)key >> 6] |= (1UL << ((int)key & 63));
            }
            public void Remove(GUIKeyCode key)
            {
                m_Bits[(int)key >> 6] &= ~(1UL << ((int)key & 63));
            }
            public void Clear()
            {
                for(var i = 0; i < 4; i++)
                    m_Bits[i] = 0;
            }
        }

        public struct Input
        {
            public Vector2 Pointer;
            public Vector2 PointerDelta;
            public Vector2 Scroll;

            public KeySet Held;
            public KeySet Pressed;
            public KeySet Released;
        }

        private const int HISTORY_SIZE = 16;

        public GUIInputTracker() : base(HISTORY_SIZE) { }
    }
}