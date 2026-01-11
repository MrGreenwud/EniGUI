using System;
using UnityEngine;
using UnityEngine.Rendering;
using EniLife;

namespace EniGUI.LowLevel
{
    public sealed class GUIContext : IDisposable
    {
        private const uint BATCH_SET_CAPACITY = 16;

        internal readonly RenderTargetIdentifier[] MultipleTexture;
        internal readonly CommandBuffer CommandBuffer;
        internal readonly BatchSet BatchSet;

        internal readonly IDStack IDStack;
        internal readonly ClipStack ClipStack;

        public RenderTexture Color { get; private set; }
        public RenderTexture ID { get; private set; }
        public RenderTexture Depth { get; private set; }

        public GUIContext(uint with, uint height, uint maxVertexCountPerBatch)
        {
            MultipleTexture = new RenderTargetIdentifier[3];
            Resize(with, height);

            BatchSet = new BatchSet(BATCH_SET_CAPACITY, maxVertexCountPerBatch);
            CommandBuffer = new CommandBuffer();

            IDStack = new IDStack();
            ClipStack = new ClipStack();
        }

        public void Resize(uint with, uint height)
        {
            Color?.Dispose();
            ID?.Dispose();
            Depth?.Dispose();

            Color = new((int)with, (int)height, 0, RenderTextureFormat.ARGB32);
            ID = new((int)with / 2, (int)height / 2, 0, RenderTextureFormat.RG32);
            Depth = new((int)with, (int)height, 0, RenderTextureFormat.R16);

            Color.hideFlags = HideFlags.DontSave;
            ID.hideFlags = HideFlags.DontSave;
            Depth.hideFlags = HideFlags.DontSave;

            MultipleTexture[0] = Color;
            MultipleTexture[1] = ID;
            MultipleTexture[2] = Depth;
        }

        public void Clear()
        {
            var originTexture = RenderTexture.active;

            RenderTexture.active = Color;
            GL.Clear(true, true, UnityEngine.Color.clear);

            RenderTexture.active = ID;
            GL.Clear(true, true, UnityEngine.Color.clear);

            RenderTexture.active = Depth;
            GL.Clear(true, true, UnityEngine.Color.clear);

            BatchSet.Clear();
            CommandBuffer.Clear();

            RenderTexture.active = originTexture;
        }

        public void Dispose()
        {
            MultipleTexture[0] = default;
            MultipleTexture[1] = default;
            MultipleTexture[2] = default;

            Color.Dispose();
            ID.Dispose();
            Depth.Dispose();

            BatchSet.Dispose();
            CommandBuffer.Dispose();
        }
    }
}
