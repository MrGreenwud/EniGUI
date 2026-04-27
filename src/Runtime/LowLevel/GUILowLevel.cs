using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using EniGUI.Internal;

namespace EniGUI.LowLevel
{
    public static class GUILowLevel
    {
        private static BatchSet s_BatchSet;
        private static TextureSet s_TextureSet;
        private static CommandBuffer s_CommandBuffer;

        private static ClipStack s_ClipStack;
        private static IDStack s_IDStack;

        private static Matrix4x4 s_Camera;

        public static void BeginFrame(GUIContext context)
        {
            s_BatchSet = context.BatchSet;
            s_TextureSet = context.TextureSet;
            s_CommandBuffer = context.CommandBuffer;

            s_ClipStack = context.ClipStack;
            s_IDStack = context.IDStack;

            context.Clear();

            s_CommandBuffer.SetRenderTarget(context.MultipleTexture, context.DummyDepth);
            s_CommandBuffer.ClearRenderTarget(true, true, Color.clear);

            s_CommandBuffer.SetGlobalMatrix("GUI_MATRIX_VP", context.ViewProjection);
            s_Camera = Matrix4x4.identity;

            GUIDrawer.BindContext(context.DrawContext);
        }
        public static void EndFrame() 
        {
            EmitTextures();
            EmitDrawCommands();
            Graphics.ExecuteCommandBuffer(s_CommandBuffer);

            s_BatchSet = null;
            s_TextureSet = null;
            s_CommandBuffer = null;

            s_ClipStack = null;
            s_IDStack = null;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BindTexture(GUITexture texture)
        {
            if (s_TextureSet.TryBind(texture))
                return;

            EmitTextures();
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

        public static void SetCamera(Matrix4x4 camera)
        {
            EmitDrawCommands();
            s_Camera = camera;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawQuad(ShortRect rect)
        {
            var batch = s_BatchSet.GetBatch();
            batch.AddElement(
                rect,
                s_IDStack.Current, 
                (byte)s_ClipStack.Current, 
                s_TextureSet.Current, 
                0,
                0);
        }

        public static void DrawRoundedQuad(ShortRect rect)
        {
            var batch = s_BatchSet.GetBatch();
            batch.AddElement(
                rect,
                s_IDStack.Current,
                (byte)s_ClipStack.Current,
                s_TextureSet.Current,
                0,
                1);
        }
        public static void DrawText(ShortRect rect, string text)
        {
            var batch = s_BatchSet.GetBatch();
            batch.AddElement(
                rect,
                s_IDStack.Current,
                0,
                (byte)s_ClipStack.Current,
                s_TextureSet.Current,
                2);
        }
        public static void DrawLine(ShortRect rect)
        {
            var batch = s_BatchSet.GetBatch();
            batch.AddElement(
                rect,
                s_IDStack.Current,
                0,
                (byte)s_ClipStack.Current,
                s_TextureSet.Current,
                3);
        }
        public static void DrawBezier(ShortRect rect)
        {
            var batch = s_BatchSet.GetBatch();
            batch.AddElement(
                rect,
                s_IDStack.Current,
                0,
                (byte)s_ClipStack.Current,
                s_TextureSet.Current,
                4);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EmitDrawCommands() => GUIDrawer.EmitDrawCommands(s_Camera);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EmitTextures()
        {
            var textures = s_TextureSet.Textures;

            for (int i = 0; i < textures.Length; i++)
                s_CommandBuffer.SetGlobalTexture($"TEX{i}", textures[i]);

            s_TextureSet.Clear();
        }
    }
}
