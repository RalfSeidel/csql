using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace csql.addin.Settings
{
	/// <summary>
	/// 
	/// </summary>
	[System.Runtime.InteropServices.ComVisible( true )]
	internal class PreprocessorDefinitionCollectionConverter : TypeConverter
	{
		/// <summary>
		/// Gets a value indicating whether this converter can convert an object in the given source type to a string using the specified context.
		/// </summary>
		/// <param name="context">An <see cref="T:System.ComponentModel.ITypeDescriptorContext"/> that provides a format context.</param>
		/// <param name="sourceType">A <see cref="T:System.Type"/> that represents the type you wish to convert from.</param>
		/// <returns>
		/// true if this converter can perform the conversion; otherwise, false.
		/// </returns>
		public override bool CanConvertFrom( ITypeDescriptorContext context, Type sourceType )
		{
			if ( sourceType == typeof( string ) ) {
				return true;
			}
			else {
				return base.CanConvertFrom( context, sourceType );
			}
		}


		/// <summary>
		/// Convert from a semicolon separated string to a list of preprocessor definitions.
		/// </summary>
		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			string stringValue = value as string;
			if ( stringValue != null ) {
				ICollection<PreprocessorDefinition> result = new List<PreprocessorDefinition>();
				string[] items = stringValue.Split( ';' );

				foreach ( var i in items ) {
					string item = i.Trim();
					if ( item.Length == 0 )
						continue;

					PreprocessorDefinition pd = ConvertStringItemToObject( i );
					result.Add( pd );
				}
				return result;
			}
			else {
				return base.ConvertFrom( context, culture, value );
			}
		}


		/// <inheritdoc/>
		public override bool CanConvertTo( ITypeDescriptorContext context, Type destinationType )
		{
			if ( destinationType == typeof( string ) ) {
				return true;
			}
			else {
				return base.CanConvertTo( context, destinationType );
			}
		}

		/// <summary>
		/// Convert from a list of string to a string of semicolon separated values.
		/// </summary>
		public override object ConvertTo( ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType )
		{
			if ( destinationType == typeof( string ) && value != null ) {
				IEnumerable<PreprocessorDefinition> items = (IEnumerable<PreprocessorDefinition>)value;
				StringBuilder sb = new StringBuilder();
				string separator = "";
				foreach ( var item in items ) {
					if ( !item.IsEnabled )
						continue;

					sb.Append( separator );
					sb.Append( item.Name );
					if ( !String.IsNullOrEmpty( item.Value ) ) {
						sb.Append( '=' );
						sb.Append( item.Value );
					}
					separator = ";";
				}

				foreach ( var item in items ) {
					if ( item.IsEnabled )
						continue;

					sb.Append( separator );
					sb.Append( '!' );
					sb.Append( item.Name );
					if ( !String.IsNullOrEmpty( item.Value ) ) {
						sb.Append( '=' );
						sb.Append( item.Value );
					}
					separator = ";";
				}
				return sb.ToString();
			}
			else {
				return base.ConvertTo( context, culture, value, destinationType );
			}
		}

		/// <summary>
		/// Check if the given type is compatible with IEnumerable&lt;string&gt;
		/// </summary>
		private bool IsEnumerablePreprocessorDefinitionType( Type type )
		{
			if ( !type.IsGenericType )
				return false;

			Type[] genericArguments = type.GetGenericArguments();
			if ( genericArguments.Length != 1 )
				return false;

			Type genericArgument = genericArguments[0];
			if ( genericArgument != typeof( PreprocessorDefinition ) )
				return false;

			Type enumerableType = typeof( IEnumerable<> ).MakeGenericType( genericArguments );
			if ( !enumerableType.IsAssignableFrom( type ) )
				return false;

			return true;
		}


		private PreprocessorDefinition ConvertStringItemToObject( string item )
		{
			string[] nameValue = item.Split( '=' );
			bool enabled = false;
			string name = null;
			string value = null;

			if ( nameValue.Length == 0 )
				return null;

			name = nameValue[0].Trim();
			if ( nameValue.Length > 1 )
				value = nameValue[1].Trim();

			if ( name.StartsWith( "!" ) ) {
				enabled = false;
				name = name.TrimStart( '!' ).Trim();
			}
			else {
				enabled = true;
			}

			PreprocessorDefinition result = new PreprocessorDefinition();
			result.IsEnabled = enabled;
			result.Name = name;
			result.Value = value;


			return result;
		}
	}
}
