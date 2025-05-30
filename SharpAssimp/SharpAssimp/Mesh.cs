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
    /// A mesh represents geometry with a single material.
    /// </summary>
    public sealed class Mesh : IMarshalable<Mesh, AiMesh>
    {
        private string m_name;
        private PrimitiveType m_primitiveType;
        private int m_materialIndex;
        private readonly List<Vector3> m_vertices;
        private readonly List<Vector3> m_normals;
        private readonly List<Vector3> m_tangents;
        private readonly List<Vector3> m_bitangents;
        private readonly List<Face> m_faces;
        private readonly List<Vector4>[] m_colors;
        private readonly List<Vector3>[] m_texCoords;
        private readonly int[] m_texComponentCount;
        private readonly List<Bone> m_bones;
        private readonly List<MeshAnimationAttachment> m_meshAttachments;
        private MeshMorphingMethod m_morphMethod;
        private BoundingBox m_boundingBox;

        /// <summary>
        /// Gets or sets the mesh name. This tends to be used
        /// when formats name nodes and meshes independently,
        /// vertex animations refer to meshes by their names,
        /// or importers split meshes up, each mesh will reference
        /// the same (dummy) name.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        /// Gets or sets the primitive type. This may contain more than one
        /// type unless if <see cref="PostProcessSteps.SortByPrimitiveType"/>
        /// option is not set.
        /// </summary>
        public PrimitiveType PrimitiveType
        {
            get => m_primitiveType;
            set => m_primitiveType = value;
        }

        /// <summary>
        /// Gets or sets the index of the material associated with this mesh.
        /// </summary>
        public int MaterialIndex
        {
            get => m_materialIndex;
            set => m_materialIndex = value;
        }

        /// <summary>
        /// Gets the number of vertices in this mesh. This is the count that all
        /// per-vertex lists should be the size of.
        /// </summary>
        public int VertexCount => m_vertices.Count;

        /// <summary>
        /// Gets if the mesh has a vertex array. This should always return
        /// true provided no special scene flags are set.
        /// </summary>
        public bool HasVertices => m_vertices.Count > 0;

        /// <summary>
        /// Gets the vertex position list.
        /// </summary>
        public List<Vector3> Vertices => m_vertices;

        /// <summary>
        /// Gets if the mesh as normals. If it does exist, the count should be the same as the vertex count.
        /// </summary>
        public bool HasNormals => m_normals.Count > 0;

        /// <summary>
        /// Gets the vertex normal list.
        /// </summary>
        public List<Vector3> Normals => m_normals;

        /// <summary>
        /// Gets if the mesh has tangents and bitangents. It is not
        /// possible for one to be without the other. If it does exist, the count should be the same as the vertex count.
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
        /// Gets the number of faces contained in the mesh.
        /// </summary>
        public int FaceCount => m_faces.Count;

        /// <summary>
        /// Gets if the mesh contains faces. If no special
        /// scene flags are set, this should always return true.
        /// </summary>
        public bool HasFaces => m_faces.Count > 0;

        /// <summary>
        /// Gets the mesh's faces. Each face will contain indices
        /// to the vertices.
        /// </summary>
        public List<Face> Faces => m_faces;

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
        /// Gets the array that contains each vertex color channels, by default all are lists of zero (but can be set to null). Each index
        /// in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum vertex color channel limit.
        /// </summary>
        public List<Vector4>[] VertexColorChannels => m_colors;

        /// <summary>
        /// Gets the array that contains each texture coordinate channel, by default all are lists of zero (but can be set to null). Each index
        /// in the array corresponds to the texture coordinate channel. The length of the array corresponds to Assimp's maximum UV channel limit.
        /// </summary>
        public List<Vector3>[] TextureCoordinateChannels => m_texCoords;

        /// <summary>
        /// Gets the array that contains the count of UV(W) components for each texture coordinate channel, usually 2 (UV) or 3 (UVW). A component
        /// value of zero means the texture coordinate channel does not exist. The channel index (index in the array) corresponds
        /// to the texture coordinate channel index.
        /// </summary>
        public int[] UVComponentCount => m_texComponentCount;

        /// <summary>
        /// Gets the number of bones that influence this mesh.
        /// </summary>
        public int BoneCount => m_bones.Count;

        /// <summary>
        /// Gets if this mesh has bones.
        /// </summary>
        public bool HasBones => m_bones.Count > 0;

        /// <summary>
        /// Gets the bones that influence this mesh.
        /// </summary>
        public List<Bone> Bones => m_bones;

        /// <summary>
        /// Gets the number of mesh animation attachments that influence this mesh.
        /// </summary>
        public int MeshAnimationAttachmentCount => m_meshAttachments.Count;

        /// <summary>
        /// Gets if this mesh has mesh animation attachments.
        /// </summary>
        public bool HasMeshAnimationAttachments => m_meshAttachments.Count > 0;

        /// <summary>
        /// Gets the mesh animation attachments that influence this mesh.
        /// </summary>
        public List<MeshAnimationAttachment> MeshAnimationAttachments => m_meshAttachments;

        /// <summary>
        /// Gets or sets the morph method used when animation attachments are used.
        /// </summary>
        public MeshMorphingMethod MorphMethod
        {
            get => m_morphMethod;
            set => m_morphMethod = value;
        }

        /// <summary>
        /// Gets or sets the axis aligned bounding box that contains the extents of the mesh.
        /// </summary>
        public BoundingBox BoundingBox
        {
            get => m_boundingBox;
            set => m_boundingBox = value;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        public Mesh() : this(string.Empty, PrimitiveType.Triangle) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="name">Name of the mesh.</param>
        public Mesh(string name) : this(name, PrimitiveType.Triangle) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="primType">Primitive types contained in the mesh.</param>
        public Mesh(PrimitiveType primType) : this(string.Empty, primType) { }

        /// <summary>
        /// Constructs a new instance of the <see cref="Mesh"/> class.
        /// </summary>
        /// <param name="name">Name of the mesh</param>
        /// <param name="primType">Primitive types contained in the mesh.</param>
        public Mesh(string name, PrimitiveType primType)
        {
            m_name = name;
            m_primitiveType = primType;
            m_materialIndex = 0;
            m_morphMethod = MeshMorphingMethod.Unknown;

            m_vertices = [];
            m_normals = [];
            m_tangents = [];
            m_bitangents = [];
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

            m_texComponentCount = new int[AiDefines.AI_MAX_NUMBER_OF_TEXTURECOORDS];
            m_bones = [];
            m_faces = [];
            m_meshAttachments = [];
        }

        /// <summary>
        /// Checks if the mesh has vertex colors for the specified channel. This returns false if the list
        /// is null or empty. The channel, if it exists, should contain the same number of entries as <see cref="VertexCount"/>.
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
        /// Checks if the mesh has texture coordinates for the specified channel. This returns false if the list
        /// is null or empty. The channel, if it exists, should contain the same number of entries as <see cref="VertexCount"/>.
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

        /// <summary>
        /// Convienence method for setting this meshe's face list from an index buffer.
        /// </summary>
        /// <param name="indices">Index buffer</param>
        /// <param name="indicesPerFace">Indices per face</param>
        /// <returns>True if the operation succeeded, false otherwise (e.g. not enough data)</returns>
        public bool SetIndices(int[] indices, int indicesPerFace)
        {
            if (indices == null || indices.Length == 0 || ((indices.Length % indicesPerFace) != 0))
                return false;

            m_faces.Clear();

            int numFaces = indices.Length / indicesPerFace;
            int index = 0;

            for (int i = 0; i < numFaces; i++)
            {
                Face face = new();
                for (int j = 0; j < indicesPerFace; j++)
                {
                    face.Indices.Add(indices[index]);
                    index++;
                }
                m_faces.Add(face);
            }

            return true;
        }

        /// <summary>
        /// Convenience method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>int index array</returns>
        public IEnumerable<int> GetIndices()
        {
            if (!HasFaces)
                return [];

            return m_faces
                .Where(x => x.IndexCount > 0 && x.Indices != null)
                .SelectMany(x => x.Indices);
        }

        /// <summary>
        /// Convenience method for accumulating all face indices into a single index
        /// array as unsigned integers (the default from Assimp, if you need them).
        /// </summary>
        /// <returns>uint index enumerable</returns>
        public IEnumerable<uint> GetUnsignedIndices()
        {
            if (!HasFaces)
                return [];

            return m_faces
                .Where(x => x.IndexCount > 0 && x.Indices != null)
                .SelectMany(x => x.Indices)
                .Select(x => (uint)x);
        }

        /// <summary>
        /// Convenience method for accumulating all face indices into a single
        /// index array.
        /// </summary>
        /// <returns>short index enumerable</returns>
        public IEnumerable<short> GetShortIndices()
        {
            if (!HasFaces)
                return [];

            return m_faces
                .Where(x => x.IndexCount > 0 && x.Indices != null)
                .SelectMany(x => x.Indices)
                .Select(x => (short)x);
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

            for (int i = 0; i < m_texComponentCount.Length; i++)
            {
                m_texComponentCount[i] = 0;
            }

            m_bones.Clear();
            m_faces.Clear();
            m_meshAttachments.Clear();
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
        bool IMarshalable<Mesh, AiMesh>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Mesh, AiMesh>.ToNative(IntPtr thisPtr, out AiMesh nativeValue)
        {
            nativeValue.Name = new AiString(m_name);
            nativeValue.Vertices = IntPtr.Zero;
            nativeValue.Normals = IntPtr.Zero;
            nativeValue.Tangents = IntPtr.Zero;
            nativeValue.BiTangents = IntPtr.Zero;
            nativeValue.AnimMeshes = IntPtr.Zero;
            nativeValue.Bones = IntPtr.Zero;
            nativeValue.Faces = IntPtr.Zero;
            nativeValue.Colors = new AiMeshColorArray();
            nativeValue.TextureCoords = new AiMeshTextureCoordinateArray();
            nativeValue.NumUVComponents = new AiMeshUVComponentArray();
            nativeValue.PrimitiveTypes = m_primitiveType;
            nativeValue.MaterialIndex = (uint)m_materialIndex;
            nativeValue.NumVertices = (uint)VertexCount;
            nativeValue.NumBones = (uint)BoneCount;
            nativeValue.NumFaces = (uint)FaceCount;
            nativeValue.NumAnimMeshes = (uint)MeshAnimationAttachmentCount;
            nativeValue.MorphMethod = m_morphMethod;
            nativeValue.AABB = m_boundingBox;
            nativeValue.TextureCoordsNames = IntPtr.Zero;

            if (nativeValue.NumVertices > 0)
            {

                //Since we can have so many buffers of Vector3 with same length, lets re-use a buffer
                Vector3[] copy = new Vector3[nativeValue.NumVertices];

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

                //UV components for each tex coordinate channel
                for (int i = 0; i < m_texComponentCount.Length; i++)
                {
                    nativeValue.NumUVComponents[i] = (uint)m_texComponentCount[i];
                }
            }

            //Faces
            if (nativeValue.NumFaces > 0)
                nativeValue.Faces = MemoryHelper.ToNativeArray<Face, AiFace>(m_faces);

            //Bones
            if (nativeValue.NumBones > 0)
                nativeValue.Bones = MemoryHelper.ToNativeArray<Bone, AiBone>(m_bones, true);

            //Attachment meshes
            if (nativeValue.NumAnimMeshes > 0)
                nativeValue.AnimMeshes = MemoryHelper.ToNativeArray<MeshAnimationAttachment, AiAnimMesh>(m_meshAttachments);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Mesh, AiMesh>.FromNative(in AiMesh nativeValue)
        {
            ClearBuffers();

            int vertexCount = (int)nativeValue.NumVertices;
            m_name = AiString.GetString(nativeValue.Name); //Avoid struct copy
            m_materialIndex = (int)nativeValue.MaterialIndex;
            m_morphMethod = nativeValue.MorphMethod;
            m_boundingBox = nativeValue.AABB;
            m_primitiveType = nativeValue.PrimitiveTypes;

            //Load Per-vertex components
            if (vertexCount > 0)
            {

                //Positions
                if (nativeValue.Vertices != IntPtr.Zero)
                    m_vertices.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Vertices, vertexCount));

                //Normals
                if (nativeValue.Normals != IntPtr.Zero)
                    m_normals.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Normals, vertexCount));

                //Tangents
                if (nativeValue.Tangents != IntPtr.Zero)
                    m_tangents.AddRange(MemoryHelper.FromNativeArray<Vector3>(nativeValue.Tangents, vertexCount));

                //BiTangents
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

                //UV components for each tex coordinate channel
                for (int i = 0; i < nativeValue.NumUVComponents.Length; i++)
                {
                    m_texComponentCount[i] = (int)nativeValue.NumUVComponents[i];
                }
            }

            //Faces
            if (nativeValue.NumFaces > 0 && nativeValue.Faces != IntPtr.Zero)
                m_faces.AddRange(MemoryHelper.FromNativeArray<Face, AiFace>(nativeValue.Faces, (int)nativeValue.NumFaces));

            //Bones
            if (nativeValue.NumBones > 0 && nativeValue.Bones != IntPtr.Zero)
                m_bones.AddRange(MemoryHelper.FromNativeArray<Bone, AiBone>(nativeValue.Bones, (int)nativeValue.NumBones, true));

            //Attachment meshes
            if (nativeValue.NumAnimMeshes > 0 && nativeValue.AnimMeshes != IntPtr.Zero)
                m_meshAttachments.AddRange(MemoryHelper.FromNativeArray<MeshAnimationAttachment, AiAnimMesh>(nativeValue.AnimMeshes, (int)nativeValue.NumAnimMeshes, true));
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Mesh, AiMesh}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if (nativeValue == IntPtr.Zero)
                return;

            AiMesh aiMesh = MemoryHelper.Read<AiMesh>(nativeValue);

            if (aiMesh.NumVertices > 0)
            {
                if (aiMesh.Vertices != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Vertices);

                if (aiMesh.Normals != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Normals);

                if (aiMesh.Tangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.Tangents);

                if (aiMesh.BiTangents != IntPtr.Zero)
                    MemoryHelper.FreeMemory(aiMesh.BiTangents);

                //Vertex Color channels
                for (int i = 0; i < aiMesh.Colors.Length; i++)
                {
                    IntPtr colorPtr = aiMesh.Colors[i];

                    if (colorPtr != IntPtr.Zero)
                        MemoryHelper.FreeMemory(colorPtr);
                }

                //Texture coordinate channels
                for (int i = 0; i < aiMesh.TextureCoords.Length; i++)
                {
                    IntPtr texCoordsPtr = aiMesh.TextureCoords[i];

                    if (texCoordsPtr != IntPtr.Zero)
                        MemoryHelper.FreeMemory(texCoordsPtr);
                }
            }

            //Faces
            if (aiMesh.NumFaces > 0 && aiMesh.Faces != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiFace>(aiMesh.Faces, (int)aiMesh.NumFaces, Face.FreeNative);

            //Bones
            if (aiMesh.NumBones > 0 && aiMesh.Bones != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiBone>(aiMesh.Bones, (int)aiMesh.NumBones, Bone.FreeNative, true);

            //Attachment meshes
            if (aiMesh.NumAnimMeshes > 0 && aiMesh.AnimMeshes != IntPtr.Zero)
                MemoryHelper.FreeNativeArray<AiAnimMesh>(aiMesh.AnimMeshes, (int)aiMesh.NumAnimMeshes, MeshAnimationAttachment.FreeNative, true);

            if (freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
