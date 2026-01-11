using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace EniGUI.LowLevel
{
    internal readonly struct Vertex
    {
        public readonly float3 Position;
        public readonly float2 UV;
        public readonly uint ID;
        public readonly uint Clip;

        public Vertex(float3 position, float2 uv, uint id, ushort clip)
        {
            Position = position;
            UV = uv;
            ID = id;
            Clip = clip;
        }
    }

    internal sealed class BatchData : IDisposable
    {
        private static readonly float2[] s_ZeroUVs = new float2[4];

        public readonly Mesh Mesh;

        private NativeArray<Vertex> m_Vertexes;
        private NativeArray<int> m_Indexes;

        private int m_VertexCount = 0;
        private int IndexCount => m_VertexCount * 3 / 2;

        public Material Material { get; private set; }
        public bool IsFull => m_VertexCount >= m_Vertexes.Length;

        public BatchData(int maxCount = 10000)
        {
            m_Vertexes = new NativeArray<Vertex>(maxCount, Allocator.Persistent);
            m_Indexes = new NativeArray<int>(maxCount * 6 / 4, Allocator.Persistent);

            Mesh = new Mesh();
            Mesh.MarkDynamic();
            Mesh.hideFlags = HideFlags.DontSave;
            Mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000);

            Mesh.SetVertexBufferParams(maxCount,
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.UInt32, 1),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.UInt32, 1));

            Mesh.SetIndexBufferParams(maxCount * 6 / 4, IndexFormat.UInt32);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(Rect rect, float depth, uint id, ushort clip, ReadOnlySpan<float2> uvs = default)
        {
            int baseVertexCount = m_VertexCount;

            float3 position0 = new(rect.xMin, rect.yMax, depth);
            float3 position1 = new(rect.xMin, rect.yMin, depth);
            float3 position2 = new(rect.xMax, rect.yMin, depth);
            float3 position3 = new(rect.xMax, rect.yMax, depth);

            uvs = uvs == default ? s_ZeroUVs.AsSpan() : uvs;

            m_Vertexes[baseVertexCount + 0] = new(position0, uvs[0], id, clip);
            m_Vertexes[baseVertexCount + 1] = new(position1, uvs[1], id, clip);
            m_Vertexes[baseVertexCount + 2] = new(position2, uvs[2], id, clip);
            m_Vertexes[baseVertexCount + 3] = new(position3, uvs[3], id, clip);

            int baseIndexCount = IndexCount;

            m_Indexes[baseIndexCount + 0] = baseVertexCount + 0;
            m_Indexes[baseIndexCount + 1] = baseVertexCount + 1;
            m_Indexes[baseIndexCount + 2] = baseVertexCount + 2;
            m_Indexes[baseIndexCount + 3] = baseVertexCount + 0;
            m_Indexes[baseIndexCount + 4] = baseVertexCount + 2;
            m_Indexes[baseIndexCount + 5] = baseVertexCount + 3;

            m_VertexCount += 4;
        }

        public void UpdateMesh()
        {
            int indexCount = IndexCount;

            Mesh.SetVertexBufferData(m_Vertexes, 0, 0, m_VertexCount, 0);
            Mesh.SetIndexBufferData(m_Indexes, 0, 0, indexCount);

            Mesh.subMeshCount = 1;
            Mesh.SetSubMesh(0, new SubMeshDescriptor(0, indexCount, MeshTopology.Triangles));
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
        }
    }
}
