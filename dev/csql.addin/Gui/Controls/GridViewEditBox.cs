using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace csql.addin.Gui.Controls
{
	/// <summary>
	/// EditBox is a customized cotrol that can switch between two modes: editing and normal.
	/// when it is in editing mode, a TextBox will show up to enable editing. When in normal mode, it 
	/// displays content as a TextBlock.
	/// 
	/// This control can only be used in GridView to enable editing.
	/// </summary>
	public class GridViewEditBox : Control
	{
		/// <summary>
		/// The AdornerLayer on the TextBlock
		/// </summary>
		private GridViewEditBoxAdorner adorner;

		/// <summary>
		/// TextBox in visual tree.
		/// </summary>
		private FrameworkElement textBox;

		/// <summary>
		/// Whether EditBox can switch into Editable mode. If the ListViewItem that contain the EditBox is selected, when mouse enter the EditBox, it becomes true
		/// </summary>
		private bool canEdit = false;
		private bool isMouseWithinScope = false; //whether can swithc into Editable mode. If the ListViewItem that contain the EditBox is selected, when mouse is over the EditBox, it becomes true.
		private ItemsControl itemsControl; //The ListView that contains it.
		private ListViewItem listViewItem; //The ListViewItem that contains it.

		/// <summary>
		/// ValueProperty DependencyProperty.
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
				DependencyProperty.Register(
						"Value",
						typeof( object ),
						typeof( GridViewEditBox ),
						new FrameworkPropertyMetadata( null ) );

		/// <summary>
		/// The value of the EditBox
		/// </summary>
		public object Value
		{
			get { return GetValue( ValueProperty ); }
			set { SetValue( ValueProperty, value ); }
		}

		/// <summary>
		/// IsEditingProperty DependencyProperty
		/// </summary>
		public static DependencyProperty IsEditingProperty =
				DependencyProperty.Register(
						"IsEditing",
						typeof( bool ),
						typeof( GridViewEditBox ),
						new FrameworkPropertyMetadata( false ) );


		/// <summary>
		/// True if the control is in editing mode
		/// </summary>
		public bool IsEditing
		{
			get { return (bool)GetValue( IsEditingProperty ); }

			private set
			{
				SetValue( IsEditingProperty, value );
				adorner.UpdateVisibilty( value );
			}
		}


		/// <summary>
		/// Whether the ListViewItem that contains it is selected.
		/// </summary>
		private bool IsParentSelected
		{
			get
			{
				return listViewItem != null && listViewItem.IsSelected;
			}
		}


		/// <summary>
		/// Static constructor
		/// </summary>
		static GridViewEditBox()
		{
			DefaultStyleKeyProperty.OverrideMetadata( typeof( GridViewEditBox ), new FrameworkPropertyMetadata( typeof( GridViewEditBox ) ) );
		}


		/// <summary>
		/// Called when the Template's tree has been generated
		/// </summary>
		public override void OnApplyTemplate()
		{
			base.OnApplyTemplate();

			TextBlock textBlock = GetTemplateChild( "PART_TextBlockPart" ) as TextBlock;
			Debug.Assert( textBlock != null, "No TextBlock!" );

			this.textBox = new TextBox();
			this.adorner = new GridViewEditBoxAdorner( textBlock, textBox );
			AdornerLayer layer = AdornerLayer.GetAdornerLayer( textBlock ); ;
			layer.Add( adorner );

			this.textBox.KeyDown += new KeyEventHandler( OnTextBoxKeyDown );
			this.textBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler( OnTextBoxLostKeyboardFocus );

			//hook resize event to handle the the column resize. 
			HookTemplateParentResizeEvent();

			//hook the resize event to  handle ListView resize cases.
			HookItemsControlEvents();

			this.listViewItem = GetDependencyObjectFromVisualTree( this, typeof( ListViewItem ) ) as ListViewItem;
			Debug.Assert( listViewItem != null, "No ListViewItem found" );
		}


		/// <summary>
		/// If the ListView that contains EditBox is selected, when Mouse enters it, it can switch to editale mode now.
		/// </summary>
		protected override void OnMouseEnter( MouseEventArgs e )
		{
			base.OnMouseEnter( e );
			if ( !IsEditing && IsParentSelected ) {
				canEdit = true;
			}
		}

		/// <summary>
		/// If mouse leave it, no matter wheher the ListViewItem that contains it is selected or not, 
		/// it can not switch into Editable mode.
		/// </summary>
		protected override void OnMouseLeave( MouseEventArgs e )
		{
			base.OnMouseLeave( e );
			isMouseWithinScope = false;
			canEdit = false;
		}

		/// <summary>
		/// When ListViewItem that contains EditBox is selected and this event happens, if one of the following conditions is satisified,
		/// the EditBox will be switched into editable mode.
		/// 1. A MouseEnter happened before this. 
		/// 2. Mouse never move out of it since the ListViewItem was selected.
		/// </summary>
		protected override void OnMouseUp( MouseButtonEventArgs e )
		{
			base.OnMouseUp( e );

			if ( e.ChangedButton == MouseButton.Right || e.ChangedButton == MouseButton.Middle )
				return;

			if ( !IsEditing ) {
				if ( !e.Handled && (canEdit || isMouseWithinScope) ) {
					IsEditing = true;
				}

				//Handle a specific case: After a ListViewItem was selected by clicking it,
				// Clicking the EditBox again should switch into Editable mode.
				if ( IsParentSelected )
					isMouseWithinScope = true;
			}
		}


		/// <summary>
		/// When in editable mode, Pressing Enter Key and F2 Key make it switch into normal model
		/// </summary>
		private void OnTextBoxKeyDown( object sender, KeyEventArgs e )
		{
			if ( IsEditing && (e.Key == Key.Enter || e.Key == Key.F2) ) {
				IsEditing = false;
				canEdit = false;
			}
		}

		/// <summary>
		/// When in editable mode, losing focus make it switch into normal mode.
		/// </summary>
		private void OnTextBoxLostKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
		{
			IsEditing = false;
		}

		/// <summary>
		/// Set IsEditing to false when parent has changed its size
		/// </summary>
		private void OnCouldSwitchToNormalMode( object sender, RoutedEventArgs e )
		{
			IsEditing = false;
		}

		/// <summary>
		/// Walk the visual tree to find the ItemsControl and hook its some events on it.
		/// </summar
		private void HookItemsControlEvents()
		{
			itemsControl = GetDependencyObjectFromVisualTree( this, typeof( ItemsControl ) ) as ItemsControl;
			if ( itemsControl != null ) {
				//The reason of hooking Resize/ScrollChange/MouseWheel event is :
				//when one of these event happens, the EditBox should be switched into editable mode.
				itemsControl.SizeChanged += new SizeChangedEventHandler( OnCouldSwitchToNormalMode );
				itemsControl.AddHandler( ScrollViewer.ScrollChangedEvent, new RoutedEventHandler( OnScrollViewerChanged ) );
				itemsControl.AddHandler( ScrollViewer.MouseWheelEvent, new RoutedEventHandler( OnCouldSwitchToNormalMode ), true );
			}
		}

		/// <summary>
		/// If EditBox is in editable mode, scrolling ListView should switch it into normal mode.
		/// </summary>
		private void OnScrollViewerChanged( object sender, RoutedEventArgs args )
		{
			if ( IsEditing && Mouse.PrimaryDevice.LeftButton == MouseButtonState.Pressed ) {
				IsEditing = false;
			}
		}

		/// <summary>
		/// Walk visual tree to find the first DependencyObject of the specific type.
		/// </summary>
		private DependencyObject GetDependencyObjectFromVisualTree( DependencyObject startObject, Type type )
		{
			for ( DependencyObject parent = startObject; parent != null; parent = VisualTreeHelper.GetParent( parent ) ) {
				if ( type.IsInstanceOfType( parent ) )
					return parent;
			}

			return null;
		}

		/// <summary>
		/// Get the TemplatedParent and hook its resize event. 
		/// The reason of hooking this event is that when resize a column, the EditBox should switch editable mode.
		/// </summary>
		private void HookTemplateParentResizeEvent()
		{
			FrameworkElement parent = TemplatedParent as FrameworkElement;
			if ( parent != null ) {
				parent.SizeChanged += new SizeChangedEventHandler( OnCouldSwitchToNormalMode );
			}
		}
	}
}
