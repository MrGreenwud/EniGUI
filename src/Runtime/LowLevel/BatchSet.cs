using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EniGUI.LowLevel
{
    internal sealed class BatchSet : IDisposable
    {
        private readonly uint m_MaxVertexCountPerBatch;
        private readonly Dictionary<Material, BatchData> m_MappedBatches = new();
        
        private BatchData[] m_Batches;
        private int m_FirstUsed;
        private int m_LastUsed;

        private Material m_HotMaterial;
        private BatchData m_HotBatch;

        public ReadOnlySpan<BatchData> Batches => m_Batches.AsSpan(m_FirstUsed, m_LastUsed - m_FirstUsed);

        public BatchSet(uint capacity, uint maxVertexCountPerBatch)
        {
            m_MaxVertexCountPerBatch = maxVertexCountPerBatch;
            m_Batches = new BatchData[capacity];
            m_MappedBatches = new((int)capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchData GetBatch(Material material)
        {
            if (m_HotMaterial == material && !m_HotBatch.IsFull)
                return m_HotBatch;

            if (!m_MappedBatches.TryGetValue(material, out BatchData batch) || batch.IsFull)
            {
                if (m_LastUsed >= m_Batches.Length)
                    Array.Resize(ref m_Batches, m_Batches.Length * 2);

                batch = m_Batches[m_LastUsed] ??= new BatchData((int)m_MaxVertexCountPerBatch);
                batch.Clear(material);
                m_LastUsed++;

                if (!m_MappedBatches.ContainsKey(material))
                    m_MappedBatches.Add(material, batch);
                else
                    m_MappedBatches[material] = batch;
            }

            m_HotMaterial = material;
            m_HotBatch = batch;

            return m_HotBatch;
        }

        public void Break()
        {
            m_FirstUsed = m_LastUsed;
            m_MappedBatches.Clear();
            m_HotMaterial = null;
            m_HotBatch = null;
        }
        public void Clear()
        {
            m_LastUsed = 0;
            Break();
        }

        public void Dispose()
        {
            m_MappedBatches.Clear();

            for(int i = 0; i < m_Batches.Length; i++)
                m_Batches[i]?.Dispose();
        }
    }
}