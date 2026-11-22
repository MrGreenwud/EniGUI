using UnityEngine;
using EniGUI.LowLevel;
using UnityEngine.Assertions;

namespace EniGUI
{
    public static class GUI
    {
        public static Material CreateDefaultMaterial()
        {
            var shader = Shader.Find("EniGUI/Default");
            Assert.IsNotNull(shader, "Shader EniGUI/Default not found");
            
            return new Material(shader) 
            { 
                hideFlags = HideFlags.HideAndDontSave 
            };
        }

        public static InputState Input => GUILowLevel.Input;
        public static HitState Hit => GUILowLevel.Hit;

        public static void Button(ShortRect rect, Texture texture)
        {
            Button(rect, texture, (uint)rect.GetHashCode());
        }
        
        public static bool Button(ShortRect rect, Texture texture, uint id)
        {
            var hash = GUILowLevel.PushID(id);
            Image(rect, texture);
            GUILowLevel.PopID();

            return Input.Released(GUIKeyCode.Mouse0)
                && Hit.Hover(hash);
        }

        public static void Text(ShortRect rect, string text)
        {
            GUILowLevel.DrawText(rect, text);
        }
        
        public static void Image(ShortRect rect, Texture texture)
        {
            GUILowLevel.BindTexture(texture);
            GUILowLevel.DrawQuad(rect);
            GUILowLevel.UnbindTexture();
        }

        public static void PushID(uint id)
        {
            GUILowLevel.PushID(id);
        }

        public static void PopID()
        {
            GUILowLevel.PopID();
        }
    }
}
