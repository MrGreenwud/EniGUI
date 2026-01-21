using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;
using UnityEngine.Rendering;

namespace EniGUI.LowLevel
{
    internal struct Vertex
    {
        public float pX;
        public float pY;
        public float pZ;
        public float2 UV;
        public uint ID;
        public uint Clip;
    }

    internal sealed class BatchData : IDisposable
    {
        private const MeshUpdateFlags MESH_UPDATE_FLAGS =
            MeshUpdateFlags.DontRecalculateBounds
            | MeshUpdateFlags.DontResetBoneBounds
            | MeshUpdateFlags.DontValidateIndices
            | MeshUpdateFlags.DontNotifyMeshUsers;
        
        private static readonly float2[] s_ZeroUVs = new float2[4];

        public readonly Mesh Mesh;

        private NativeArray<Vertex> m_Vertexes;
        private NativeArray<int> m_Indexes;

        private int m_VertexCount = 0;

        public Material Material { get; private set; }
        public bool IsFull => m_VertexCount + 4 > m_Vertexes.Length;

        public BatchData(int maxVertexCount = 10000)
        {
            int maxIndexCount = maxVertexCount * 6 / 4;

            m_Vertexes = new NativeArray<Vertex>(maxVertexCount, Allocator.Persistent);
            m_Indexes = new NativeArray<int>(maxIndexCount, Allocator.Persistent);

            for (int q = 0; q < maxIndexCount / 6; q++)
            {
                int vi = q * 4;
                int ii = q * 6;

                m_Indexes[ii + 0] = vi + 0;
                m_Indexes[ii + 1] = vi + 1;
                m_Indexes[ii + 2] = vi + 2;

                m_Indexes[ii + 3] = vi + 0;
                m_Indexes[ii + 4] = vi + 2;
                m_Indexes[ii + 5] = vi + 3;
            }

            Mesh = new Mesh();
            Mesh.MarkDynamic();
            Mesh.hideFlags = HideFlags.DontSave;
            Mesh.bounds = new Bounds(Vector3.zero, Vector3.one * 100000);

            Mesh.SetVertexBufferParams(maxVertexCount,
                new VertexAttributeDescriptor(VertexAttribute.Position, VertexAttributeFormat.Float32, 3, 0),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord0, VertexAttributeFormat.Float32, 2),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord1, VertexAttributeFormat.UInt32, 1),
                new VertexAttributeDescriptor(VertexAttribute.TexCoord2, VertexAttributeFormat.UInt32, 1));

            Mesh.SetIndexBufferParams(maxIndexCount, IndexFormat.UInt32);

            Mesh.SetIndexBufferData(m_Indexes, 0, 0, maxIndexCount, MESH_UPDATE_FLAGS);

            Mesh.subMeshCount = 1;
            Mesh.SetSubMesh(0, new SubMeshDescriptor(0, maxIndexCount, MeshTopology.Triangles));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(Rect rect, uint id, ushort clip)
        {
            AddQuad(rect, id, clip, s_ZeroUVs);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AddQuad(Rect rect, uint id, ushort clip, ReadOnlySpan<float2> uvs = default)
        {
            unsafe
            {
                var baseVertexCount = m_VertexCount;

                var xMin = rect.xMin;
                var xMax = rect.xMax;
                var yMin = rect.yMin;
                var yMax = rect.yMax;

                var uv0 = uvs[0];
                var uv1 = uvs[1];
                var uv2 = uvs[2];
                var uv3 = uvs[3];

                Vertex* v = (Vertex*)m_Vertexes.GetUnsafePtr() + baseVertexCount;

                v[0].pX = xMin;
                v[0].pY = yMax;
                v[0].UV = uv0;
                v[0].ID = id;
                v[0].Clip = clip;

                v[1].pX = xMin;
                v[1].pY = yMin;
                v[1].UV = uv1;
                v[1].ID = id;
                v[1].Clip = clip;

                v[2].pX = xMax;
                v[2].pY = yMin;
                v[2].UV = uv2;
                v[2].ID = id;
                v[2].Clip = clip;

                v[3].pX = xMax;
                v[3].pY = yMax;
                v[3].UV = uv3;
                v[3].ID = id;
                v[3].Clip = clip;

                m_VertexCount += 4;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UpdateMesh()
        {
            Mesh.SetVertexBufferData(m_Vertexes, 0, 0, m_VertexCount, 0, MESH_UPDATE_FLAGS);
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
