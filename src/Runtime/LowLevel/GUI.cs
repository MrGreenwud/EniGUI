using EniGUI.LowLevel;

namespace EniGUI
{
    public static class GUI
    {
        public static void Button(ShortRect rect, GUITexture texture)
        {
            Button(rect, texture, (uint)rect.GetHashCode());
        }
        
        public static void Button(ShortRect rect, GUITexture texture, uint id)
        {
            var hesh = GUILowLevel.PushID(id);
            Image(rect, texture);
            GUILowLevel.PopID();
        }

        public static void Text(ShortRect rect, string text)
        {
            GUILowLevel.DrawText(rect, text);
        }
        
        public static void Image(ShortRect rect, GUITexture texture)
        {
            GUILowLevel.BindTexture(texture);
            GUILowLevel.DrawQuad(rect);
            GUILowLevel.UnbindTexture();
        }

        public static void PushID(uint id) => GUILowLevel.PushID(id);
        public static void PopID() => GUILowLevel.PopID();
    }
}
