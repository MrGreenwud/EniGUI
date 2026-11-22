using System;
using UnityEngine;
using UnityEngine.Assertions;

namespace EniGUI.Internal
{
    public sealed class BatchSet
    {
        private readonly int m_MaxElementPerBatch;

        private Batch[] m_Batches = new Batch[32];

        public ReadOnlySpan<Batch> Batches => m_Batches.AsSpan(0, Count);
        public int Count { get; private set; }

        public BatchSet() : this(5000) { }

        public BatchSet(int maxElementPerBatch)
        {
            m_MaxElementPerBatch = maxElementPerBatch;
        }

        public void Split(int count, Material material)
        {
            Assert.IsNotNull(material);
            Assert.IsTrue(count > 0);

            var batchCount = (count + m_MaxElementPerBatch - 1) / m_MaxElementPerBatch;

            if (Count + batchCount > m_Batches.Length)
                Array.Resize(ref m_Batches, Math.Max(m_Batches.Length * 2, (Count + batchCount) * 2));

            var offset = 0;

            for (var i = 0; i < batchCount; i++)
            {
                var batchSize = Math.Min(m_MaxElementPerBatch, count);
                m_Batches[Count++] = new(material, offset, batchSize);
                offset += batchSize;
                count -= batchSize;
            }
        }

        public void Clear()
        {
            Count = 0;
        }
    }
}