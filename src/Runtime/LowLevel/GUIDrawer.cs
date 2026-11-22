using System.Runtime.CompilerServices;
using UnityEngine;
using EniGUI.LowLevel;

namespace EniGUI.Internal
{
    internal static class GUIDrawer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EmitTextures(GUIDrawContext context)
        {
            var textures = context.TextureSet.Textures;
            var cmd = context.CommandBuffer;

            for (int i = 0; i < textures.Length; i++)
                cmd.SetGlobalTexture($"TEX{i}", textures[i]);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EmitDrawCommands(GUIDrawContext context, Matrix4x4 camera)
        {
            var batches = context.BatchSet.Batches;
            var elements = context.ElementSet.Elements;

            if (batches.Length == 0)
                return;

            var cmd = context.CommandBuffer;
            var buffer = context.ComputeBuffer;

            cmd.SetRenderTarget(context.MultipleTexture, context.DummyDepth);
            cmd.SetGlobalBuffer("ELEMENTS", buffer);

            for (int i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];

                cmd.SetBufferData(buffer, elements, batch.Offset, 0, batch.Count);
                cmd.DrawProcedural(camera, batch.Material, 0, MeshTopology.Triangles, batch.Count * 6);
            }
        }
    }
}
