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
    /// Represents a single bone of a mesh. A bone has a name which allows it to be found in the frame
    /// hierarchy and by which it can be addressed by animations. In addition it has a number of
    /// influences on vertices and a matrix relating the mesh position to the position of the bone at the time of binding.
    /// </summary>
    public sealed class Bone : IMarshalable<Bone, AiBone>
    {
        private string m_name;
        private List<VertexWeight> m_weights;
        private Matrix4x4 m_offsetMatrix;

        /// <summary>
        /// Gets or sets the name of the bone.
        /// </summary>
        public string Name
        {
            get => m_name;
            set => m_name = value;
        }

        /// <summary>
        /// Gets the number of vertex influences the bone contains.
        /// </summary>
        public int VertexWeightCount => m_weights.Count;

        /// <summary>
        /// Gets if the bone has vertex weights - this should always be true.
        /// </summary>
        public bool HasVertexWeights => m_weights.Count > 0;

        /// <summary>
        /// Gets the influence weights of this bone, by vertex index.
        /// </summary>
        public List<VertexWeight> VertexWeights => m_weights;

        /// <summary>
        /// Gets or sets the matrix that transforms from bone space to mesh space in bind pose. This matrix describes the
        /// position of the mesh in the local space of this bone when the skeleton was bound. Thus it can be used directly to determine a desired vertex
        /// position, given the world-space transform of the bone when animated, and the position of the vertex in mesh space.
        /// 
        /// It is sometimes called an inverse-bind matrix or inverse-bind pose matrix.
        /// </summary>
        public Matrix4x4 OffsetMatrix
        {
            get => m_offsetMatrix;
            set => m_offsetMatrix = value;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Bone"/> class.
        /// </summary>
        public Bone()
        {
            m_name = string.Empty;
            m_offsetMatrix = Matrix4x4.Identity;
            m_weights = [];
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="Bone"/> class.
        /// </summary>
        /// <param name="name">Name of the bone</param>
        /// <param name="offsetMatrix">Bone's offset matrix</param>
        /// <param name="weights">Vertex weights</param>
        public Bone(string name, Matrix3x3 offsetMatrix, VertexWeight[] weights)
        {
            m_name = name;
            m_offsetMatrix = offsetMatrix;
            m_weights = [];
            m_weights.AddRange(weights);
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<Bone, AiBone>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<Bone, AiBone>.ToNative(IntPtr thisPtr, out AiBone nativeValue)
        {
            nativeValue.Name = new AiString(m_name);
            nativeValue.OffsetMatrix = m_offsetMatrix;
            nativeValue.NumWeights = (uint)m_weights.Count;
            nativeValue.Armature = IntPtr.Zero;
            nativeValue.Node = IntPtr.Zero;
            nativeValue.Weights = IntPtr.Zero;

            if (nativeValue.NumWeights > 0)
                nativeValue.Weights = MemoryHelper.ToNativeArray<VertexWeight>(m_weights);
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<Bone, AiBone>.FromNative(in AiBone nativeValue)
        {
            m_name = AiString.GetString(nativeValue.Name); //Avoid struct copy
            m_offsetMatrix = nativeValue.OffsetMatrix;
            m_weights.Clear();

            if (nativeValue.NumWeights > 0 && nativeValue.Weights != IntPtr.Zero)
                m_weights.AddRange(MemoryHelper.FromNativeArray<VertexWeight>(nativeValue.Weights, (int)nativeValue.NumWeights));
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{Bone, AiBone}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if (nativeValue == IntPtr.Zero)
                return;

            AiBone aiBone = MemoryHelper.Read<AiBone>(nativeValue);
            int numWeights = MemoryHelper.Read<int>(nativeValue + MemoryHelper.SizeOf<AiString>());
            IntPtr weightsPtr = nativeValue + MemoryHelper.SizeOf<AiString>() + sizeof(uint);

            if (aiBone.NumWeights > 0 && aiBone.Weights != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiBone.Weights);

            if (freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
