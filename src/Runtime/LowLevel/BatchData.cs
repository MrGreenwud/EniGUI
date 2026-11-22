using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace EniGUI.LowLevel
{
    internal sealed class BatchData : IDisposable
    {
        private static readonly float2[] m_ZeroUVs = new float2[2];

        public readonly ComputeBuffer Buffer;

        private NativeArray<UIElement> m_Elements;

        public int Count { get; private set; }
        public Material Material { get; private set; }
        public bool IsFull { get; private set; }

        public BatchData(int maxElementCount = 2500)
        {
            m_Elements = new(maxElementCount, Allocator.Persistent);
            Buffer = new(maxElementCount, UnsafeUtility.SizeOf<UIElement>());
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddElement(
            ShortRect rect,
            byte type,
            byte effects,
            uint id,
            ushort clip, 
            byte texture, 
            ushort color)
        {
            unsafe
            {
                var posXMin = unchecked((ushort)rect.MinX);
                var posXMax = unchecked((ushort)rect.MaxX);
                var posYMin = unchecked((ushort)rect.MinY);
                var posYMax = unchecked((ushort)rect.MaxY);

                var uv0 = new float2(0, 1);
                var uv1 = new float2(1, 0);

                var uvX0 = (ushort)(uv0.x * 256f);
                var uvY0 = (ushort)(uv0.y * 256f);
                var uvX1 = (ushort)(uv1.x * 256f);
                var uvY1 = (ushort)(uv1.y * 256f);

                UIElement* e = (UIElement*)m_Elements.GetUnsafePtr() + Count;

                e->PosMin = ((uint)posXMin << 16) | posYMin;
                e->PosMax = ((uint)posXMax << 16) | posYMax;

                e->UVMax = ((uint)uvY1 << 16) | uvX1;
                e->UVMin = ((uint)uvY0 << 16) | uvX0;

                e->Meta0 = ((uint)type & 0xF) << 28 | 0u << 20 | id & 0xFFFFF;
                e->Meta1 = ((uint)clip & 0x1FFF) << 19 | ((uint)texture & 0x7) << 16 | color;
                e->Meta2 = 0; //unused
                e->Meta3 = 0; //unused
                e->Meta4 = 0; //unused

                Count++;
            }

            IsFull = Count + 1 >= m_Elements.Length;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(ShortRect rect, uint id, ushort clip, ReadOnlySpan<float2> uvs = default)
        {
            
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateBuffer()
        {
            Buffer.SetData(m_Elements, 0, 0, Count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Clear(Material material)
        {
            Material = material;
            Count = 0;
            IsFull = false;
        }

        public void Dispose()
        {
            Buffer.Release();
            m_Elements.Dispose();
        }
    }
}
