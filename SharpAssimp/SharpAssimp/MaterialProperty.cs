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
using System.Diagnostics;
using System.Numerics;
using System.Text;

namespace SharpAssimp
{
    /// <summary>
    /// A key-value pairing that represents some material property.
    /// </summary>
    public sealed class MaterialProperty : IMarshalable<MaterialProperty, AiMaterialProperty>
    {
        private string m_name;
        private PropertyType m_type;
        private byte[]? m_rawValue;
        private TextureType m_texType;
        private int m_texIndex;
        private string m_fullyQualifiedName = string.Empty;
        private bool m_fullQualifiedNameNeedsUpdate = true;

        /// <summary>
        /// Gets or sets the property key name. E.g. $tex.file. This corresponds to the
        /// "AiMatKeys" base name constants.
        /// </summary>
        public string Name
        {
            get => m_name;
            set
            {
                m_name = value;
                m_fullQualifiedNameNeedsUpdate = true;

                AssertIsBaseName();
            }
        }

        /// <summary>
        /// Gets or sets the type of property.
        /// </summary>
        public PropertyType PropertyType
        {
            get => m_type;
            set => m_type = value;
        }

        /// <summary>
        /// Gets the raw byte data count.
        /// </summary>
        public int ByteCount => (m_rawValue == null) ? 0 : m_rawValue.Length;

        /// <summary>
        /// Checks if the property has data.
        /// </summary>
        public bool HasRawData => m_rawValue != null;

        /// <summary>
        /// Gets the raw byte data. To modify/read this data, see the Get/SetXXXValue methods.
        /// </summary>
        public byte[]? RawData => m_rawValue;

        /// <summary>
        /// Gets or sets the texture type semantic, for non-texture properties this is always <see cref="SharpAssimp.TextureType.None"/>.
        /// </summary>
        public TextureType TextureType
        {
            get => m_texType;
            set
            {
                m_texType = value;
                m_fullQualifiedNameNeedsUpdate = true;
            }
        }

        /// <summary>
        /// Gets or sets the texture index, for non-texture properties this is always zero.
        /// </summary>
        public int TextureIndex
        {
            get => m_texIndex;
            set
            {
                m_texIndex = value;
                m_fullQualifiedNameNeedsUpdate = true;
            }
        }

