using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using EnvDTE;
using EnvDTE80;

namespace csql.addin.Commands
{
	/// <summary>
	/// Modify the hierarchy for .csql files.
	/// </summary>
	public class FileGrouper
	{
		/// <summary>
		/// Reference to the visual studio automation object.
		/// </summary>
		private readonly DTE2 application;

		/// <summary>
		/// The constructor.
		/// </summary>
		/// <param name="application">The visual studio automation application.</param>
		public FileGrouper( DTE2 application )
		{
			this.application = application;
		}

		/// <summary>
		/// Modify the hierarchy for .csql files.
		/// </summary>
		public void GroupProjectItems()
		{
			ToolWindows toolWindows = application.ToolWindows;

			application.SuppressUI = true;

			UIHierarchy hierarchy = toolWindows.SolutionExplorer;
			if ( hierarchy.UIHierarchyItems.Count <= 0 )
				return;

			GroupItems( hierarchy.UIHierarchyItems );

			application.SuppressUI = false;
		}

		/// <summary>
		/// Check if the given element from the solution explorer hierarchy is a file.
		/// </summary>
		/// <param name="item">A item of the project solution hierarchy.</param>
		/// <returns></returns>
		private bool IsFile( UIHierarchyItem item )
		{
			string fileName = GetItemFileName( item );
			return !String.IsNullOrEmpty( fileName );
		}

