using System;
using UnityEngine;

namespace EniGUI
{
    public static class EniGUIUtility
    {
        public static uint CreateID(Rect rect, string name = "")
        {
            uint hash = (uint)HashCode.Combine(rect.x, rect.y, rect.width, rect.height, name);

            hash ^= 2166136261u;
            hash *= 16777619u;
            hash |= 0xFF000000u;

            return hash;
        }

        public static Color32 EncodeID(uint id)
        {
            byte r = (byte)(id & 0xFF);
            byte g = (byte)((id >> 8) & 0xFF);
            byte b = (byte)((id >> 16) & 0xFF);

            return new Color32(r, g, b, 255);
        }

        public static uint DecodeID(Color32 color)
        {
            if (color.a == 0 || (color.r == 0 && color.g == 0 && color.b == 0))
                return 0;

            uint id = (uint)(color.r | (color.g << 8) | (color.b << 16) | 1);
            return id;
        }
    }
}