namespace EniGUI.LowLevel
{
    public struct UIElement
    {
        public uint PosMin; //16 bit xMin, 16 bit yMin.
        public uint PosMax; //16 bit xMax, 16 bit yMax.

        public uint UVMin; //16 bit xMin, 16 bit yMin.
        public uint UVMax; //16 bit xMax, 16 bit yMax.

        public uint Meta0; //4 bit type, 8 bit effects (effects mask), 20 bit id.
        public uint Meta1; //8 bit clips, 4 bit texture/font, 20 bit color (r5g6b5à4).
        public uint Meta2; //32 bit tangent or radius or nine-slice data (8 bit per border)
        public uint Meta3; //4 bit shadow intencity (1 ~ 6.25%), 8 bit shadow direction, 20 bit shadow color (r7g7b6)
        public uint Meta4; //8 bit thikness (font, line, bezer), 8 outline thikness, 16 bit outline color (r5g6b5).
    }
}