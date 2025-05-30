﻿/*
* Copyright (c) 2012-2020 AssimpNet - Nicholas Woodfield
* 
* Permission is hereby granted, free of charge, to any person obtaining a copy
* of this software and associated documentation files (the "Software"), to deal
* in the Software without restriction, including without limitation the rights
* to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
* copies of the Software, and to permit persons to whom the Software is
* furnished to do so, subject to the following conditions:
* 
* The above copyright notice and this permission notice shall be included in
* all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
* IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
* AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
* OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
* THE SOFTWARE.
*/

using SharpAssimp.Unmanaged;
using System.Numerics;

namespace SharpAssimp
{
    /// <summary>
    /// A mesh attachment store per-vertex animations for a particular frame. You may
    /// think of this as a 'patch' for the host mesh, since the mesh attachment replaces only certain
    /// vertex data streams at a particular time. Each mesh stores 'n' attached meshes. The actual
    /// relationship between the time line and mesh attachments is established by the mesh animation channel,
    /// which references singular mesh attachments by their ID and binds them to a time offset.
    /// </summary>
    public sealed class MeshAnimationAttachment : IMarshalable<MeshAnimationAttachment, AiAnimMesh>
    {
        private string m_name = string.Empty;
        private readonly List<Vector3> m_vertices;
        private readonly List<Vector3> m_normals;
        private readonly List<Vector3> m_tangents;
        private readonly List<Vector3> m_bitangents;
        private readonly List<Vector4>[] m_colors;
        private readonly List<Vector3>[] m_texCoords;
        private float m_weight;

        /// <summary>
        /// Gets or sets the mesh animation name.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        /// Gets the number of vertices in this mesh. This is a replacement
        /// for the host mesh's vertex count. Likewise, a mesh attachment
        /// cannot add or remove per-vertex attributes, therefore the existance
        /// of vertex data will match the existance of data in the mesh.
        /// </summary>
        public int VertexCount => m_vertices.Count;

        /// <summary>
        /// Checks whether the attachment mesh overrides the vertex positions
        /// of its host mesh.
        /// </summary>
        public bool HasVertices => m_vertices.Count > 0;

        /// <summary>
        /// Gets the vertex position list.
        /// </summary>
        public List<Vector3> Vertices => m_vertices;

        /// <summary>
        /// Checks whether the attachment mesh overrides the vertex normals of
        /// its host mesh.
        /// </summary>
        public bool HasNormals => m_normals.Count > 0;

        /// <summary>
        /// Gets the vertex normal list.
        /// </summary>
        public List<Vector3> Normals => m_normals;

        /// <summary>
        /// Checks whether the attachment mesh overrides the vertex
        /// tangents and bitangents of its host mesh.
        /// </summary>
        public bool HasTangentBasis => m_tangents.Count > 0 && m_bitangents.Count > 0;

        /// <summary>
        /// Gets the vertex tangent list.
        /// </summary>
        public List<Vector3> Tangents => m_tangents;

        /// <summary>
        /// Gets the vertex bitangent list.
        /// </summary>
        public List<Vector3> BiTangents => m_bitangents;

