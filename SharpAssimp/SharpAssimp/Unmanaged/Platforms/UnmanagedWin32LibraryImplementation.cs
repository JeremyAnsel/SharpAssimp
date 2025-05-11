/*
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

using System.Runtime.InteropServices;

namespace SharpAssimp.Unmanaged.Platforms
{
    internal sealed partial class UnmanagedWin32LibraryImplementation : UnmanagedLibraryImplementation
    {
        public override string DllExtension => ".dll";

        public UnmanagedWin32LibraryImplementation(string defaultLibName, Type[] unmanagedFunctionDelegateTypes)
            : base(defaultLibName, unmanagedFunctionDelegateTypes)
        {
        }

        protected override IntPtr NativeLoadLibrary(string path)
        {
            IntPtr libraryHandle = WinNativeLoadLibrary(path);

            if (libraryHandle == IntPtr.Zero && ThrowOnLoadFailure)
            {
                Exception? innerException = null;

                //Keep the try-catch in case we're running on Mono. We're providing our own implementation of "Marshal.GetHRForLastWin32Error" which is NOT implemented
                //in mono, but let's just be cautious.
                try
                {
                    int hr = GetHRForLastWin32Error();
                    innerException = Marshal.GetExceptionForHR(hr);
                }
                catch (Exception) { }

                if (innerException != null)
                    throw new AssimpException(
                        $"Error loading unmanaged library from path: {path}\n\n{innerException.Message}", innerException);
                else
                    throw new AssimpException($"Error loading unmanaged library from path: {path}");
            }

            return libraryHandle;
        }

        protected override IntPtr NativeGetProcAddress(IntPtr handle, string functionName)
        {
            return GetProcAddress(handle, functionName);
        }

        protected override void NativeFreeLibrary(IntPtr handle)
        {
            FreeLibrary(handle);
        }

        private static int GetHRForLastWin32Error()
        {
            //Mono, for some reason, throws in Marshal.GetHRForLastWin32Error(), but it should implement GetLastWin32Error, which is recommended than
            //p/invoking it ourselves when SetLastError is set in DllImport
            int dwLastError = Marshal.GetLastWin32Error();

            if ((dwLastError & 0x80000000) == 0x80000000)
                return dwLastError;
            else
                return (dwLastError & 0x0000FFFF) | unchecked((int)0x80070000);
        }

        #region Native Methods

#if NET8_0_OR_GREATER
        [LibraryImport("kernel32.dll", EntryPoint = "LoadLibraryW", SetLastError = true, StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.Utf16StringMarshaller))]
        private static partial IntPtr WinNativeLoadLibrary(string fileName);

        [LibraryImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static partial bool FreeLibrary(IntPtr hModule);

        [LibraryImport("kernel32.dll", StringMarshalling = StringMarshalling.Custom, StringMarshallingCustomType = typeof(System.Runtime.InteropServices.Marshalling.AnsiStringMarshaller))]
        private static partial IntPtr GetProcAddress(IntPtr hModule, string procName);
#else
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode, BestFitMapping = false, SetLastError = true, EntryPoint = "LoadLibraryW")]
        public static extern IntPtr WinNativeLoadLibrary(string fileName);

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeLibrary(IntPtr hModule);

        [DllImport("kernel32.dll", CharSet = CharSet.Ansi)]
        private static extern IntPtr GetProcAddress(IntPtr hModule, string procName);
#endif

        #endregion
    }
}
