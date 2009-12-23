using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace csql.addin.Gui.Controls
{
	/// <summary>
	/// A adorner class host a TextBox to enable editing and the TextBox lies in the AdornerLayer,
	/// when EditBox is in editable mode, arrange it with desired size; otherwise, arrange it with 
	/// size(0,0,0,0).
	/// </summary>
	internal sealed class GridViewEditBoxAdorner : Adorner
	{
		/// <summary>
		/// Exstra space for TextBox to make the text in it clear.
		/// </summary>
		private const double PaddingWidth = 15;

		/// <summary>
		/// Visual children.
		/// </summary>
		private readonly VisualCollection visualChildren;

		/// <summary>
		/// The canvas to contain the TextBox to enable it can expand out of cell.
		/// </summary>
		private readonly Canvas canvas;

		/// <summary>
		/// The TextBox this adorner needs to cover.
		/// </summary>
		private readonly TextBox textBox;

		/// <summary>
		/// Whether is in editable mode.
		/// </summary>
		private bool isVisible;

		/// <summary>
		/// Inialize the EditBoxAdorner.
		/// </summary>
		public GridViewEditBoxAdorner( UIElement adornedElement, UIElement adorningElement )
			: base( adornedElement )
		{
			this.textBox = adorningElement as TextBox;
			Debug.Assert( textBox != null, "No TextBox!" );

			this.canvas = new Canvas();
			this.visualChildren = new VisualCollection( this );

			BuildTextBox();
		}

		/// <summary>
		/// override property to return infomation about visual tree.
		/// </summary>
		protected override int VisualChildrenCount { get { return visualChildren.Count; } }

		/// <summary>
		/// override function to return infomation about visual tree.
		/// </summary>
		protected override Visual GetVisualChild( int index ) { return visualChildren[index]; }

		/// <summary>
		/// Public method for EditBox to update staus of TextBox when IsEditing property is changed.
		/// </summary>
		/// <param name="isVisislbe"></param>
		public void UpdateVisibilty( bool isVisislbe )
		{
			isVisible = isVisislbe;
			InvalidateMeasure();
		}

		/// <summary>
		/// Override to measure elements.
		/// </summary>
		protected override Size MeasureOverride( Size constraint )
		{
			textBox.IsEnabled = isVisible;
			if ( isVisible ) {
				// If in editable mode, measure the space the adorner element should cover.
				AdornedElement.Measure( constraint );
				textBox.Measure( constraint );

				//since the adorner is to cover the EditBox, it should return the AdornedElement.Width, 
				//the extra 15 is to make it more clear.
				return new Size( AdornedElement.DesiredSize.Width + PaddingWidth, textBox.DesiredSize.Height );
			} else {
				//if it is not in editable mode, no need to show anything.
				return new Size( 0, 0 );
			}
		}

		/// <summary>
		/// Override function to arrange elements.
		/// </summary>
		protected override Size ArrangeOverride( Size finalSize )
		{
			if ( isVisible ) {
				textBox.Arrange( new Rect( 0, 0, finalSize.Width, finalSize.Height ) );
			} else {
				// if is not is editable mode, no need to show elements.
				textBox.Arrange( new Rect( 0, 0, 0, 0 ) );
			}
			return finalSize;
		}

		/// <summary>
		/// Inialize necessary properties and hook necessary events on TextBox, then add it into tree.
		/// </summary>
		private void BuildTextBox()
		{
			this.canvas.Children.Add( textBox );
			this.visualChildren.Add( this.canvas );

			//Bind Text onto AdornedElement.
			Binding binding = new Binding( "Text" );
			binding.Mode = BindingMode.TwoWay;
			binding.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
			binding.Source = this.AdornedElement;

			this.textBox.SetBinding( TextBox.TextProperty, binding );

			// Update TextBox's focus status when layout finishs.
			this.textBox.LayoutUpdated += new EventHandler( OnTextBoxLayoutUpdated );
		}

		/// <summary>
		/// When Layout finish, if in editable mode, update focus status on TextBox.
		/// </summary>
		private void OnTextBoxLayoutUpdated( object sender, EventArgs e )
		{
			if ( isVisible )
				this.textBox.Focus();
		}

	}
}
