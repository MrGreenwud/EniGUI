using System;
using UnityEngine;
using EniLife;

namespace EniGUI.Renderer.LowLevel
{
    public sealed class DrawContext : IDisposable
    {
        public readonly RenderTexture Color;
        public readonly RenderTexture ID;

        public DrawContext(uint with, uint height)
        {
            Color = new((int)with, (int)height, 24, RenderTextureFormat.ARGB32);
            ID = new((int)with / 2, (int)height / 2, 24, RenderTextureFormat.ARGB32);

            Color.hideFlags = HideFlags.DontSave;
            ID.hideFlags = HideFlags.DontSave;
        }

        public void Clear()
        {
            var originTexture = RenderTexture.active;

            RenderTexture.active = Color;
            GL.Clear(true, true, UnityEngine.Color.clear);

            RenderTexture.active = ID;
            GL.Clear(true, true, UnityEngine.Color.clear);

            RenderTexture.active = originTexture;
        }

        public void Dispose()
        {
            Color.Dispose();
            ID.Dispose();
        }
    }
}
