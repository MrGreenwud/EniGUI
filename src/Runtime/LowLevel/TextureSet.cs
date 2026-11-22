using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace EniGUI.LowLevel
{
    internal sealed class TextureSet
    {
        private const int TEXTURE_SLOT_COUNT = 8;
        private const int TEXTURE_EMPTY_SLOT_ID = 8;

        private ulong[] m_IDs = new ulong[TEXTURE_SLOT_COUNT];
        private Texture[] m_Textures = new Texture[TEXTURE_SLOT_COUNT];

        private ulong m_CurrentID;

        public byte CurrentSlot { get; private set; } = TEXTURE_EMPTY_SLOT_ID;
        public int Count { get; private set; }

        public ReadOnlySpan<Texture> Textures => m_Textures.AsSpan(0, Count);

        public bool TryBind(Texture texture)
        {
            var id = EntityId.ToULong(texture.GetEntityId());

            if (m_CurrentID == id || TrySelect(id))
                return true;

            if (Count >= TEXTURE_SLOT_COUNT)
                return false;

            Add(texture, id);
            return true;
        }

        public void Unbind()
        {
            CurrentSlot = TEXTURE_EMPTY_SLOT_ID;
            m_CurrentID = 0;
        }

        public void Clear()
        {
            Unbind();
            Count = 0;

            unsafe
            {
                fixed (ulong* p = m_IDs)
                {
                    for (int i = 0; i < m_IDs.Length; i++)
                        p[i] = 0;
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private unsafe bool TrySelect(ulong id)
        {
            fixed (ulong* IDs = m_IDs)
            {
                for (int i = 0; i < Count; i++)
                {
                    if (IDs[i] == id)
                    {
                        m_CurrentID = id;
                        CurrentSlot = (byte)i;
                        return true;
                    }
                }
            }

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void Add(Texture texture, ulong id)
        {
            m_IDs[Count] = id;
            m_Textures[Count] = texture;
            CurrentSlot = (byte)Count;
            Count++;
        }
    }
}