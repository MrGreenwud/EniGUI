using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Assertions;
using Unity.Mathematics;

namespace EniGUI.LowLevel
{
    public static class GUIDrawer
    {
        private static GUIContext s_Context = null;
        private static CommandBuffer s_CommandBuffer;
        private static BatchSet s_BatchSet;

        private static IDStack s_IDStack;
        private static ClipStack s_ClipStack;

        private static Matrix4x4 s_Camera;

        public static void BeginFrame(GUIContext context)
        {
            Assert.IsNull(s_Context, "Begin error");

            s_Context = context;
            s_CommandBuffer = s_Context.CommandBuffer;
            s_BatchSet = s_Context.BatchSet;

            s_IDStack = s_Context.IDStack;
            s_ClipStack = s_Context.ClipStack;

            s_Context.Clear();

            Matrix4x4 vp = new Matrix4x4();
            vp.m00 = 2f / s_Context.Color.width;
            vp.m11 = 2f / s_Context.Color.height;
            vp.m22 = 1f;
            vp.m33 = 1f;
            vp.m03 = -1f;
            vp.m13 = -1f;

            s_CommandBuffer.SetGlobalMatrix("GUI_MATRIX_VP", vp);
        }
        public static void EndFrame()
        {
            Assert.IsNotNull(s_Context, "End error");

            EmitDrawCommands();
            Graphics.ExecuteCommandBuffer(s_CommandBuffer);
            
            s_Context = null;
            s_CommandBuffer = null;
            s_BatchSet = null;
            s_IDStack = null;
            s_ClipStack = null;

            s_Camera = Matrix4x4.identity;
        }

        public static void SetCamera(Matrix4x4 camera)
        {
            EmitDrawCommands();
            s_Camera = camera;
        }

        public static void PushClip(Rect rect) => s_ClipStack.Push(rect);
        public static void PopClip() => s_ClipStack.Pop();

        public static void PushID(uint id) => s_IDStack.Push(id);
        public static void PopID() => s_IDStack.Pop();

        public static void DrawQuad(Rect rect, Material material, ReadOnlySpan<float2> uvs = default)
        {
            BatchData batchData = s_BatchSet.GetBatch(material);
            batchData.AddQuad(rect, 10, s_IDStack.Current, (ushort)s_ClipStack.Current, uvs);
        }
    
        private static void EmitDrawCommands()
        {
            var batches = s_BatchSet.Batches;

            if(batches.Length == 0)
                return;

            s_CommandBuffer.SetRenderTarget(s_Context.Color);
            for (int i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];
                batch.UpdateMesh();
                s_CommandBuffer.DrawMesh(batch.Mesh, s_Camera, batch.Material, 0, 0);
            }

            s_BatchSet.Break();
        }
    }
}