        /// <summary>
        /// Gets the number of valid vertex color channels contained in the
        /// mesh (list is not empty/not null). This can be a value between zero and the maximum vertex color count. Each individual channel
        /// should be the size of <see cref="VertexCount"/>.
        /// </summary>
        public int VertexColorChannelCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < m_colors.Length; i++)
                {
                    if (HasVertexColors(i))
                        count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the number of valid texture coordinate channels contained
        /// in the mesh (list is not empty/not null). This can be a value between zero and the maximum texture coordinate count.
        /// Each individual channel should be the size of <see cref="VertexCount"/>.
        /// </summary>
        public int TextureCoordinateChannelCount
        {
            get
            {
                int count = 0;
                for (int i = 0; i < m_texCoords.Length; i++)
                {
                    if (HasTextureCoords(i))
                        count++;
                }

                return count;
            }
        }

        /// <summary>
        /// Gets the array that contains each vertex color channels that override a specific channel in the host mesh, by default all are lists of zero (but can be set to null). 
        /// Each index in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum vertex color channel limit.
        /// </summary>
        public List<Vector4>[] VertexColorChannels => m_colors;

        /// <summary>
        /// Gets the array that contains each texture coordinate channel that override a specific channel in the host mesh, by default all are lists of zero (but can be set to null).
        /// Each index in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum UV channel limit.
        /// </summary>
        public List<Vector3>[] TextureCoordinateChannels => m_texCoords;

        /// <summary>
        /// Gets or sets the weight of the mesh animation.
        /// </summary>
        public float Weight
        {
            get => m_weight;
            set => m_weight = value;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MeshAnimationAttachment"/> class.
        /// </summary>
        public MeshAnimationAttachment()
        {
            m_vertices = [];
            m_normals = [];
            m_tangents = [];
            m_bitangents = [];
            m_weight = 0.0f;

            m_colors = new List<Vector4>[AiDefines.AI_MAX_NUMBER_OF_COLOR_SETS];

            for (int i = 0; i < m_colors.Length; i++)
            {
                m_colors[i] = [];
            }

            m_texCoords = new List<Vector3>[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];

            for (int i = 0; i < m_texCoords.Length; i++)
            {
                m_texCoords[i] = [];
            }
        }

        /// <summary>
        /// Checks if the mesh attachment overrides a particular set of vertex colors on
        /// the host mesh. This returns false if the list is null or empty. The index is between 
        /// zero and the maximumb number of vertex color channels.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if vertex colors are present in the channel.</returns>
        public bool HasVertexColors(int channelIndex)
        {
            if (channelIndex >= m_colors.Length || channelIndex < 0)
                return false;

            List<Vector4> colors = m_colors[channelIndex];

            if (colors != null)
                return colors.Count > 0;

            return false;
        }

        /// <summary>
        /// Checks if the mesh attachment overrides a particular set of texture coordinates on
        /// the host mesh. This returns false if the list is null or empty. The index is 
        /// between zero and the maximum number of texture coordinate channels.
        /// </summary>
        /// <param name="channelIndex">Channel index</param>
        /// <returns>True if texture coordinates are present in the channel.</returns>
        public bool HasTextureCoords(int channelIndex)
        {
            if (channelIndex >= m_texCoords.Length || channelIndex < 0)
                return false;

            List<Vector3> texCoords = m_texCoords[channelIndex];

            if (texCoords != null)
                return texCoords.Count > 0;

            return false;
        }

        private void ClearBuffers()
        {
            m_vertices.Clear();
            m_normals.Clear();
            m_tangents.Clear();
            m_bitangents.Clear();

            for (int i = 0; i < m_colors.Length; i++)
            {
                List<Vector4>? colors = m_colors[i];

                if (colors == null)
                    m_colors[i] = [];
                else
                    colors.Clear();
            }

            for (int i = 0; i < m_texCoords.Length; i++)
            {
                List<Vector3>? texCoords = m_texCoords[i];

                if (texCoords == null)
                    m_texCoords[i] = [];
                else
                    texCoords.Clear();
            }
        }

        private static Vector3[] CopyTo(List<Vector3> list, Vector3[] copy)
        {
            list.CopyTo(copy);

            return copy;
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<MeshAnimationAttachment, AiAnimMesh>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<MeshAnimationAttachment, AiAnimMesh>.ToNative(IntPtr thisPtr, out AiAnimMesh nativeValue)
        {
            nativeValue.Name = new AiString(m_name);
            nativeValue.Vertices = IntPtr.Zero;
            nativeValue.Normals = IntPtr.Zero;
            nativeValue.Tangents = IntPtr.Zero;
            nativeValue.BiTangents = IntPtr.Zero;
            nativeValue.Colors = new AiMeshColorArray();
            nativeValue.TextureCoords = new AiMeshTextureCoordinateArray();
            nativeValue.NumVertices = (uint)VertexCount;
            nativeValue.Weight = m_weight;

            if (VertexCount > 0)
            {

                //Since we can have so many buffers of Vector3 with same length, lets re-use a buffer
                Vector3[] copy = new Vector3[VertexCount];

                nativeValue.Vertices = MemoryHelper.ToNativeArray<Vector3>(CopyTo(m_vertices, copy));

                if (HasNormals)
                    nativeValue.Normals = MemoryHelper.ToNativeArray<Vector3>(CopyTo(m_normals, copy));

                if (HasTangentBasis)
                {
                    nativeValue.Tangents = MemoryHelper.ToNativeArray<Vector3>(CopyTo(m_tangents, copy));
                    nativeValue.BiTangents = MemoryHelper.ToNativeArray<Vector3>(CopyTo(m_bitangents, copy));
                }

                //Vertex Color channels
                for (int i = 0; i < m_colors.Length; i++)
                {
                    List<Vector4> list = m_colors[i];

                    if (list == null || list.Count == 0)
                    {
                        nativeValue.Colors[i] = IntPtr.Zero;
                    }
                    else
                    {
                        nativeValue.Colors[i] = MemoryHelper.ToNativeArray<Vector4>(list);
                    }
                }

                //Texture coordinate channels
                for (int i = 0; i < m_texCoords.Length; i++)
                {
                    List<Vector3> list = m_texCoords[i];

                    if (list == null || list.Count == 0)
                    {
                        nativeValue.TextureCoords[i] = IntPtr.Zero;
                    }
                    else
                    {
                        nativeValue.TextureCoords[i] = MemoryHelper.ToNativeArray<Vector3>(CopyTo(list, copy));
                    }
                }
            }
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<MeshAnimationAttachment, AiAnimMesh>.FromNative(in AiAnimMesh nativeValue)
        {
            ClearBuffers();

            m_name = AiString.GetString(nativeValue.Name); //Avoid struct copy

            int vertexCount = (int)nativeValue.NumVertices;
            m_weight = nativeValue.Weight;

            if (vertexCount > 0)
            {

                if (nativeValue.Vertices != IntPtr.Zero)
                    m_vertices.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Vertices, vertexCount));

                if (nativeValue.Normals != IntPtr.Zero)
                    m_normals.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Normals, vertexCount));

                if (nativeValue.Tangents != IntPtr.Zero)
                    m_tangents.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Tangents, vertexCount));

