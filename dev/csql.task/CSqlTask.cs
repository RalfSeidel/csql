using System;
using Microsoft.Build.Framework;

namespace csql.task
{
	/// <summary>
	/// An task to preprocess and execute (c)sql scripts within msbuild.
	/// </summary>
	public class CSql : ITask
	{
		public CSql()
		{
		}

		/// <summary>
		/// Gets or sets the build engine associated with the task.
		/// </summary>
		public IBuildEngine BuildEngine
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets any host object that is associated with the task.
		/// </summary>
		public ITaskHost HostObject
		{
			get;
			set;
		}

		/// <summary>
		/// Executes a task.
		/// </summary>
		/// <returns>
		/// <c>true</c> if the task executed successfully; otherwise, <c>false</c>.
		/// </returns>
		public bool Execute()
		{
			var message = "Executing Task " + GetType().Name;
			LogMessage( message );
			return true;

		}


		private void LogMessage( string message )
		{
			var eventArgs = new BuildMessageEventArgs( message, string.Empty, GetType().Name, MessageImportance.Low );
			BuildEngine.LogMessageEvent( eventArgs );
		}
	}
}
