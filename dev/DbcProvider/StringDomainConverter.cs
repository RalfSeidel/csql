using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Argument for the event raised when the standard values for a field are retrieved.
	/// </summary>
	[CLSCompliant( true )]
	public class StringLookupGetValuesEventArgs : EventArgs
	{
		private readonly ITypeDescriptorContext context;
		private readonly ICollection<string> values;

		public ITypeDescriptorContext Context
		{
			get
			{
				return this.context;
			}
		}

		/// <summary>
		/// The collection to be filled by the event handler.
		/// </summary>
		public ICollection<string> Values
		{
			get
			{
				return this.values;
			}
		}

		/// <summary>
		/// Constructor.
		/// </summary>
		internal StringLookupGetValuesEventArgs( ITypeDescriptorContext context, ICollection<string> values )
		{
			this.context = context;
			this.values = values;
		}

	}

	/// <summary>
	/// A pseudo converter that provides a list of predefined values for a string property.
	/// </summary>
	internal class StringLookupConverter : StringConverter
	{
		public event EventHandler<StringLookupGetValuesEventArgs> GetLookupValues;

		/// <summary>
		/// Default constructor
		/// </summary>
		public StringLookupConverter()
		{
		}

		/// <summary>
		/// Override the base implementation and returns <c>true</c>
		/// </summary>
		public override bool GetStandardValuesSupported( ITypeDescriptorContext context )
		{
			return true;
		}

		/// <summary>
		/// Calls the delegate to retrieve the standard values and returns them to the caller.
		/// </summary>
		public override StandardValuesCollection GetStandardValues( ITypeDescriptorContext context )
		{
			List<string> lookupValues = new List<string>();
			OnGetLookupValues( context, lookupValues );
			StandardValuesCollection result = new StandardValuesCollection( lookupValues );
			return result;
		}

		/// <summary>
		/// Raises the GetLookupValues event.
		/// </summary>
		private void OnGetLookupValues( ITypeDescriptorContext context, ICollection<string> lookupValues )
		{
			if ( this.GetLookupValues != null ) {
				StringLookupGetValuesEventArgs args = new StringLookupGetValuesEventArgs( context, lookupValues );
				GetLookupValues( this, args );
			}
		}
	}
}
