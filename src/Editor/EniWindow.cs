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
        ID = 1
    }

    public class EniWindow : EditorWindow
    {
        private EditorGUIEventHandler m_EventHandler;

        private LifetimeHandle m_LifetimeHandle;
        private GUIContext m_Context;

        private DrawMod m_DrawMod = DrawMod.Color;

        public Material DefaultMaterial { get; private set; }
        public GUID GUID { get; private set; }
        public Vector2 Size { get; private set; }

        public Lifetime Lifetime => m_LifetimeHandle.Lifetime;

        private void OnGUI()
        {
            if (Event.current.type == EventType.Layout)
                return;

            Initialize();
            Resize();
            HandleInput();
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

            DefaultMaterial = GUI.CreateDefaultMaterial();
            DefaultMaterial.AutoDispose(Lifetime);

            m_Context = new(Matrix4x4.identity, 100, 100, 5000);
            m_Context.AutoDispose(Lifetime);

            m_EventHandler = new();

            OnOpen();
        }

        private void Resize()
        {
            if (Size == position.size)
                return;

            Size = position.size;
            m_Context.Resize((uint)Size.x, (uint)Size.y);

            OnResize();
        }

        private void HandleInput()
        {
            var e = Event.current;

            m_EventHandler.Handle(e);
            m_Context.HitTracker.Push(m_EventHandler.CurrentHit);

            if (e.type == EventType.Repaint)
                m_Context.InputTracker.Push(m_EventHandler.Capture());
        }

        private void Draw()
        {
            if (Event.current.type != EventType.Repaint)
                return;

            GUILowLevel.BeginFrame(m_Context);
            GUILowLevel.SetMaterial(DefaultMaterial);
            OnDraw();
            GUILowLevel.ResetID();
            GUILowLevel.EndFrame();

            Texture image = m_DrawMod switch
            {
                DrawMod.Color => m_Context.Color,
                DrawMod.ID => m_Context.ID,
                _ => m_Context.Color,
            };

            UnityEngine.GUI.DrawTexture(
                new Rect(Vector2.zero, Size),
                image,
                ScaleMode.StretchToFill,
                true);

            Repaint();
        }

        public void SwitchDrawMod(DrawMod drawMod) => m_DrawMod = drawMod;

        protected virtual void OnOpen() { }
        protected virtual void OnClose() { }
        protected virtual void OnDraw() { }
        protected virtual void OnResize() { }
    }
}