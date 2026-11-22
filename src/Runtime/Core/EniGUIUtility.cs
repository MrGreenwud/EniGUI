using System;
using UnityEngine;

namespace EniGUI.LowLevel
{
    public static class EniGUIUtility
    {
        public static uint GetControlID(uint hint, ShortRect rect, string name = "")
        {
            uint hash = (uint)HashCode.Combine(rect.X, rect.Y, rect.Width, rect.Height, name);

            hash ^= hint;
            hash *= 16777619u;

            return hash;
        }

        public static uint DecodeID(ushort r, ushort g) => (uint)(r | (g << 16));
    }
}