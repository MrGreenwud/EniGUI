using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EniGUI.LowLevel
{
    internal sealed class BatchSet : IDisposable
    {
        private readonly uint m_MaxVertexCountPerBatch;
        private readonly Material m_Material;

        private BatchData[] m_Batches;
        private int m_FirstUsed;
        private int m_LastUsed;

        private BatchData m_HotBatch;

        public byte ActiveTexture { get; private set; }
        public ReadOnlySpan<BatchData> Batches => m_Batches.AsSpan(m_FirstUsed, m_LastUsed - m_FirstUsed);

        public BatchSet(Material material, uint capacity, uint maxVertexCountPerBatch)
        {
            m_Material = material;
            m_MaxVertexCountPerBatch = maxVertexCountPerBatch;
            m_Batches = new BatchData[capacity];
            FillBatches();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchData GetBatch()
        {
            if (m_HotBatch != null && !m_HotBatch.IsFull)
                return m_HotBatch;

            if (m_LastUsed >= m_Batches.Length)
            {
                Array.Resize(ref m_Batches, m_Batches.Length * 2);
                FillBatches();
            }

            var batch = m_Batches[m_LastUsed];
            batch.Clear(m_Material);
            m_LastUsed++;

            m_HotBatch = batch;

            return m_HotBatch;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Break()
        {
            m_FirstUsed = m_LastUsed;
            m_HotBatch = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear()
        {
            m_LastUsed = 0;
            Break();
        }

        public void Dispose()
        {
            for(int i = 0; i < m_Batches.Length; i++)
                m_Batches[i]?.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillBatches()
        {
            for (int i = m_LastUsed; i < m_Batches.Length; i++)
                m_Batches[i] = new((int)m_MaxVertexCountPerBatch);
        }
    }
}