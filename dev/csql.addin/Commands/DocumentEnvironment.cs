using System;
using System.Collections.Generic;
using System.Text;
using EnvDTE;
using System.Diagnostics;
using System.IO;
using EnvDTE80;
using System.Collections;

namespace csql.addin.Commands
{
	/// <summary>
	/// Enumeration of supported environment variables 
	/// that can be substituted in the command line 
	/// arguments.
	/// </summary>
	internal enum EnvironmentVariable
	{
		/// <summary>
		/// The directory of the current solution.
		/// </summary>
		SolutionDir,
		SolutionDirectory = SolutionDir,

		/// <summary>
		/// The directory of the project the current item belongs to.
		/// </summary>
		ProjectDir,
		ProjectDirectory = ProjectDir,

		/// <summary>
		/// The directory of the item processed.
		/// </summary>
		ItemDir,
		ItemDirectory = ItemDir,

		/// <summary>
		/// The output directory.
		/// </summary>
		TargetDir,
		TargetDirectory = TargetDir,
	}

	/// <summary>
	/// Resolved environment variables associated with a document.
	/// </summary>
	public interface IDocumentEnvironment
	{
		/// <summary>
		/// The directory of the item processed.
		/// </summary>
		string ItemDirectory { get; }

		/// <summary>
		/// The directory of the project the current item belongs to.
		/// </summary>
		string ProjectDirectory { get; }

		/// <summary>
		/// The directory of the current solution.
		/// </summary>
		string SolutionDirectory { get; }

		/// <summary>
		/// The output directory.
		/// </summary>
		string TargetDirectory { get; }
	}

	/// <summary>
	/// Resolved environment variables associated with a document.
	/// </summary>
	public class DocumentEnvironment : IDocumentEnvironment
	{
		private readonly Document document;

		/// <summary>
		/// Initializes a new instance of the <see cref="Environment"/> class.
		/// </summary>
		public DocumentEnvironment( Document document )
		{
			Debug.Assert( document != null );
			this.document = document;
		}

		/// <summary>
		/// The directory of the item processed.
		/// </summary>
		public string ItemDirectory
		{
			get
			{
				string itemPath = document.FullName;
				string directory = GetRootedDirectory( itemPath );
				return directory;
			}
		}

		/// <summary>
		/// The directory of the project the current item belongs to.
		/// If the item does not belong to any project the property
		/// the method returns the directory of the active project.
		/// If there is no active project the property returns 
		/// the <see cref="P:SolutionDirectory"/> instead.
		/// </summary>
		public string ProjectDirectory
		{
			get
			{
				Project project = DocumentProject;

				if ( project == null )
					return SolutionDirectory;

				string projectPath = project.FullName;
				if ( string.IsNullOrEmpty( projectPath ) )
					return SolutionDirectory;

				string directory = GetRootedDirectory( projectPath );
				return directory;
			}
		}


		/// <summary>
		/// The directory of the current solution.
		/// If no solution is loaded the property returns 
		/// the <see cref="P:ItemDirectory"/> instead.
		/// </summary>
		public string SolutionDirectory
		{
			get
			{
				Solution solution = document.DTE.Solution;
				if ( solution == null )
					return ItemDirectory;

				string solutionPath = solution.FullName;
				string directory = GetRootedDirectory( solutionPath );
				return directory;
			}
		}

		/// <summary>
		/// The output directory.
		/// </summary>
		public string TargetDirectory
		{
			get
			{
				throw new NotSupportedException( "TODO" );
			}
		}

		private Project DocumentProject
		{
			get
			{
				ProjectItem projectItem = document.ProjectItem;
				if ( projectItem == null )
					return ActiveProject;

				Project project = projectItem.ContainingProject;
				if ( project == null )
					project = ActiveProject;

				return project;
			}
		}

		private Project ActiveProject
		{
			get
			{
				DTE application = document.DTE;
				IEnumerable  projects = application.ActiveSolutionProjects as IEnumerable;
				if ( projects == null )
					return null;

				IEnumerator e = projects.GetEnumerator();
				if ( !e.MoveNext() )
					return null;

				Project project = e.Current as Project;
				return project;
			}
		}


		private static string GetRootedDirectory( string path )
		{
			string pathRoot = Path.GetPathRoot( path );
			string directory = Path.GetDirectoryName( path );
			if ( string.IsNullOrEmpty( pathRoot ) )
				return directory;

			directory = Path.Combine( pathRoot, directory );
			return directory;
		}

	}

}