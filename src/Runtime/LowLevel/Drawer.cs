using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;

namespace EniGUI.Renderer.LowLevel
{
    public static class Drawer
    {
        private const uint MAX_ELEMENT_COUNT_PER_LAYER = 10000;
        private const float DEPTH_STEP_PER_ELEMENT = 0.2f;

        private readonly static CommandBuffer s_CommandBuffer = new();
        private readonly static BatchSet s_BatchSet = new(16);

        private static DrawContext s_CurrentContext = null;
        private static uint s_CurrentCountElement;

        private static Matrix4x4 s_Camera;

        public static void BeginFrame(DrawContext context)
        {
            Assert.IsNull(s_CurrentContext, "Begin error");

            s_CurrentContext = context;
            s_CurrentContext.Clear();
            s_CommandBuffer.Clear();
        }
        public static void EndFrame()
        {
            Assert.IsNotNull(s_CurrentContext, "End error");

            DrawLayer();

            Graphics.ExecuteCommandBuffer(s_CommandBuffer);
            s_CommandBuffer.Clear();
            s_BatchSet.Clear();

            s_CurrentContext = null;
            s_Camera = Matrix4x4.identity;
        }

        public static void SetCamera(Matrix4x4 camera) => s_Camera = camera;
        public static void SetClip(Rect clipRect) => s_CommandBuffer.EnableScissorRect(clipRect);

        public static void DrawQuad(Rect rect, uint id, Material material, ReadOnlySpan<Vector2> uvs = default)
        {
            BatchData batchData = s_BatchSet.Get(material);

            float depth = -MAX_ELEMENT_COUNT_PER_LAYER + (s_CurrentCountElement * DEPTH_STEP_PER_ELEMENT);
            batchData.AddQuad(rect, id, depth, uvs);

            s_CurrentCountElement++;

            if (s_CurrentCountElement >= MAX_ELEMENT_COUNT_PER_LAYER)
                DrawLayer();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void DrawLayer()
        {
            var batches = s_BatchSet.Batches;

            if (batches.Length == 0)
                return;

            s_CommandBuffer.SetRenderTarget(s_CurrentContext.Color);
            for (int i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                batch.UpdateMesh();
                s_CommandBuffer.DrawMesh(batch.Mesh, s_Camera, batch.Material, 0, 0);
            }

            s_CommandBuffer.ClearRenderTarget(true, false, Color.clear);

            s_CommandBuffer.SetRenderTarget(s_CurrentContext.ID);
            for (int i = 0; i < batches.Length; i++)
                s_CommandBuffer.DrawMesh(batches[i].Mesh, s_Camera, null, 0, 0);

            s_CommandBuffer.ClearRenderTarget(true, false, Color.clear);
            s_CurrentCountElement = 0;
        }
    }
}
