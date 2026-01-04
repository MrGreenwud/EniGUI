using UnityEditor;
using UnityEngine;
using EniLife;
using EniLife.Editor;
using EniGUI.Renderer.LowLevel;

namespace EniGUI.Editor
{
    public class EniWindow : EditorWindow
    {
        private LifetimeHandle m_WindowLifetime;
        private LifetimeHandle m_ScreenLifetime;

        private DrawContext m_DrawContext;

        public GUID GUID { get; private set; }
        public Vector2 Size { get; private set; }

        public Lifetime OwnLifetime => m_WindowLifetime.Lifetime;

        private void OnGUI()
        {
            Initialize();
            Resize();
            Draw();
        }

        private void OnDisable()
        {
            m_WindowLifetime.Dispose();
            m_WindowLifetime = null;

            OnClose();
        }

        private void Initialize()
        {
            if (m_WindowLifetime != null)
                return;

            m_WindowLifetime = EditorLifetime.Global.CreateNested();
            GUID = GUID.Generate();

            OnOpen();
        }
        private void Resize()
        {
            if (Size == position.size)
                return;

            Size = position.size;
            m_ScreenLifetime?.Dispose();

            m_ScreenLifetime = OwnLifetime.CreateNested();
            m_DrawContext = new DrawContext((uint)Size.x, (uint)Size.y);
            m_DrawContext.AutoDispose(m_ScreenLifetime.Lifetime);

            OnResize();
        }
        private void Draw()
        {
            Drawer.BeginFrame(m_DrawContext);
            OnDraw();
            Drawer.EndFrame();

            GUI.DrawTexture(new Rect(Vector2.zero, Size), m_DrawContext.Color);
        }

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnDraw() { }
        protected virtual void OnResize() { }
    }
}