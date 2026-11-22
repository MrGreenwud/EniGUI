using System;
using UnityEngine;
using UnityEngine.Rendering;
using EniLife;
using EniGUI.Internal;
using Unity.Collections.LowLevel.Unsafe;

namespace EniGUI.LowLevel
{
    internal readonly struct GUIDrawContext
    {
        public readonly RenderTargetIdentifier[] MultipleTexture;
        public readonly RenderTexture DummyDepth;
        public readonly CommandBuffer CommandBuffer;
        public readonly ComputeBuffer ComputeBuffer;
        public readonly ElementSet ElementSet;
        public readonly TextureSet TextureSet;
        public readonly BatchSet BatchSet;

        public GUIDrawContext(
            RenderTargetIdentifier[] multipleTexture,
            RenderTexture dummyDepth,
            CommandBuffer commandBuffer,
            ComputeBuffer computeBuffer,
            ElementSet elementSet,
            TextureSet textureSet,
            BatchSet batchSet)
        {
            MultipleTexture = multipleTexture;
            DummyDepth = dummyDepth;
            CommandBuffer = commandBuffer;
            ComputeBuffer = computeBuffer;
            ElementSet = elementSet;
            TextureSet = textureSet;
            BatchSet = batchSet;
        }
    }

    public sealed class GUIContext : IDisposable
    {
        internal Matrix4x4 ViewProjection { get; private set; }

        internal readonly RenderTargetIdentifier[] MultipleTexture;

        internal readonly CommandBuffer CommandBuffer;
        internal readonly ComputeBuffer ComputeBuffer;

        internal readonly ElementSet ElementSet;
        internal readonly TextureSet TextureSet;
        internal readonly BatchSet BatchSet;

        internal readonly IDStack IDStack;
        internal readonly ClipStack ClipStack;

        public readonly GUIInputTracker InputTracker;
        public readonly GUIHitTracker HitTracker;

        internal RenderTexture DummyDepth { get; private set; }

        public RenderTexture Color { get; private set; }
        public RenderTexture ID { get; private set; }

        internal GUIDrawContext DrawContext => new(
            MultipleTexture, 
            DummyDepth, 
            CommandBuffer, 
            ComputeBuffer, 
            ElementSet, 
            TextureSet,
            BatchSet);

        public GUIContext(Matrix4x4 viewProjection, uint width, uint height, int maxElementCountPerBatch)
        {
            MultipleTexture = new RenderTargetIdentifier[2];
            Resize(viewProjection, width, height);

            ElementSet = new();
            TextureSet = new();
            BatchSet = new(maxElementCountPerBatch);

            CommandBuffer = new();
            ComputeBuffer = new(maxElementCountPerBatch, UnsafeUtility.SizeOf<UIElement>());

            IDStack = new();
            ClipStack = new();

            InputTracker = new();
            HitTracker = new();
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
            DummyDepth = new((int)width / 2, (int)height / 2, 0, RenderTextureFormat.R16);

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

            CommandBuffer.Dispose();
            ComputeBuffer.Release();

            ClipStack.Dispose();
            ElementSet.Dispose();
        }
    }
}
