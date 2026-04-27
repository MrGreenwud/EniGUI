using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace EniGUI.LowLevel
{
    internal sealed class IDStack
    {
        private readonly Stack<uint> m_PreviousID = new();

        public uint Current { get; private set; }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Push(uint id)
        {
            uint hash = Current;

            hash ^= id;
            hash *= 16777619u;

            m_PreviousID.Push(Current);
            Current = hash;
            return Current;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pop()
        {
            Current = m_PreviousID.Pop();
        }
        
        public void Reset()
        {
            m_PreviousID.Clear();
            Current = 0;
        }
    }
}