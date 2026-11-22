using EniGUI.Editor;
using EniLife;
using UnityEngine;
using UnityEditor;

namespace EniGUI.Test
{
    public class TestWindow : EniWindow
    {
        private Material Material;
        private Texture m_TestTexture;

        [MenuItem("EniGUI/Test")]
        private static void Open()
        {
            var window = GetWindow(typeof(TestWindow));
            window.titleContent = new GUIContent("Test Window");
            window.Show();
        }

        protected override void OnOpen()
        {
            Material = new (Shader.Find("EniGUI/Default"));
            Material.hideFlags = HideFlags.DontSave;
            Material.AutoDispose(Lifetime);

            m_TestTexture = Resources.Load<Texture>("Blue-Button-No-Background");
            SwitchDrawMod(DrawMod.Color);
        }

        protected override void OnDraw()
        {
            //LowLevel.GUIDrawer.DrawQuad(new Rect(0, 0, 100, 100), Material);
            //LowLevel.GUIDrawer.DrawQuad(new Rect(120, 0, 100, 100), Material);
            //LowLevel.GUIDrawer.DrawQuad(new Rect(100, 100, 20, 20), Material);
            //LowLevel.GUIDrawer.DrawQuad(new Rect(60, 130, 15, 15), Material);

            //GUILowLevel.BeginFrame(m_Context);

            //GUI.Button(new ShortRect(x, y, 640, 320), texture, 1);
            //GUI.Button(new ShortRect(x, 50, 640, 320), texture, 2);

            //GUILowLevel.BindTexture(new LowLevel.GUITexture(1, m_TestTexture));
            //GUILowLevel.UnbindTexture();

            //GUILowLevel.PushID(100);
            //GUILowLevel.BindTexture(texture);
            //GUILowLevel.DrawQuad(new ShortRect(0, 0, 100, 200));
            //GUILowLevel.UnbindTexture();
            //GUILowLevel.PopID();
            //GUILowLevel.PushID(300);
            //GUILowLevel.DrawQuad();
            //GUILowLevel.PopID();

            short y = 0;
            short x = 0;

            uint width = (uint)Screen.width / 8;
            uint height = (uint)Screen.height / 8;

            if(GUI.Button(new(0, 0, 300, 150), m_TestTexture, 1))
            {
                Debug.Log(1);
            }

            //for (uint i = 0; i < 100; i++)
            //{
            //    GUI.PushID(i);
            //    for (int j = 0; j < 100; j++)
            //    {
            //        GUI.Button(new ShortRect(x, y, 8, 8), texture, (uint)(i + j));
            //        y += 8;
            //    }
            //    GUI.PopID();

            //    y = 0;
            //    x += 8;
            //}

            //GUILowLevel.EndFrame();
        }
    }
}