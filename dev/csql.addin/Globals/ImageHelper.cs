using System;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace csql.addin.Globals
{
	/// <summary>
	/// Helpers for images and bitmap handling.
	/// </summary>
	static class ImageHelper
	{
		internal static Image LoadImageResource( string resourcePath )
		{
			if ( String.IsNullOrEmpty( resourcePath ) ) 
				throw new ArgumentNullException( "resourcePath" );

			Assembly resourceAssembly = Assembly.GetExecutingAssembly();
			Stream stream = resourceAssembly.GetManifestResourceStream( resourcePath );
			if ( stream == null ) {
				throw new ArgumentException( "Can't find the resource " + resourcePath, "resourcePath" );
			}
			Image bitmap = Image.FromStream( stream );
			return bitmap;
		}
	}
}
