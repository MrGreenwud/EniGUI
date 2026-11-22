using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using EniGUI.Internal;

namespace EniGUI.LowLevel
{
    public static class GUILowLevel
    {
        private static GUIContext m_Context;

        private static BatchSet s_BatchSet;
        private static ElementSet s_ElementSet;
        private static TextureSet s_TextureSet;

        private static CommandBuffer s_CommandBuffer;

        private static ClipStack s_ClipStack;
        private static IDStack s_IDStack;

        private static Matrix4x4 s_Camera;
        private static Material s_Material;

        public static InputState Input => new(m_Context.InputTracker.Current);
        public static HitState Hit => new(m_Context.HitTracker.Current, m_Context.HitTracker.Ago(1));

        public static void BeginFrame(GUIContext context)
        {
            m_Context = context;

            s_ElementSet = context.ElementSet;
            s_TextureSet = context.TextureSet;
            s_BatchSet = context.BatchSet;

            s_CommandBuffer = context.CommandBuffer;

            s_ClipStack = context.ClipStack;
            s_IDStack = context.IDStack;

            context.Clear();

            s_CommandBuffer.SetRenderTarget(context.MultipleTexture, context.DummyDepth);
            s_CommandBuffer.ClearRenderTarget(true, true, Color.clear);

            s_CommandBuffer.SetGlobalMatrix("GUI_MATRIX_VP", context.ViewProjection);
            s_Camera = Matrix4x4.identity;
        }
        public static void EndFrame()
        {
            Flush();
            Graphics.ExecuteCommandBuffer(s_CommandBuffer);

            s_ElementSet = null;
            s_TextureSet = null;
            s_BatchSet = null;

            s_CommandBuffer = null;

            s_ClipStack = null;
            s_IDStack = null;
        }

        public static bool BeginLayer(int id) { return true; }
        public static void EndLayer() { }

        public static void RepaintLayer(int id) { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(Texture texture)
        {
            if (s_TextureSet.TryBind(texture))
                return;

            Flush();
            s_TextureSet.TryBind(texture);
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnbindTexture() => s_TextureSet.Unbind();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PushClip(Rect rect) => s_ClipStack.Push(rect);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopClip() => s_ClipStack.Pop();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint PushID(uint id) => s_IDStack.Push(id);
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void PopID() => s_IDStack.Pop();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void ResetID() => s_IDStack.Reset();

        public static void SetCamera(Matrix4x4 camera)
        {
            Flush();
            s_Camera = camera;
        }

        public static void SetMaterial(Material material)
        {
            if(s_Material == null)
                s_Material = material;

            if (s_Material == material)
                return;

            Flush();
            s_Material = material;
        }

        public static void DrawQuad(ShortRect rect)
        {
            s_ElementSet.Add(
                rect,
                0,
                0,
                s_IDStack.Current,
                s_ClipStack.Current,
                s_TextureSet.CurrentSlot,
                0);
        }
        public static void DrawRoundedQuad(ShortRect rect)
        {
            s_ElementSet.Add(
                rect,
                1,
                0,
                s_IDStack.Current,
                (byte)s_ClipStack.Current,
                s_TextureSet.CurrentSlot,
                0);
        }
        public static void DrawText(ShortRect rect, string text)
        {
            s_ElementSet.Add(
                rect,
                2,
                0,
                s_IDStack.Current,
                s_ClipStack.Current,
                s_TextureSet.CurrentSlot,
                0);
        }
        public static void DrawLine(ShortRect rect)
        {
            s_ElementSet.Add(
                rect,
                3,
                0,
                s_IDStack.Current,
                s_ClipStack.Current,
                s_TextureSet.CurrentSlot,
                0);
        }
        public static void DrawBezier(ShortRect rect)
        {
            s_ElementSet.Add(
                rect,
                4,
                0,
                s_IDStack.Current,
                s_ClipStack.Current,
                s_TextureSet.CurrentSlot,
                0);
        }

        public static uint PickID(Vector2 pointer)
        {
            var id = m_Context.ID;

            var x = (int)pointer.x;
            var y = id.height - 1 - (int)pointer.y;

            if ((uint)x >= (uint)id.width || (uint)y >= (uint)id.height)
                return 0;

            var request = AsyncGPUReadback.Request(id, 0, x, 1, y, 1, 0, 1);
            request.WaitForCompletion();

            if(request.hasError)
                return 0;

            return request.GetData<uint>()[0] & 0xFFFFFu;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Flush()
        {
            if (s_ElementSet.Count == 0)
                return;

            s_BatchSet.Split(s_ElementSet.Count, s_Material);

            GUIDrawer.EmitTextures(m_Context.DrawContext);
            GUIDrawer.EmitDrawCommands(m_Context.DrawContext, s_Camera);

            s_ElementSet.Clear();
            s_TextureSet.Clear();
            s_BatchSet.Clear();
        }
    }
}
