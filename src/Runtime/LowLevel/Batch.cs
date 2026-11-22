using UnityEngine;

namespace EniGUI.Internal
{
    public readonly struct Batch
    {
        public readonly Material Material;
        public readonly int Offset;
        public readonly int Count;

        public Batch(Material material, int offset, int count)
        {
            Material = material;
            Offset = offset;
            Count = count;
        }
    }
}