using System;
using UnityEngine;
using UnityEngine.Rendering;
using EniLife;

namespace EniGUI.LowLevel
{
    internal readonly struct GUIDrawContext
    {
        public readonly RenderTargetIdentifier[] MultipleTexture;
        public readonly RenderTexture DummyDepth;
        public readonly CommandBuffer CommandBuffer;
        public readonly BatchSet BatchSet;

        public GUIDrawContext(
            RenderTargetIdentifier[] multipleTexture,
            RenderTexture dummyDepth,
            CommandBuffer commandBuffer,
            BatchSet batchSet)
        {
            MultipleTexture = multipleTexture;
            DummyDepth = dummyDepth;
            CommandBuffer = commandBuffer;
            BatchSet = batchSet;
        }
    }

    public sealed class GUIContext : IDisposable
    {
        private const uint BATCH_SET_CAPACITY = 16;
        private static Material m_Material;

        internal Matrix4x4 ViewProjection { get; private set; }

        internal readonly RenderTargetIdentifier[] MultipleTexture;
        internal readonly CommandBuffer CommandBuffer;

        internal readonly BatchSet BatchSet;
        internal readonly TextureSet TextureSet;

        internal readonly IDStack IDStack;
        internal readonly ClipStack ClipStack;

        internal RenderTexture DummyDepth { get; private set; }

        public RenderTexture Color { get; private set; }
        public RenderTexture ID { get; private set; }

        internal GUIDrawContext DrawContext => new (MultipleTexture, DummyDepth, CommandBuffer, BatchSet);

        public GUIContext(Matrix4x4 viewProjection, uint width, uint height, uint maxVertexCountPerBatch)
        {
            MultipleTexture = new RenderTargetIdentifier[2];
            Resize(viewProjection, width, height);

            if(m_Material == null)
                m_Material = new (Shader.Find("EniGUI/Default"));

            BatchSet = new BatchSet(m_Material, BATCH_SET_CAPACITY, maxVertexCountPerBatch);
            TextureSet = new ();

            CommandBuffer = new CommandBuffer();

            IDStack = new IDStack();
            ClipStack = new ClipStack();
        }

        public void Resize(uint width, uint height)
        {
            Matrix4x4 viewProjection = new();
            viewProjection.m00 = 2f / width;
            viewProjection.m11 = 2f / height;
            viewProjection.m22 = 1f;
            viewProjection.m33 = 1f;
            viewProjection.m03 = -1f;
            viewProjection.m13 = -1f;

            Resize(viewProjection, width, height);
        }

        public void Resize(Matrix4x4 viewProjection, uint width, uint height)
        {
            Color?.Dispose();
            ID?.Dispose();
            DummyDepth?.Dispose();

            Color = new((int)width, (int)height, 0, RenderTextureFormat.ARGB32);
            ID = new((int)width, (int)height, 0, RenderTextureFormat.RG32);
            DummyDepth = new((int)width, (int)height, 0, RenderTextureFormat.R16);

            Color.hideFlags = HideFlags.DontSave;
            ID.hideFlags = HideFlags.DontSave;
            DummyDepth.hideFlags = HideFlags.DontSave;

            MultipleTexture[0] = Color;
            MultipleTexture[1] = ID;

            ViewProjection = viewProjection;
        }

        public void Clear()
        {
            BatchSet.Clear();
            TextureSet.Clear();
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
