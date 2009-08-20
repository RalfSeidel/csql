using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace csql
{
    /// <summary>
    /// Native methode declarations.
    /// </summary>
    class NativeMethods
    {
        /// <summary>
        /// See <see cref="http://msdn.microsoft.com/en-us/library/aa365150(VS.85).aspx"/>
        /// </summary>
        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern SafeFileHandle CreateNamedPipe(
            String pipeName,
            uint dwOpenMode,
            uint dwPipeMode,
            uint nMaxInstances,
            uint nOutBufferSize,
            uint nInBufferSize,
            uint nDefaultTimeOut,
            IntPtr lpSecurityAttributes );


        /// <summary>
        /// Connects the named pipe.
        /// </summary>
        [DllImport( "kernel32.dll", SetLastError = true )]
        public static extern int ConnectNamedPipe(
           SafeFileHandle hNamedPipe,
           IntPtr lpOverlapped );
    }
}
