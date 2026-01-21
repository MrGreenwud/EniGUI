using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
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

    internal sealed class ClipStack : IDisposable
    {
        private const int CLIPS_MAX_COUNT = 1024;

        private readonly static float4 s_DefaultClip = 
            new(float.MinValue, 
                float.MaxValue, 
                float.MaxValue, 
                float.MinValue);

        private readonly Stack<int> m_PreviousClips = new();
        private readonly Stack<float2> m_PreviousOrigin = new();
        private readonly float4[] m_Clips = new float4[CLIPS_MAX_COUNT];
        
        private int m_LastClip;

        public int Current { get; private set; }
        public float2 Origin { get; private set; }

        public readonly ComputeBuffer Buffer;

        public ClipStack()
        {
            Buffer = new(CLIPS_MAX_COUNT, sizeof(float) * 4);
            Buffer.SetData(m_Clips);
        }

        public void Push(Rect rect)
        {
            float4 current = Current >= -1 ? m_Clips[Current] : s_DefaultClip;

            float4 clip = new(
                Origin.x + rect.xMin,
                Origin.y + rect.yMax,
                Origin.x + rect.xMax,
                Origin.y + rect.yMin);

            if(!(clip.x >= current.x && clip.x <= current.z 
                && clip.y <= current.y && clip.y >= current.w))
            {
                Debug.LogError("Clip don't overlap current clip");
            }

            clip.x = Mathf.Max(clip.x, current.x);
            clip.y = Mathf.Min(clip.y, current.y);
            clip.z = Mathf.Min(clip.z, current.z);
            clip.w = Mathf.Max(clip.w, current.w);

            m_PreviousClips.Push(Current);
            m_PreviousOrigin.Push(Origin);

            m_Clips[m_LastClip] = clip;
            Current = m_LastClip;
            m_LastClip++;

            Origin += new float2(rect.x, rect.y);
        }
        public void Pop()
        {
            Origin = m_PreviousOrigin.Pop();
            Current = m_PreviousClips.Pop();
        }

        public void Reset()
        {
            m_PreviousClips.Clear();
            m_PreviousOrigin.Clear();

            m_LastClip = 0;
            Current = -1;
            Origin = new(0, 0);
        }

        public void Dispose()
        {
            Buffer.Dispose();
        }
    }

    public static class EniGUIUtility
    {
        public static uint GetControlID(uint hint, Rect rect, string name = "")
        {
            uint hash = (uint)HashCode.Combine(rect.x, rect.y, rect.width, rect.height, name);

            hash ^= hint;
            hash *= 16777619u;

            return hash;
        }

        public static uint DecodeID(ushort r, ushort g)
        {
            uint id = (uint)(r | (g << 16));
            return id;
        }
    }
}