		/// <summary>
		/// Gets the name of a project item that referes to file
		/// </summary>
		/// <param name="item">The project item.</param>
		/// <returns>The name of the file or <c>null</c> if the item does not represent a single file.</returns>
		private string GetItemFileName( UIHierarchyItem item )
		{
			if ( !(item.Object is EnvDTE.ProjectItem) )
				return null;

			EnvDTE.ProjectItem prjItem = (EnvDTE.ProjectItem)item.Object;
			if ( prjItem.FileCount != 1 )
				return null;

			string fileName = prjItem.get_FileNames( 1 );
			if ( String.IsNullOrEmpty( fileName ) )
				return null;

			return fileName;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="group"></param>
		/// <param name="item"></param>
		private void AddSubItemsToItemGroup( ItemGroup group, UIHierarchyItem item )
		{
			UIHierarchyItems subitems = item.UIHierarchyItems;
			foreach ( UIHierarchyItem subitem in subitems ) {
				group.Items.Add( subitem );
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="masterItem"></param>
		/// <param name="childItem"></param>
		private void AddItemToMasterItem( ProjectItem masterItem, ProjectItem childItem )
		{
			foreach ( ProjectItem subItem in childItem.ProjectItems ) {
				AddItemToMasterItem( masterItem, subItem );
			}
			if ( childItem.Collection != masterItem.ProjectItems ) {
				string fileName = childItem.get_FileNames( 0 );
				childItem.Remove();
				masterItem.ProjectItems.AddFromFile( fileName );
			}
		}


		/// <summary>
		/// Recursivly group all items ending with .csql together.
		/// </summary>
		/// <param name="items"></param>
		private void GroupItems( UIHierarchyItems items )
		{
			ItemGroups groups = new ItemGroups();

			// Build the groups
			foreach ( UIHierarchyItem item in items ) {
				bool isCsqlFile = false;
				if ( IsFile( item ) ) {
					ProjectItem prjItem = (EnvDTE.ProjectItem)item.Object;
					string fileName = GetItemFileName( item ).ToLower().Trim();

					if ( fileName.Length > ".csql".Length && fileName.EndsWith( ".csql" ) ) {
						isCsqlFile = true;
					}
				}
				if ( isCsqlFile ) {
					string name = item.Name;
					int firstDot = name.IndexOf( '.' );
					string groupName = name.Substring( 0, firstDot );
					ItemGroup group = groups.GetGroup( groupName );
					group.Items.Add( item );
					AddSubItemsToItemGroup( group, item );
				}
				else {
					GroupItems( item.UIHierarchyItems );
				}
			}

			// Restructure the project hierarchy.
			foreach ( ItemGroup group in groups.Values ) {
				if ( group.MasterItem != null ) {
					UIHierarchyItem master = group.MasterItem;
					IEnumerable<UIHierarchyItem> childs = group.ChildItems;
					ProjectItem masterItem = (ProjectItem)master.Object;

					foreach ( UIHierarchyItem item in childs ) {
						ProjectItem childItem = (ProjectItem)item.Object;
						AddItemToMasterItem( masterItem, childItem );
					}
				}
			}
		}

		#region Embedded classes

		/// <summary>
		/// A group of project item files.
		/// </summary>
		private class ItemGroup
		{
			/// <summary>
			/// See <see cref="P:Name"/>
			/// </summary>
			private readonly string groupName;

			/// <summary>
			/// See <see cref="P:Items"/>
			/// </summary>
			private IList<UIHierarchyItem> groupItems = new List<UIHierarchyItem>();


			#region Constructor

			public ItemGroup( string groupName )
			{
				this.groupName = groupName;
			}

			#endregion

			#region Properties

			/// <summary>
			/// The name of the group i.e. the stem the file name up to the first 
			/// appearance of a dot.
			/// </summary>
			public string Name
			{
				get { return this.groupName; }
			}


			/// <summary>
			/// All file items found having the same name and ending with .csql.
			/// </summary>
			public IList<UIHierarchyItem> Items
			{
				get { return this.groupItems; }
			}

			/// <summary>
			/// Gets the master item of a group.
			/// </summary>
			/// <remarks>
			/// The implementation checks all items in the group. The with 
			/// the name of the group an the single extension ".csql" is 
			/// considered to be the master item. 
			/// </remarks>
			/// <value>The master item.</value>
			public UIHierarchyItem MasterItem
			{
				get
				{
					string masterItemFullName;
					UIHierarchyItem item;

					masterItemFullName = Name.ToLower() + ".csql";
					item = FindItem( masterItemFullName );
					if ( item != null )
						return item;

					masterItemFullName = Name.ToLower() + ".all.csql";
					item = FindItem( masterItemFullName );
					if ( item != null )
						return item;

					return null;
				}
			}

			/// <summary>
			/// Gets all the child items of the master.
			/// </summary>
			/// <value>The child items.</value>
			public IList<UIHierarchyItem> ChildItems
			{
				get
				{
					UIHierarchyItem master = this.MasterItem;
					List<UIHierarchyItem> childs = new List<UIHierarchyItem>( this.Items.Count );

					foreach ( UIHierarchyItem item in this.groupItems ) {
						if ( item != master ) {
							childs.Add( item );
						}
					}
					childs.Sort( ChildItemComparer.Instance );
					return childs;
				}
			}

			/// <summary>
			/// Check that all files in this group exists. If not we will not build 
			/// the group.
			/// </summary>
			private bool FilesExists
			{
				get
				{
					foreach ( UIHierarchyItem item in this.groupItems ) {
						ProjectItem prjItem = (ProjectItem)item.Object;
						if ( prjItem.FileCount != 1 ) {
							return false;
						}
						string fileName = prjItem.get_FileNames( 0 );
						if ( !File.Exists( fileName ) ) {
							return false;
						}
					}
					return true;
				}
			}

			#endregion

			/// <summary>
			/// Finds the item with the specified full name.
			/// </summary>
			/// <param name="fullName">The full name converted in lower case .</param>
			/// <returns>The item with the same name if if exists. <c>null</c> otherwise</returns>
			private UIHierarchyItem FindItem( string fullName )
			{
				foreach ( UIHierarchyItem item in this.groupItems ) {
					string name = item.Name.ToLower();

					if ( name.Equals( fullName ) ) {
						return item;
					}
				}
				return null;
			}
		}

		/// <summary>
		/// Helper class to collect item groups.
		/// </summary>
		private class ItemGroups : Dictionary<string, ItemGroup>
		{
			/// <summary>
			/// Get the group with the given name.
			/// </summary>
			/// <param name="groupName"></param>
			/// <returns></returns>
			public ItemGroup GetGroup( string groupName )
			{
				ItemGroup group = null;
				this.TryGetValue( groupName, out group );
				if ( group == null ) {
					group = new ItemGroup( groupName );
					this.Add( groupName, group );
				}
				return group;
			}
		}


		/// <summary>
		/// Comparer for the sorting of the child items.
		/// </summary>
		private class ChildItemComparer : Comparer<UIHierarchyItem>
		{
			/// <summary>
			/// <see cref="P:Instance"/>
			/// </summary>
			private static ChildItemComparer instance = new ChildItemComparer();

			/// <summary>
			/// Get the singleton instance of this class.
			/// </summary>
			public static ChildItemComparer Instance
			{
				get { return instance; }
			}

			public override int Compare( UIHierarchyItem x, UIHierarchyItem y )
			{
				string leftExtension = GetExtension( x );
				int leftOrderKey = GetExtensionOrderKey( leftExtension );
				string rightExtension = GetExtension( y );
				int rightOrderKey = GetExtensionOrderKey( rightExtension );

				int result = leftOrderKey - rightOrderKey;
				return result;
			}

			private static string GetExtension( UIHierarchyItem item )
			{
				int firstDot = item.Name.IndexOf( '.' );
				int lastDot = item.Name.LastIndexOf( '.' );
				string extension = lastDot > firstDot ? item.Name.Substring( firstDot + 1, lastDot - firstDot - 1 ) : "";
				extension = extension.ToLower();
				return extension;
			}

			private static int GetExtensionOrderKey( string extension )
			{
				switch ( extension ) {
					case "": return 0;
					case "tbl": return 10;
					case "key": return 20;
					case "fct": return 30;
					case "vws": return 40;
					case "prc": return 50;
					case "trg": return 60;
					case "ins": return 70;
					case "tst": return 100;
					default: return 1000;
				}
			}
		}

		#endregion
	}
}
