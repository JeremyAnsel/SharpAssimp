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
using System.Runtime.InteropServices;

namespace SharpAssimp
{
    /// <summary>
    /// Describes a file format which Assimp can export to.
    /// </summary>
    [DebuggerDisplay("{Description}")]
    public sealed class ExportFormatDescription
    {
        private readonly string m_formatId;
        private readonly string m_description;
        private readonly string m_fileExtension;

        /// <summary>
        /// Gets a short string ID to uniquely identify the export format. E.g. "collada" or "obj".
        /// </summary>
        public string FormatId => m_formatId;

        /// <summary>
        /// Gets a short description of the file format to present to users.
        /// </summary>
        public string Description => m_description;

        /// <summary>
        /// Gets the recommended file extension for the exported file in lower case.
        /// </summary>
        public string FileExtension => m_fileExtension;

        /// <summary>
        /// Constructs a new ExportFormatDescription.
        /// </summary>
        /// <param name="formatDesc">Unmanaged structure</param>
        internal ExportFormatDescription(in AiExportFormatDesc formatDesc)
        {
            m_formatId = Marshal.PtrToStringAnsi(formatDesc.FormatId) ?? string.Empty;
            m_description = Marshal.PtrToStringAnsi(formatDesc.Description) ?? string.Empty;
            m_fileExtension = Marshal.PtrToStringAnsi(formatDesc.FileExtension) ?? string.Empty;

            //Stupid hack, for some reason the formatID for COLLADA format is always messed up
            if (m_fileExtension == "dae")
                m_formatId = "collada";
        }
    }
}
