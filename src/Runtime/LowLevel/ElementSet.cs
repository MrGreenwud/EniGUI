using System;
using System.Runtime.CompilerServices;
using EniGUI.LowLevel;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace EniGUI.Internal
{
    public sealed class ElementSet : IDisposable
    {
        private const int ELEMENTS_COPACITY = 1024;

        internal NativeArray<UIElement> Elements;

        public int Count { get; private set; }

        public ElementSet()
        {
            Elements = new(ELEMENTS_COPACITY, Allocator.Persistent);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Add(
            ShortRect rect,
            byte type,
            byte effects,
            uint id,
            ushort clip,
            ushort texture,
            ushort color)
        {
            if (Count >= Elements.Length)
                Grow();

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

                UIElement* e = (UIElement*)Elements.GetUnsafePtr() + Count;

                e->PosMin = ((uint)posXMin << 16) | posYMin;
                e->PosMax = ((uint)posXMax << 16) | posYMax;

                e->UVMax = ((uint)uvY1 << 16) | uvX1;
                e->UVMin = ((uint)uvY0 << 16) | uvX0;

                e->Meta0 = ((uint)type & 0xF) << 28 | 0u << 20 | id & 0xFFFFF;
                e->Meta1 = ((uint)clip & 0xFFu) << 24 | ((uint)texture & 0xFFu) << 20 | color & 0xFFFFFu;
                e->Meta2 = 0; //unused
                e->Meta3 = 0; //unused
                e->Meta4 = 0; //unused

                Count++;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Grow()
        {
            var temp = new NativeArray<UIElement>(Elements.Length * 2, Allocator.Persistent);

            NativeArray<UIElement>.Copy(Elements, temp, Count);

            Elements.Dispose();
            Elements = temp;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Dispose()
        {
            Elements.Dispose();
        }
    }
}