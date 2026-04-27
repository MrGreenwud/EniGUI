using System;
using UnityEngine;

namespace EniGUI.LowLevel
{
    public static class EniGUIUtility
    {
        public static uint GetControlID(uint hint, Rect rect, string name = "")
        {
            uint hash = (uint)HashCode.Combine(rect.x, rect.y, rect.width, rect.height, name);

            hash ^= hint;
            hash *= 16777619u;

            return hash;
        }

        public static uint DecodeID(ushort r, ushort g) => (uint)(r | (g << 16));
    }
}