                if (nativeValue.BiTangents != IntPtr.Zero)
                    m_bitangents.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.BiTangents, vertexCount));

                //Vertex Color channels
                for (int i = 0; i < nativeValue.Colors.Length; i++)
                {
                    IntPtr colorPtr = nativeValue.Colors[i];

                    if (colorPtr != IntPtr.Zero)
                        m_colors[i].AddRange(MemoryHelper.FromNativeArray<Vector4>(colorPtr, vertexCount));
                }

                //Texture coordinate channels
                for (int i = 0; i < nativeValue.TextureCoords.Length; i++)
                {
                    IntPtr texCoordsPtr = nativeValue.TextureCoords[i];

                    if (texCoordsPtr != IntPtr.Zero)
                        m_texCoords[i].AddRange(MemoryHelper.FromNativeArray<Vector3>(texCoordsPtr, vertexCount));
                }
            }
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{MeshAnimationAttachment, AiAnimMesh}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if (nativeValue == IntPtr.Zero)
                return;

            AiAnimMesh aiAnimMesh = MemoryHelper.Read<AiAnimMesh>(nativeValue);

            if (aiAnimMesh.NumVertices > 0)
            {
                if (aiAnimMesh.Vertices != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiAnimMesh.Vertices);

                if (aiAnimMesh.Normals != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiAnimMesh.Normals);

                if (aiAnimMesh.Tangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiAnimMesh.Tangents);

                if (aiAnimMesh.BiTangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiAnimMesh.BiTangents);

                //Vertex Color channels
                for (int i = 0; i < aiAnimMesh.Colors.Length; i++)
                {
                    IntPtr colorPtr = aiAnimMesh.Colors[i];

                    if (colorPtr != IntPtr.Zero)
                        MemoryHelper.FreeMemory(colorPtr);
                }

                //Texture coordinate channels
                for (int i = 0; i < aiAnimMesh.TextureCoords.Length; i++)
                {
                    IntPtr texCoordsPtr = aiAnimMesh.TextureCoords[i];

                    if (texCoordsPtr != IntPtr.Zero)
                        MemoryHelper.FreeMemory(texCoordsPtr);
                }
            }

            if (freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
