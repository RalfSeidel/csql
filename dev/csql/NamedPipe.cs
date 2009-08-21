using System;
using System.IO;
using Microsoft.Win32.SafeHandles;

namespace csql
{
    /// <summary>
    /// Class to create a named pipe.
    /// </summary>
    public class NamedPipe
    {
        private const string m_pipePathPrefix = "\\\\.\\pipe\\";
        private const int m_pipeBufferSize = 4096;

        /// <summary>
        /// The handle of the pipe once it has been created.
        /// </summary>
        private SafeFileHandle m_pipeHandle;


        /// <summary>
        /// Flags for the CreateNamedPipe Api call.
        /// </summary>
        [Flags]
        private enum OpenMode : uint
        {
            /// <summary>
            /// 
            /// </summary>
            PipeAccessInbound = 0x00000001,
            /// <summary>
            /// 
            /// </summary>
            PipeAccessOutput = 0x00000002,
            /// <summary>
            /// 
            /// </summary>
            PipeAccessDuplex = 0x00000003,
            /// <summary>
            /// 
            /// </summary>
            WriteDac = 0x00040000,
            /// <summary>
            /// 
            /// </summary>
            WriteOwner = 0x00080000,
            /// <summary>
            /// 
            /// </summary>
            AccessSystemSecurity = 0x01000000,
            /// <summary>
            /// 
            /// </summary>
            FileFlagFirstPipeInstance = 0x00080000,
            /// <summary>
            /// 
            /// </summary>
            FileFlagOverlapped = 0x40000000,
            /// <summary>
            /// 
            /// </summary>
            FileFlagWriteThough = 0x80000000
        }

        /// <summary>
        /// Flags for the CreateNamedPipe Api call.
        /// </summary>
        [Flags]
        private enum PipeMode : uint
        {
            /// <summary>
            /// 
            /// </summary>
            PipeWait = 0x00,
            /// <summary>
            /// 
            /// </summary>
            PipeNowait = 0x01,
            /// <summary>
            /// 
            /// </summary>
            PipeReadmodeByte = 0x00,
            /// <summary>
            /// 
            /// </summary>
            PipeReadmodeMessage = 0x02,
            /// <summary>
            /// 
            /// </summary>
            PipeTypeByte = 0x00,
            /// <summary>
            /// 
            /// </summary>
            PipeTypeMessage = 0x04,
            /// <summary>
            /// 
            /// </summary>
            PipeAcceptRemoteClients = 0x00,
            /// <summary>
            /// 
            /// </summary>
            PipeRejectRemoteClients = 0x08
        }

        /// <summary>
        /// Initializes a new uninitalized name pipe instance.
        /// </summary>
        public NamedPipe()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NamedPipe"/> class.
        /// </summary>
        /// <param name="name">The name of the pipe to create.</param>
        public NamedPipe( string name )
        {
            Create( name );
        }


        /// <summary>
        /// Gets the pipe handle.
        /// </summary>
        /// <value>The pipe handle.</value>
        public SafeFileHandle PipeHandle
        {
            get { return m_pipeHandle; }
        }

        /// <summary>
        /// Gets the pipe path for the given pipe name.
        /// </summary>
        /// <remarks>
        /// The methode simply prepends the pipe prefix to the name.
        /// </remarks>
        /// <param name="pipeName">Name of the pipe.</param>
        /// <returns>The full qualified path of the pipe.</returns>
        public static string GetPipePath( string pipeName )
        {
            string pipePath = m_pipePathPrefix + pipeName;
            return pipePath;
        }

        /// <summary>
        /// Creates the nameed pipe.
        /// </summary>
        /// <param name="name">The name of the pipe.</param>
        public void Create( string name )
        {
            if ( String.IsNullOrEmpty( name )  ) {
                throw new ArgumentNullException( "name", "The name of the pipe has to be provided by the caller." );
            }
            if ( m_pipeHandle != null ) {
                throw new InvalidOperationException( "The pipe has allready been created." );
            }
            string pipePath = m_pipePathPrefix + name;
            uint openMode = (uint)(OpenMode.PipeAccessDuplex | OpenMode.FileFlagFirstPipeInstance | OpenMode.FileFlagOverlapped);
            uint pipeMode = (uint)(PipeMode.PipeWait | PipeMode.PipeTypeByte | PipeMode.PipeRejectRemoteClients);
            SafeFileHandle handle = NativeMethods.CreateNamedPipe(
                pipePath,
                openMode,
                pipeMode,
                1,
                m_pipeBufferSize,
                m_pipeBufferSize,
                0,
                IntPtr.Zero );

            if ( handle.IsInvalid ) {
                throw new System.ComponentModel.Win32Exception( "CreateNamedPipe Api call failed" );
            }

            m_pipeHandle = handle;

        }

        /// <summary>
        /// Opens the pipe for reading.
        /// </summary>
        /// <returns>A stream for the pipe.</returns>
        public Stream Open()
        {
            int success = NativeMethods.ConnectNamedPipe( m_pipeHandle, IntPtr.Zero );
            if ( success == -1 ) {
                throw new System.ComponentModel.Win32Exception( "ConnectNamedPipe Api call failed" );
            }

            FileStream stream = new FileStream( m_pipeHandle, FileAccess.Read, m_pipeBufferSize, false );
            return stream;
        }

    }
}
