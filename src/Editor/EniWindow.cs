using UnityEditor;
using UnityEngine;
using EniLife;
using EniLife.Editor;
using EniGUI.LowLevel;

namespace EniGUI.Editor
{
    public enum DrawMod
    {
        Color = 0,
        ID = 1,
        Depth = 2,
    }

    public class EniWindow : EditorWindow
    {
        private LifetimeHandle m_LifetimeHandle;
        private GUIContext m_DrawContext;
        private DrawMod m_DrawMod = DrawMod.Color;

        public GUID GUID { get; private set; }
        public Vector2 Size { get; private set; }

        public Lifetime Lifetime => m_LifetimeHandle.Lifetime;

        private void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
                return;

            Initialize();
            Resize();
            Draw();
        }

        private void OnDisable()
        {
            m_LifetimeHandle.Dispose();
            m_LifetimeHandle = null;

            OnClose();
        }

        private void Initialize()
        {
            if (m_LifetimeHandle != null)
                return;

            m_LifetimeHandle = EditorLifetime.Global.CreateNested();
            GUID = GUID.Generate();

            m_DrawContext = new GUIContext(100, 100, 10000);
            m_DrawContext.AutoDispose(Lifetime);

            OnOpen();
        }
        private void Resize()
        {
            if (Size == position.size)
                return;

            Size = position.size;
            m_DrawContext.Resize((uint)Size.x, (uint)Size.y);

            OnResize();
        }
        private void Draw()
        {
            LowLevel.GUIDrawer.BeginFrame(m_DrawContext);
            OnDraw();
            LowLevel.GUIDrawer.EndFrame();

            GUI.DrawTexture(new Rect(Vector2.zero, Size), m_DrawContext.Color, ScaleMode.StretchToFill, true);
        }

        public void SwitchDrawMod(DrawMod drawMod) => m_DrawMod = drawMod;

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnDraw() { }
        protected virtual void OnResize() { }
    }
}