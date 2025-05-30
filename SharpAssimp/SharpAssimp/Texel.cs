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

using System.Numerics;
using System.Runtime.InteropServices;

namespace SharpAssimp
{
    /// <summary>
    /// Represents a texel in ARGB8888 format.
    /// </summary>
    /// <param name="B">Blue component.</param>
    /// <param name="G">Green component.</param>
    /// <param name="R">Red component.</param>
    /// <param name="A">Alpha component.</param>
    [StructLayout(LayoutKind.Sequential)]
    public record struct Texel(byte B, byte G, byte R, byte A)
    {
        /// <summary>
        /// Implicitly converts a texel to a Vector4.
        /// </summary>
        /// <param name="texel">Texel to convert</param>
        /// <returns>Converted Vector4</returns>
        public static implicit operator Vector4(Texel texel)
            => new(texel.R / 255.0f, texel.G / 255.0f, texel.B / 255.0f, texel.A / 255.0f);
    }
}
