using System.Runtime.CompilerServices;
using UnityEngine;
using EniGUI.LowLevel;

namespace EniGUI.Internal
{
    internal static class GUIDrawer
    {
        private static GUIDrawContext s_Context;

        public static void BindContext(GUIDrawContext context) => s_Context = context;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void EmitDrawCommands(Matrix4x4 camera)
        {
            var batches = s_Context.BatchSet.Batches;

            if (batches.Length == 0)
                return;

            var cmd = s_Context.CommandBuffer;

            cmd.SetRenderTarget(s_Context.MultipleTexture, s_Context.DummyDepth);

            for (int i = 0; i < batches.Length; i++)
            {
                var batch = batches[i];

                batch.UpdateBuffer();

                cmd.SetGlobalBuffer("ELEMENTS", batch.ElementBuffer);
                cmd.DrawProcedural(camera, batch.Material, 0, MeshTopology.Triangles, batch.Count * 6);
            }
        }
    }
}
