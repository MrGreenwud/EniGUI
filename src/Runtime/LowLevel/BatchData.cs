using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using UnityEngine.Rendering;

namespace EniGUI.Renderer.LowLevel
{
    internal sealed class BatchData : IDisposable
    {
        private static readonly Vector2[] s_ZeroUVs = new Vector2[4];

        public readonly Mesh Mesh;

        private NativeArray<Vector3> m_Vertexes;
        private NativeArray<int> m_Indexes;
        private NativeArray<Vector2> m_UVs;
        private NativeArray<Color32> m_Color;

        private int m_VertexCount = 0;
        private int IndexCount => m_VertexCount * 3 / 2;

        public Material Material { get; private set; }

        public BatchData(int maxCount = 10000)
        {
            m_Vertexes = new NativeArray<Vector3>(maxCount, Allocator.Persistent);
            m_Indexes = new NativeArray<int>(maxCount * 6 / 4, Allocator.Persistent);
            m_UVs = new NativeArray<Vector2>(maxCount, Allocator.Persistent);
            m_Color = new NativeArray<Color32>(maxCount, Allocator.Persistent);

            Mesh = new Mesh();
            Mesh.MarkDynamic();
            Mesh.hideFlags = HideFlags.DontSave;

            Mesh.SetVertexBufferParams(maxCount,
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
                new VertexAttributeDescriptor(VertexAttribute.Color, VertexAttributeFormat.UNorm8, 4));

            Mesh.SetIndexBufferParams(maxCount * 6 / 4, IndexFormat.UInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(Rect rect, uint id, float depth, ReadOnlySpan<Vector2> uvs = default)
        {
            int baseVertexCount = m_VertexCount;

            m_Vertexes[baseVertexCount + 0] = new Vector3(rect.xMin, rect.yMax, depth);
            m_Vertexes[baseVertexCount + 1] = new Vector3(rect.xMin, rect.yMin, depth);
            m_Vertexes[baseVertexCount + 2] = new Vector3(rect.xMax, rect.yMax, depth);
            m_Vertexes[baseVertexCount + 3] = new Vector3(rect.xMax, rect.yMin, depth);

            int baseIndexCount = IndexCount;

            m_Indexes[baseIndexCount + 0] = baseVertexCount + 0;
            m_Indexes[baseIndexCount + 1] = baseVertexCount + 1;
            m_Indexes[baseIndexCount + 2] = baseVertexCount + 2;
            m_Indexes[baseIndexCount + 3] = baseVertexCount + 0;
            m_Indexes[baseIndexCount + 4] = baseVertexCount + 2;
            m_Indexes[baseIndexCount + 5] = baseVertexCount + 3;

            uvs = uvs == default ? s_ZeroUVs.AsSpan() : uvs;
            Color32 color = id == 0 ? Color.clear : EniGUIUtility.EncodeID(id);

            for (int i = 0; i < 4; i++)
            {
                int index = baseVertexCount + i;

                m_UVs[index] = uvs[i];
                m_Color[index] = color;
            }

            m_VertexCount += 4;
        }

        public void UpdateMesh()
        {
            Mesh.SetVertexBufferData(m_Vertexes, 0, 0, m_VertexCount);
            Mesh.SetVertexBufferData(m_Color, 0, 0, m_VertexCount, 1);
            Mesh.SetVertexBufferData(m_UVs, 0, 0, m_VertexCount, 2);
            Mesh.SetIndexBufferData(m_Indexes, 0, 0, IndexCount);

            Mesh.subMeshCount = 1;
            Mesh.SetSubMesh(0, new SubMeshDescriptor(0, IndexCount, MeshTopology.Triangles));
        }

        public void Clear(Material material)
        {
            m_VertexCount = 0;
            Material = material;
        }

        public void Dispose()
        {
            UnityEngine.Object.DestroyImmediate(Mesh);

            m_Vertexes.Dispose();
            m_Indexes.Dispose();
            m_UVs.Dispose();
            m_Color.Dispose();
        }
    }
}
