namespace EniGUI.LowLevel
{
    internal struct UIElement
    {
        public uint PosMin; //16 bit xMin, 16 bit yMin.
        public uint PosMax; //16 bit xMax, 16 bit yMax.

        public uint UVMin; //16 bit xMin, 16 bit yMin.
        public uint UVMax; //16 bit xMax, 16 bit yMax.

        public uint Meta0; //4 bit type, 8 bit effects (pipeline ID), 20 bit id.
        public uint Meta1; //13 bit clips, 3 bit texture/font, 16 bit color (r5g6b5).
        public uint Meta2; //4 bit shadow intencity (1 ~ 6.25%), 8 bit shadow direction, 8 bit shadow color (r3g2b3), 12 bit tangent or radius.
        public uint Meta3; //8 bit thikness (font, line, bezer), 8 outline thikness, 16 bit outline color (r5g6b5).
    }
}
