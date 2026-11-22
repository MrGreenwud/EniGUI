using UnityEngine.Assertions;

namespace EniGUI
{
    public class RingedHistory<T> where T : struct
    {
        private readonly T[] m_Items;
        private readonly int m_Mask;

        private int m_Head = 0;

        public ref readonly T Current => ref m_Items[m_Head & m_Mask];
        
        public RingedHistory(int size = 16)
        {
            Assert.IsTrue(size > 0 && (size & (size - 1)) == 0, 
                "size must be a power of two");

            m_Items = new T[size];
            m_Mask = size - 1;
        }

        public ref readonly T Ago(int frames) => ref m_Items[(m_Head - frames) & m_Mask];
        public void Push(T item) => m_Items[++m_Head & m_Mask] = item;
    }
}