        /// <summary>
        /// Gets the property's fully qualified name. Format: "{base name},{texture type semantic},{texture index}". E.g. "$clr.diffuse,0,0". This
        /// is the key that is used to index the property in the material property map.
        /// </summary>
        public string FullyQualifiedName
        {
            get
            {
                if (m_fullQualifiedNameNeedsUpdate)
                {
                    m_fullyQualifiedName = Material.CreateFullyQualifiedName(m_name, m_texType, m_texIndex);
                    m_fullQualifiedNameNeedsUpdate = false;
                }

                return m_fullyQualifiedName;
            }
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class.
        /// </summary>
        public MaterialProperty()
        {
            m_name = string.Empty;
            m_type = PropertyType.Buffer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a buffer property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="buffer">Property value</param>
        public MaterialProperty(string baseName, byte[] buffer)
        {
            m_name = baseName;
            m_type = PropertyType.Buffer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = buffer;

            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a float property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, float value)
        {
            m_name = baseName;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetFloatValue(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs an integer property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, int value)
        {
            m_name = baseName;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetIntegerValue(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Constructs a boolean property.
        /// </summary>
        /// <param name="baseName">Name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, bool value)
        {
            m_name = baseName;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetBooleanValue(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a string property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, string value)
        {
            m_name = baseName;
            m_type = PropertyType.String;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetStringValue(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a texture property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        /// <param name="texType">Texture type</param>
        /// <param name="textureIndex">Texture index</param>
        public MaterialProperty(string baseName, string value, TextureType texType, int textureIndex)
        {
            m_name = baseName;
            m_type = PropertyType.String;
            m_texIndex = textureIndex;
            m_texType = texType;
            m_rawValue = null;

            SetStringValue(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a float array property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="values">Property values</param>
        public MaterialProperty(string baseName, float[] values)
        {
            m_name = baseName;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetFloatArrayValue(values);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a int array property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="values">Property values</param>
        public MaterialProperty(string baseName, int[] values)
        {
            m_name = baseName;
            m_type = PropertyType.Integer;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetIntegerArrayValue(values);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a Vector3 property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, Vector3 value)
        {
            m_name = baseName;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetVector3Value(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Constructs a new instance of the <see cref="MaterialProperty"/> class. Creates a Vector4 property.
        /// </summary>
        /// <param name="baseName">Base name of the property</param>
        /// <param name="value">Property value</param>
        public MaterialProperty(string baseName, Vector4 value)
        {
            m_name = baseName;
            m_type = PropertyType.Float;
            m_texIndex = 0;
            m_texType = TextureType.None;
            m_rawValue = null;

            SetVector4Value(value);
            AssertIsBaseName();
        }

        /// <summary>
        /// Gets the property raw data as a float.
        /// </summary>
        /// <returns>Float</returns>
        public float GetFloatValue()
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueAs<float>();

            return 0;
        }

        /// <summary>
        /// Sets the property raw data with a float.
        /// </summary>
        /// <param name="value">Float.</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetFloatValue(float value)
        {
            if (m_type != PropertyType.Float && m_type != PropertyType.Integer)
                return false;

            return SetValueAs<float>(value);
        }

        /// <summary>
        /// Gets the property raw data as a double.
        /// </summary>
        /// <returns>Double</returns>
        public double GetDoubleValue()
        {
            if (m_type == PropertyType.Double)
                return GetValueAs<double>();

            return 0;
        }

        /// <summary>
        /// Sets the property raw data with a double.
        /// </summary>
        /// <param name="value">Double.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool SetDoubleValue(double value)
        {
            if (m_type != PropertyType.Double)
                return false;

            return SetValueAs<double>(value);
        }

        /// <summary>
        /// Gets the property raw data as an integer.
        /// </summary>
        /// <returns>Integer</returns>
        public int GetIntegerValue()
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueAs<int>();

            return 0;
        }

        /// <summary>
        /// Sets the property raw data as an integer.
        /// </summary>
        /// <param name="value">Integer</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetIntegerValue(int value)
        {
            if (m_type != PropertyType.Float && m_type != PropertyType.Integer)
                return false;

            return SetValueAs<int>(value);
        }

        /// <summary>
        /// Gets the property raw data as a string.
        /// </summary>
        /// <returns>String</returns>
        public string? GetStringValue()
        {
            if (m_type != PropertyType.String)
                return null;

            return GetMaterialString(m_rawValue);
        }

        /// <summary>
        /// Sets the property raw data as string.
        /// </summary>
        /// <param name="value">String</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetStringValue(string value)
        {
            if (m_type != PropertyType.String)
                return false;

            m_rawValue = SetMaterialString(value, m_rawValue);
            return true;
        }

        /// <summary>
        /// Gets the property raw data as a float array.
        /// </summary>
        /// <param name="count">Number of elements to get</param>
        /// <returns>Float array</returns>
        public float[]? GetFloatArrayValue(int count)
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueArrayAs<float>(count);

            return null;
        }

        /// <summary>
        /// Gets the property raw data as a float array.
        /// </summary>
        /// <returns>Float array</returns>
        public float[]? GetFloatArrayValue()
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
            {
                int count = ByteCount / sizeof(float);
                return GetValueArrayAs<float>(count);
            }

            return null;
        }

        /// <summary>
        /// Sets the property raw data as a float array.
        /// </summary>
        /// <param name="values">Values to set</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool SetFloatArrayValue(float[] values)
        {
            if (m_type != PropertyType.Float && m_type != PropertyType.Integer)
                return false;

            return SetValueArrayAs<float>(values);
        }

        /// <summary>
        /// Gets the property raw data as a double array.
        /// </summary>
        /// <returns>Double array</returns>
        public double[]? GetDoubleArrayValue()
        {
            if (m_type == PropertyType.Double)
            {
                int count = ByteCount / sizeof(double);
                return GetValueArrayAs<double>(count);
            }

            return null;
        }

        /// <summary>
        /// Sets the property raw data as a double array.
        /// </summary>
        /// <param name="values">Values to set</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool SetDoubleArrayValue(double[] values)
        {
            if (m_type != PropertyType.Double)
                return false;

            return SetValueArrayAs<double>(values);
        }

        /// <summary>
        /// Gets the property raw data as an integer array.
        /// </summary>
        /// <param name="count">Number of elements to get</param>
        /// <returns>Integer array</returns>
        public int[]? GetIntegerArrayValue(int count)
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
                return GetValueArrayAs<int>(count);

            return null;
        }

        /// <summary>
        /// Gets the property raw data as an integer array.
        /// </summary>
        /// <returns>Integer array</returns>
        public int[]? GetIntegerArrayValue()
        {
            if (m_type == PropertyType.Float || m_type == PropertyType.Integer)
            {
                int count = ByteCount / sizeof(int);
                return GetValueArrayAs<int>(count);
            }

            return null;
        }

        /// <summary>
        /// Sets the property raw data as an integer array.
        /// </summary>
        /// <param name="values">Values to set</param>
        /// <returns>True if successful, otherwise false</returns>
        public bool SetIntegerArrayValue(int[] values)
        {
            if (m_type != PropertyType.Float && m_type != PropertyType.Integer)
                return false;

            return SetValueArrayAs<int>(values);
        }

        /// <summary>
        /// Gets the property raw data as a boolean.
        /// </summary>
        /// <returns>Boolean</returns>
        public bool GetBooleanValue()
        {
            return GetIntegerValue() != 0;
        }

        /// <summary>
        /// Sets the property raw data as a boolean.
        /// </summary>
        /// <param name="value">Boolean value</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetBooleanValue(bool value)
        {
            return SetIntegerValue((value == false) ? 0 : 1);
        }

        /// <summary>
        /// Gets the property raw data as a Vector3.
        /// </summary>
        /// <returns>Vector3</returns>
        public Vector3 GetVector3Value()
        {
            if (m_type != PropertyType.Float)
                return new Vector3();

            return GetValueAs<Vector3>();
        }

        /// <summary>
        /// Sets the property raw data as a Vector3.
        /// </summary>
        /// <param name="value">Vector3</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetVector3Value(Vector3 value)
        {
            if (m_type != PropertyType.Float)
                return false;

            return SetValueAs<Vector3>(value);
        }

        /// <summary>
        /// Gets the property raw data as a Vector4.
        /// </summary>
        /// <returns>Vector4</returns>
        public Vector4 GetVector4Value()
        {
            if (m_type != PropertyType.Float || m_rawValue == null)
                return new Vector4();

            //We may have a Color that's RGB, so still read it and set alpha to 1.0
            unsafe
            {
                fixed (byte* ptr = m_rawValue)
                {

                    if (m_rawValue.Length >= MemoryHelper.SizeOf<Vector4>())
                    {
                        return MemoryHelper.Read<Vector4>(new IntPtr(ptr));
                    }
                    else if (m_rawValue.Length >= MemoryHelper.SizeOf<Vector3>())
                    {
                        return new Vector4(MemoryHelper.Read<Vector3>(new IntPtr(ptr)), 1.0f);
                    }

                }
            }

            return new Vector4();
        }

        /// <summary>
        /// Sets the property raw data as a Vector4.
        /// </summary>
        /// <param name="value">Vector4</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool SetVector4Value(Vector4 value)
        {
            if (m_type != PropertyType.Float)
                return false;

            return SetValueAs<Vector4>(value);
        }

        private unsafe T[]? GetValueArrayAs<T>(int count) where T : struct
        {
            int size = MemoryHelper.SizeOf<T>();

            if (m_rawValue != null && (m_rawValue.Length >= (size * count)))
            {
                T[] array = new T[count];
                fixed (byte* ptr = m_rawValue)
                {
                    MemoryHelper.Read<T>(new IntPtr(ptr), array, 0, count);
                }

                return array;
            }

            return null;
        }

        private unsafe T GetValueAs<T>() where T : struct
        {
            int size = MemoryHelper.SizeOf<T>();

            if (m_rawValue != null && m_rawValue.Length >= size)
            {
                fixed (byte* ptr = m_rawValue)
                {
                    return MemoryHelper.Read<T>(new IntPtr(ptr));
                }
            }

            return default;
        }

        private unsafe bool SetValueArrayAs<T>(T[] data) where T : struct
        {
            if (data == null || data.Length == 0)
                return false;

            int size = MemoryHelper.SizeOf<T>(data);

            //Resize byte array if necessary
            if (m_rawValue == null || m_rawValue.Length != size)
                m_rawValue = new byte[size];

            fixed (byte* ptr = m_rawValue)
                MemoryHelper.Write<T>(new IntPtr(ptr), data, 0, data.Length);

            return true;
        }

        private unsafe bool SetValueAs<T>(T value) where T : struct
        {
            int size = MemoryHelper.SizeOf<T>();

            //Resize byte array if necessary
            if (m_rawValue == null || m_rawValue.Length != size)
                m_rawValue = new byte[size];

            fixed (byte* ptr = m_rawValue)
                MemoryHelper.Write<T>(new IntPtr(ptr), value);

            return true;
        }

        private static unsafe string GetMaterialString(byte[]? matPropData)
        {
            if (matPropData == null)
                return string.Empty;

            fixed (byte* ptr = &matPropData[0])
            {
                //String is stored as 32 bit length prefix THEN followed by zero-terminated UTF8 data (basically need to reconstruct an AiString)
                AiString aiString;
                aiString.Length = (uint)MemoryHelper.Read<int>(new IntPtr(ptr));

                //Memcpy starting at dataPtr + sizeof(int) for length + 1 (to account for null terminator)
                MemoryHelper.CopyMemory(new IntPtr(aiString.Data), new IntPtr(ptr) + sizeof(int), (int)aiString.Length + 1);

                return aiString.GetString();
            }
        }

        private static unsafe byte[]? SetMaterialString(string? value, byte[]? existing)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            int stringSize = Encoding.UTF8.GetByteCount(value);

            if (stringSize < 0)
                return null;

            int size = stringSize + 1 + sizeof(int);
            byte[] data;

            if (existing == null || existing.Length != size)
            {
                data = new byte[size];
            }
            else
            {
                data = existing;
            }

            fixed (byte* bytePtr = &data[0])
            {
                MemoryHelper.Write<int>(new IntPtr(bytePtr), stringSize);
                byte[] utfBytes = Encoding.UTF8.GetBytes(value);
                MemoryHelper.Write<byte>(new IntPtr(bytePtr + sizeof(int)), utfBytes, 0, utfBytes.Length);
                //Last byte should be zero
            }

            return data;
        }

        [Conditional("DEBUG")]
        private void AssertIsBaseName()
        {
            if (m_name == null)
                return;

            Debug.Assert(!m_name.Contains(','));
        }

        #region IMarshalable Implementation

        /// <summary>
        /// Gets if the native value type is blittable (that is, does not require marshaling by the runtime, e.g. has MarshalAs attributes).
        /// </summary>
        bool IMarshalable<MaterialProperty, AiMaterialProperty>.IsNativeBlittable => true;

        /// <summary>
        /// Writes the managed data to the native value.
        /// </summary>
        /// <param name="thisPtr">Optional pointer to the memory that will hold the native value.</param>
        /// <param name="nativeValue">Output native value</param>
        void IMarshalable<MaterialProperty, AiMaterialProperty>.ToNative(IntPtr thisPtr, out AiMaterialProperty nativeValue)
        {
            nativeValue.Key = new AiString(m_name);
            nativeValue.Type = m_type;
            nativeValue.Index = (uint)m_texIndex;
            nativeValue.Semantic = m_texType;
            nativeValue.Data = IntPtr.Zero;
            nativeValue.DataLength = 0;

            if (m_rawValue != null)
            {
                nativeValue.DataLength = (uint)m_rawValue.Length;
                nativeValue.Data = MemoryHelper.ToNativeArray<byte>(m_rawValue);
            }
        }

        /// <summary>
        /// Reads the unmanaged data from the native value.
        /// </summary>
        /// <param name="nativeValue">Input native value</param>
        void IMarshalable<MaterialProperty, AiMaterialProperty>.FromNative(in AiMaterialProperty nativeValue)
        {
            m_name = AiString.GetString(nativeValue.Key); //Avoid struct copy
            m_type = nativeValue.Type;
            m_texIndex = (int)nativeValue.Index;
            m_texType = nativeValue.Semantic;
            m_rawValue = null;

            if (nativeValue.DataLength > 0 && nativeValue.Data != IntPtr.Zero)
                m_rawValue = MemoryHelper.FromNativeArray<byte>(nativeValue.Data, (int)nativeValue.DataLength);
        }

        /// <summary>
        /// Frees unmanaged memory created by <see cref="IMarshalable{MaterialProperty, AiMaterialProperty}.ToNative"/>.
        /// </summary>
        /// <param name="nativeValue">Native value to free</param>
        /// <param name="freeNative">True if the unmanaged memory should be freed, false otherwise.</param>
        public static void FreeNative(IntPtr nativeValue, bool freeNative)
        {
            if (nativeValue == IntPtr.Zero)
                return;

            AiMaterialProperty aiMatProp = MemoryHelper.Read<AiMaterialProperty>(nativeValue);

            if (aiMatProp.DataLength > 0 && aiMatProp.Data != IntPtr.Zero)
                MemoryHelper.FreeMemory(aiMatProp.Data);

            if (freeNative)
                MemoryHelper.FreeMemory(nativeValue);
        }

        #endregion
    }
}
