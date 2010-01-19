using System;
using System.Collections;
using EnvDTE;
using EnvDTE80;

namespace csql.addin.Commands
{
	/// <summary>
	/// This class implements the logic to find the current editor file 
	/// in the vistual studio solution explorer hierarchy.
	/// </summary>
	public class SolutionExplorerFileLocator
	{
		/// <summary>
		/// The visutal studio application object.
		/// </summary>
		private readonly DTE2 application;

		/// <summary>
		/// Initializes a new instance of the <see cref="SolutionExplorerFileLocator"/> class.
		/// </summary>
		/// <param name="application">The visual studio automation application.</param>
		public SolutionExplorerFileLocator( DTE2 application )
		{
			this.application = application;
		}


		public void LocateAndSelectCurrentFile()
		{
			Document activeDocument = this.application.ActiveDocument;
			if ( activeDocument == null ) {
				// TODO: message
				return;
			}

			ProjectItem projectItem = activeDocument.ProjectItem;
			if ( projectItem == null ) {
				// Most probably reason: The current document is not included in any of project 
				// i.e. the user opened an external file.
				// TODO: message
				return;
			}

			UIHierarchyItems solutionItems = application.ToolWindows.SolutionExplorer.UIHierarchyItems;

			if ( solutionItems.Count != 1 ) {
				// No open solution
				return;
			}

			// FindHierarchyItem expands nodes as well (it must do so, because children arent loaded until expanded) 
			UIHierarchyItem uiItem = FindHierarchyItem( solutionItems.Item( 1 ).UIHierarchyItems, projectItem );

			if ( uiItem != null ) {
				uiItem.Select( vsUISelectionType.vsUISelectionTypeSelect ); //own
				application.ToolWindows.SolutionExplorer.Parent.Activate();
			}
		}


		/// <summary>
		/// Finds the hierarchy item.
		/// </summary>
		/// <param name="items">The items.</param>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		private UIHierarchyItem FindHierarchyItem( UIHierarchyItems items, object item )
		{
			// Enumerating children recursive would work, but it may be slow on large solution. 
			// This tries to be smarter and faster 

			Stack itemStack = new Stack();
			CreateItemsStack( itemStack, item );

			UIHierarchyItem last = null;
			while ( itemStack.Count != 0 ) {
				if ( !items.Expanded )
					items.Expanded = true;
				if ( !items.Expanded ) {
					// Workaround: expand dont always work... 
					UIHierarchyItem parent = ((UIHierarchyItem)items.Parent);
					parent.Select( vsUISelectionType.vsUISelectionTypeSelect );
					this.application.ToolWindows.SolutionExplorer.DoDefaultAction();
				}

				object o = itemStack.Pop();

				last = null;
				foreach ( UIHierarchyItem child in items ) {
					if ( child.Object == o ) {
						last = child;
						items = child.UIHierarchyItems;
						break;
					}
				}
			}

			return last;
		}


		/// <summary>
		/// Recursive iteration over the project hierachy.
		/// </summary>
		/// <param name="s">The stack of items.</param>
		/// <param name="item">The current item in the recursion.</param>
		private void CreateItemsStack( Stack s, object item )
		{
			if ( item is ProjectItem ) {
				ProjectItem pi = (ProjectItem)item;
				s.Push( pi );
				CreateItemsStack( s, pi.Collection.Parent );
			} else if ( item is Project ) {
				Project p = (Project)item;
				s.Push( p );
				if ( p.ParentProjectItem != null ) {
					//top nodes dont have solution as parent, but is null 
					CreateItemsStack( s, p.ParentProjectItem );
				}
			} else if ( item is Solution ) {
				// doesnt seem to ever happend... 
				Solution sol = (Solution)item;
			} else {
				throw new NotSupportedException( "Unknown project item type: " + item.GetType().FullName );
			}
		}
	}
}
