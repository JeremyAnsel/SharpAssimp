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
    /// Callback delegate for Assimp's LogStream.
    /// </summary>
    /// <param name="msg">Log message</param>
    /// <param name="userData">Supplied user data</param>
    public delegate void LoggingCallback(string msg, string userData);

    /// <summary>
    /// Represents a log stream, which receives all log messages and streams them somewhere.
    /// </summary>
    [DebuggerDisplay("IsAttached = {IsAttached}")]
    public class LogStream : IDisposable
    {
        private static readonly Lock s_sync = new();
        private static readonly List<LogStream> s_activeLogstreams = [];

        private LoggingCallback? m_logCallback;

        // Don't delete this, holding onto the callbacks prevent them from being GC'ed inappropiately
        private AiLogStreamCallback? m_assimpCallback;
        private IntPtr m_logstreamPtr;
        private string m_userData = string.Empty;
        private bool m_isDisposed;
        private bool m_isAttached;

        /// <summary>
        /// Gets or sets, if verbose logging is enabled globally.
        /// </summary>
        public static bool IsVerboseLoggingEnabled
        {
            get => AssimpLibrary.Instance.GetVerboseLoggingEnabled();
            set => AssimpLibrary.Instance.EnableVerboseLogging(value);
        }

        /// <summary>
        /// Gets or sets the user data to be passed to the callback.
        /// </summary>
        public string UserData
        {
            get => m_userData;
            set => m_userData = value;
        }

        /// <summary>
        /// Gets whether the logstream has been disposed or not.
        /// </summary>
        public bool IsDisposed => m_isDisposed;

        /// <summary>
        /// Gets whether or not the logstream is currently attached to the library.
        /// </summary>
        public bool IsAttached => m_isAttached;

        /// <summary>
        /// Static constructor.
        /// </summary>
        static LogStream()
        {
            AssimpLibrary.Instance.LibraryFreed += AssimpLibraryFreed;
        }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="initialize">
        /// Whether to immediately initialize the system by setting up native pointers. Set this to
        /// false if you want to manually initialize and use custom function pointers for advanced use cases.
        /// </param>
        protected LogStream(bool initialize = true) : this("", initialize) { }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="userData">User-supplied data</param>
        /// <param name="initialize">True if initialize should be immediately called with the default callbacks. Set this to false
        /// if your subclass requires a different way to setup the function pointers.</param>
        protected LogStream(string userData, bool initialize = true)
        {
            if (initialize)
                Initialize(OnAiLogStreamCallback, null, userData);
        }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="callback">Logging callback that is called when messages are received by the log stream.</param>
        public LogStream(LoggingCallback callback)
        {
            Initialize(OnAiLogStreamCallback, callback);
        }

        /// <summary>
        /// Constructs a new LogStream.
        /// </summary>
        /// <param name="callback">Logging callback that is called when messages are received by the log stream.</param>
        /// <param name="userData">User-supplied data</param>
        public LogStream(LoggingCallback callback, string userData)
        {
            Initialize(OnAiLogStreamCallback, callback, userData);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="LogStream"/> class.
        /// </summary>
        ~LogStream()
        {
            Dispose(false);
        }

        /// <summary>
        /// Detaches all active logstreams from the library.
        /// </summary>
        public static void DetachAllLogstreams()
        {
            lock (s_sync)
            {
                foreach (LogStream logstream in s_activeLogstreams)
                {
                    logstream.m_isAttached = false;
                    AssimpLibrary.Instance.DetachLogStream(logstream.m_logstreamPtr);
                    logstream.OnDetach();
                }

                s_activeLogstreams.Clear();
            }
        }

        /// <summary>
        /// Gets all active logstreams that are currently attached to the library.
        /// </summary>
        /// <returns>Collection of active logstreams attached to the library.</returns>
        public static IEnumerable<LogStream> GetAttachedLogStreams()
        {
            lock (s_sync)
            {
                //Return a copy to prevent concurrent modification exceptions.
                return [.. s_activeLogstreams];
            }
        }

        //Ensure we cleanup our logstreams if any are around when the unmanaged library is freed.
        private static void AssimpLibraryFreed(object? sender, EventArgs e)
        {
            DetachAllLogstreams();
        }

        /// <summary>
        /// Attaches the logstream to the library.
        /// </summary>
        public void Attach()
        {
            if (m_isAttached)
                return;

            lock (s_sync)
            {
                s_activeLogstreams.Add(this);
                m_isAttached = true;
                AssimpLibrary.Instance.AttachLogStream(m_logstreamPtr);
                OnAttach();
            }
        }

        /// <summary>
        /// Detaches the logstream from the library.
        /// </summary>
        public void Detach()
        {
            if (!m_isAttached)
                return;

            lock (s_sync)
            {
                s_activeLogstreams.Remove(this);
                m_isAttached = false;
                AssimpLibrary.Instance.DetachLogStream(m_logstreamPtr);
                OnDetach();
            }
        }

        /// <summary>
        /// Logs a message.
        /// </summary>
        /// <param name="msg">Message contents</param>
        public void Log(string msg)
        {
            if (!m_isAttached || string.IsNullOrEmpty(msg))
                return;

            OnAiLogStreamCallback(msg, IntPtr.Zero);
        }

        /// <summary>
        /// Releases unmanaged resources held by the LogStream. This should not be called by the user if the logstream is currently attached to an assimp importer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing">True to release both managed and unmanaged resources; False to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!m_isDisposed)
            {
                if (m_logstreamPtr != IntPtr.Zero)
                {
                    MemoryHelper.FreeMemory(m_logstreamPtr);
                    m_logstreamPtr = IntPtr.Zero;
                }

                if (disposing)
                    m_assimpCallback = null;

                m_isDisposed = true;
            }
        }

        /// <summary>
        /// Override this method to log a message for a subclass of Logstream, if no callback
        /// was set.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="userData">User data</param>
        protected virtual void LogMessage(string msg, string userData) { }

        /// <summary>
        /// Called when the log stream has been attached to the assimp importer. At this point it may start receiving messages.
        /// </summary>
        protected virtual void OnAttach() { }

        /// <summary>
        /// Called when the log stream has been detatched from the assimp importer. After this point it will stop receiving
        /// messages until it is re-attached.
        /// </summary>
        protected virtual void OnDetach() { }

        /// <summary>
        /// Callback for Assimp that handles a message being logged.
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="userData"></param>
        protected void OnAiLogStreamCallback(string msg, IntPtr userData)
        {
            if (m_logCallback != null)
            {
                m_logCallback(msg, m_userData);
            }
            else
            {
                LogMessage(msg, m_userData);
            }
        }

        /// <summary>
        /// Initializes the stream by setting up native pointers for Assimp to the specified functions.
        /// </summary>
        /// <param name="aiLogStreamCallback">Callback that is marshaled to native code, a reference is held on to avoid it being GC'ed.</param>
        /// <param name="callback">User callback, if any. Defaults to console if null.</param>
        /// <param name="userData">User data, or empty.</param>
        /// <param name="assimpUserData">Additional assimp user data, if any.</param>
        protected void Initialize(AiLogStreamCallback aiLogStreamCallback, LoggingCallback? callback = null, string userData = "", IntPtr assimpUserData = default)

        {
            userData ??= string.Empty;

            m_assimpCallback = aiLogStreamCallback;
            m_logCallback = callback;
            m_userData = userData;

            AiLogStream logStream;
            logStream.Callback = Marshal.GetFunctionPointerForDelegate(aiLogStreamCallback);
            logStream.UserData = assimpUserData;

            m_logstreamPtr = MemoryHelper.AllocateMemory(MemoryHelper.SizeOf<AiLogStream>());
            Marshal.StructureToPtr(logStream, m_logstreamPtr, false);
        }
    }

    /// <summary>
    /// Log stream that writes messages to the Console.
    /// </summary>
    public sealed class ConsoleLogStream : LogStream
    {
        /// <summary>
        /// Constructs a new console logstream.
        /// </summary>
        public ConsoleLogStream() : base() { }

        /// <summary>
        /// Constructs a new console logstream.
        /// </summary>
        /// <param name="userData">User supplied data</param>
        public ConsoleLogStream(string userData) : base(userData) { }

        /// <summary>
        /// Log a message to the console.
        /// </summary>
        /// <param name="msg">Message</param>
        /// <param name="userData">Userdata</param>
        protected override void LogMessage(string msg, string userData)
        {
            if (string.IsNullOrEmpty(userData))
            {
                Console.WriteLine(msg);
            }
            else
            {
                Console.WriteLine($"{userData}: {msg}");
            }
        }
    }
}
