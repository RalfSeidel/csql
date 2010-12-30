using System;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace csql
{
	/// <summary>
	/// Native methode declarations.
	/// </summary>
	internal static class NativeMethods
	{
		/// <summary>
		/// Resultcode of <see cref="WaitForSingleObject"/>
		/// </summary>
		public const uint WAIT_OBJECT_0  = 0x00000000;
		/// <summary>
		/// Resultcode of <see cref="WaitForSingleObject"/>
		/// </summary>
		public const uint WAIT_ABANDONED = 0x00000080;
		/// <summary>
		/// Resultcode of <see cref="WaitForSingleObject"/>
		/// </summary>
		public const uint WAIT_TIMEOUT   = 0x00000102;
		/// <summary>
		/// Resultcode of <see cref="WaitForSingleObject"/>
		/// </summary>
		public const uint WAIT_FAILED    = 0xFFFFFFFF;

		/// <summary>
		/// See <see cref="http://msdn.microsoft.com/en-us/library/aa365150.aspx"/>
		/// </summary>
		[DllImport( "kernel32.dll", SetLastError=true, CharSet=CharSet.Unicode )]
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
		[DllImport( "kernel32.dll", SetLastError=true )]
		public static extern int ConnectNamedPipe( SafeFileHandle hNamedPipe, IntPtr lpOverlapped );

		/*
		/// <summary>
		/// Waits until the object referenced by the handle is in the signaled state or a time-out occurred.
		/// </summary>
		/// <seealso cref="http://msdn.microsoft.com/en-us/library/ms687032.aspx"/>
		[DllImport( "kernel32.dll", SetLastError=true )]
		public static extern uint WaitForSingleObject( IntPtr hHandle, UInt32 dwMilliseconds );
		*/
	}
}
