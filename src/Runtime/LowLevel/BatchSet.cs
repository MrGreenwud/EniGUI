using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EniGUI.Renderer.LowLevel
{
    internal sealed class BatchSet
    {
        private const int MAX_VERTEX_PER_BATCH = 10000;

        private readonly Dictionary<Material, BatchData> m_MappedBatches;

        private BatchData[] m_Batches;
        private int m_LastUsed;

        private Material m_HotMaterial;
        private BatchData m_HotBatch;

        public ReadOnlySpan<BatchData> Batches => m_Batches.AsSpan(0, m_LastUsed);

        public BatchSet(int capacity)
        {
            m_Batches = new BatchData[capacity];
            m_MappedBatches = new(capacity);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public BatchData Get(Material material)
        {
            if (m_HotMaterial == material)
                return m_HotBatch;

            if (!m_MappedBatches.TryGetValue(material, out var batch))
            {
                if (m_LastUsed >= m_Batches.Length)
                    Array.Resize(ref m_Batches, m_Batches.Length * 2);

                batch = m_Batches[m_LastUsed] ??= new BatchData(MAX_VERTEX_PER_BATCH);
                batch.Clear(material);
                m_LastUsed++;

                m_MappedBatches.Add(material, batch);
            }

            m_HotMaterial = material;
            m_HotBatch = batch;

            return m_HotBatch;
        }

        public void Clear()
        {
            m_HotMaterial = null;
            m_HotBatch = null;

            m_MappedBatches.Clear();
            m_LastUsed = 0;
        }
    }
}