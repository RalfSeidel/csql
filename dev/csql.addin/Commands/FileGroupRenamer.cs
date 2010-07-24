using System.IO;
using EnvDTE;

namespace csql.addin.Commands
{
	/// <summary>
	/// 
	/// </summary>
	internal static class FileGroupRenamer
	{
		private static bool isRenaming;

		/// <summary>
		/// Apply the renaming of a grouped script to all items in the group.
		/// </summary>
		/// <param name="projectItem">The project item renamed.</param>
		/// <param name="oldName">The old name of the project item</param>
		public static void OnRename( ProjectItem projectItem, string oldName )
		{
			if ( isRenaming )
				return;

			if ( !FileClassification.IsSqlScript( oldName ) )
				return;

			if ( projectItem.ProjectItems == null || projectItem.ProjectItems.Count == 0 )
				return;

			string oldStem = Path.GetFileNameWithoutExtension( oldName );
			string newName = projectItem.get_FileNames( 1 );
			string newStem = Path.GetFileNameWithoutExtension( newName );
			if ( newStem.ToLowerInvariant().EndsWith( ".all" ) ) {
				newStem = Path.GetFileNameWithoutExtension( newStem );
			}

			isRenaming = true;
			try {
				projectItem.DTE.SuppressUI = true;
				foreach ( ProjectItem subItem in projectItem.ProjectItems ) {
					RenameSubItem( subItem, oldStem, newStem );
				}
			}
			finally {
				projectItem.DTE.SuppressUI = false;
				isRenaming = false;
			}
		}

		private static void RenameSubItem( ProjectItem subItem, string oldStem, string newStem )
		{
			string oldName = subItem.Name;
			if ( !oldName.StartsWith( oldStem ) )
				return;

			string newName = newStem + oldName.Remove( 0, oldStem.Length );
			subItem.Name = newName;
		}
	}
}
