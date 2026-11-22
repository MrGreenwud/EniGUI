using System;
using System.Runtime.CompilerServices;
using UnityEngine.Assertions;

namespace EniGUI.LowLevel
{
    internal sealed class IDStack
    {
        private uint[] m_Stack = new uint[32];
        private int m_Depth;

        public uint Current
        {
            get
            {
                if (m_Depth == 0)
                    return 0;

                var hash = m_Stack[m_Depth];
                hash = (hash ^ (hash >> 12)) & 0xFFFFFu;
                return hash == 0 ? 1u : hash; ;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public uint Push(uint id)
        {
            if (m_Depth + 1 == m_Stack.Length)
                Array.Resize(ref m_Stack, m_Stack.Length * 2);

            uint hash = (m_Stack[m_Depth] ^ id) * 0x9E3779B1u;
            hash ^= hash >> 15;
            hash *= 0x85EBCA6Bu;
            hash ^= hash >> 13;

            m_Stack[++m_Depth] = hash;

            return Current;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Pop()
        {
            Assert.IsTrue(m_Depth > 0);
            m_Depth--;
        }
        
        public void Reset()
        {
            m_Depth = 0;
        }
    }
}