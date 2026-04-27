using System;
using UnityEngine;

namespace EniGUI.LowLevel
{
    public readonly struct GUITexture
    {
        public readonly uint ID;
        public readonly Texture Texture;

        public GUITexture(uint id, Texture texture)
        {
            ID = id;
            Texture = texture;
        }
    }

    internal sealed class TextureSet
    {
        private const int BIT_MASK_LAYERS_COUNT = 16;
        private const int TEXTURE_SLOT_COUNT = 8;
        private const int MASK_BIT_COUNT = 8;

        private uint[] m_MBM = new uint[BIT_MASK_LAYERS_COUNT];
        private byte[] m_SlotOf = new byte[BIT_MASK_LAYERS_COUNT * MASK_BIT_COUNT];
        private Texture[] m_Textures = new Texture[TEXTURE_SLOT_COUNT];

        public byte Current { get; private set; }
        public int Count { get; private set; }

        public ReadOnlySpan<Texture> Textures => m_Textures.AsSpan(0, Count);

        public bool TryBind(GUITexture texture)
        {
            if (Count >= TEXTURE_SLOT_COUNT)
                return false;

            uint id = texture.ID;

            int layer = (int)(id >> 28);
            int bitIndex = (int)(id & 0x0FFFFFFF);

            uint bitMask = 1u << bitIndex;
            int index = layer * MASK_BIT_COUNT + bitIndex;

            if ((m_MBM[layer] & bitMask) == 0)
            {
                Count++;
                Current = (byte)(Count - 1);
                m_MBM[layer] |= bitMask;
                m_SlotOf[index] = Current;
                m_Textures[Current] = texture.Texture;
            }
            else
            {
                Current = m_SlotOf[index];
            }

            return true;
        }

        public void Unbind()
        {
            Current = 8;
        }

        public void Clear()
        {
            Current = 0;
            Count = 0;

            unsafe
            {
                fixed (uint* p = m_MBM)
                {
                    for (int i = 0; i < m_MBM.Length; i++)
                        p[i] = 0;
                }
            }
        }
    }
}