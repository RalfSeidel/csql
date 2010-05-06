using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
	/// <summary>
	/// Test environment settings.
	/// </summary>
	internal static class Environment
	{
		/// <summary>
		/// Gets the project root directory.
		/// TODO: use msbuild task to determine this directory.
		/// </summary>
		public static string ProjectRootDirectory
		{
			get
			{
				return @"D:\src\Codeplex\sqtpp\IntegrationTest\";
			}
		}

		public static string WorkingDirectory
		{
			get
			{
				return System.IO.Path.Combine( Environment.ProjectRootDirectory, @"Files\sqtpp\" );
			}
		}


		public static string TestFileDirectory
		{
			get
			{
				return System.IO.Path.Combine( Environment.ProjectRootDirectory, @"Files\sqtpp\input\" );
			}
		}

		public static string PathToSqtpp
		{
			get
			{
				string pathToSqtpp = Environment.ProjectRootDirectory + @"..\Debug\sqtpp.exe";
				return pathToSqtpp;
			}
		}


		public static void InitializeSystemEnvironment()
		{
			System.Environment.SetEnvironmentVariable( "computername", "sqtpp_test_host" );
			System.Environment.SetEnvironmentVariable( "username", "sqtpp_test_user" );
		}



	}
}
