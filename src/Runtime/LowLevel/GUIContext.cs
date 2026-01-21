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

        internal RenderTexture DummyDepth { get; private set; }

        public RenderTexture Color { get; private set; }
        public RenderTexture ID { get; private set; }

        public GUIContext(uint with, uint height, uint maxVertexCountPerBatch)
        {
            MultipleTexture = new RenderTargetIdentifier[2];
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
            DummyDepth?.Dispose();

            Color = new((int)with, (int)height, 0, RenderTextureFormat.ARGB32);
            ID = new((int)with, (int)height, 0, RenderTextureFormat.RG32);
            DummyDepth = new((int)with, (int)height, 0, RenderTextureFormat.R16);

            Color.hideFlags = HideFlags.DontSave;
            ID.hideFlags = HideFlags.DontSave;
            DummyDepth.hideFlags = HideFlags.DontSave;

            MultipleTexture[0] = Color;
            MultipleTexture[1] = ID;
        }

        public void Clear()
        {
            BatchSet.Clear();
            CommandBuffer.Clear();

            CommandBuffer.SetRenderTarget(MultipleTexture, DummyDepth);
            CommandBuffer.ClearRenderTarget(true, true, UnityEngine.Color.clear);
            Graphics.ExecuteCommandBuffer(CommandBuffer);

            CommandBuffer.Clear();
        }

        public void Dispose()
        {
            MultipleTexture[0] = default;
            MultipleTexture[1] = default;

            Color.Dispose();
            ID.Dispose();
            DummyDepth.Dispose();

            BatchSet.Dispose();
            CommandBuffer.Dispose();
            ClipStack.Dispose();
        }
    }
}
