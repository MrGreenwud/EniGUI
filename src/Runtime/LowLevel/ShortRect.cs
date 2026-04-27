using System;

namespace EniGUI.LowLevel
{
    public readonly struct ShortRect
    {
        public readonly short X; 
        public readonly short Y;
        public readonly short Width;
        public readonly short Height;

        public readonly short MinX => X;
        public readonly short MinY => Y;
        public readonly short MaxX => (short)Math.Clamp(X + Width, short.MinValue, short.MaxValue);
        public readonly short MaxY => (short)Math.Clamp(Y + Height, short.MinValue, short.MaxValue);

        public ShortRect(short x, short y, short width, short height)
        {
            X = x;
            Y = y; 
            Width = width; 
            Height = height;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y, Width, Height);
        }
    }
}
