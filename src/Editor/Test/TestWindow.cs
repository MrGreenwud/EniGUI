using EniGUI.Editor;
using EniLife;
using UnityEngine;
using UnityEditor;

namespace EniGUI.Test
{
    public class TestWindow : EniWindow
    {
        private Material Material;

        [MenuItem("EniGUI/Test")]
        private static void Open()
        {   
            var window = GetWindow(typeof(TestWindow));
            window.titleContent = new GUIContent("Test Window");
            window.Show();
        }

        protected override void OnOpen()
        {
            Material = new Material(Shader.Find("EniGUI/Default"));
            Material.hideFlags = HideFlags.DontSave;
            Material.AutoDispose(Lifetime);
        }

        protected override void OnDraw()
        {
            LowLevel.GUIDrawer.DrawQuad(new Rect(0, 0, 100, 100), Material);
            LowLevel.GUIDrawer.DrawQuad(new Rect(120, 0, 100, 100), Material);
            LowLevel.GUIDrawer.DrawQuad(new Rect(100, 100, 20, 20), Material);
            LowLevel.GUIDrawer.DrawQuad(new Rect(60, 130, 15, 15), Material);
        }
    }
}