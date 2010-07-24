using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

namespace Sqt.VisualStudio.Util
{
	/// <summary>
	/// Helpers for images and bitmap handling.
	/// </summary>
	internal static class ImageHelper
	{
		/// <summary>
		/// Loads an embedded resource image.
		/// </summary>
		/// <param name="resourceAssembly">The resource assembly usually the assembly of the code that calls this method.</param>
		/// <param name="resourcePath">The resource path.</param>
		/// <returns></returns>
		internal static Image LoadEmbeddedImageResource( Assembly resourceAssembly, string resourcePath )
		{
			if ( String.IsNullOrEmpty( resourcePath ) ) 
				throw new ArgumentNullException( "resourcePath" );

			Stream stream = resourceAssembly.GetManifestResourceStream( resourcePath );
			if ( stream == null ) {
				throw new ArgumentException( "Can't find the resource " + resourcePath, "resourcePath" );
			}
			Image bitmap = Image.FromStream( stream );
			return bitmap;
		}


		/// <summary>
		/// Masks an image and returns a two color bitmap.
		/// </summary>
		/// <param name="image">The image to convert to a maskColor.</param>
		/// <param name="maskColor">The color used for masking.</param>
		/// <param name="unmaskColor">
		/// The color used when an image pixel has not a masking color.
		/// If the value is <see cref="P:Color.Empty"/> the original color
		/// will be used.
		/// </param>
		/// <param name="colorsToMask">The colors to replace with the maskColor color.</param>
		/// <returns></returns>
		internal static Image GetMask( Image image, Color maskColor, Color unmaskColor, params Color[] colorsToMask )
		{
			if ( image == null )
				throw new ArgumentNullException( "image" );

			if ( colorsToMask == null )
				throw new ArgumentNullException( "colorsToMask" );

			List<Color> colorsToMaskList = new List<Color>( colorsToMask );
			bool isUnmaskDefined = unmaskColor != Color.Empty;
			Bitmap bitmap = new Bitmap( image );
			EqualColorPredicate equalColorPredicate = new EqualColorPredicate();

			for ( int y = 0; y < bitmap.Height; y++ ) {
				for ( int x = 0; x < bitmap.Width; x++ ) {
					Color pixel = bitmap.GetPixel( x, y );
					equalColorPredicate.Pixel = pixel;

					if ( colorsToMaskList.Exists( equalColorPredicate.Matches ) )
						bitmap.SetPixel( x, y, maskColor );
					else if ( isUnmaskDefined )
						bitmap.SetPixel( x, y, unmaskColor );
				}
			}

			return bitmap;
		}

		/// <summary>
		/// Helper predicate to check color equality.
		/// </summary>
		/// <remarks>
		/// Its not possibile to use <see cref="M:Color.Equals"/>
		/// because the methode returns <c>false</c> if one of the colors
		/// compared is a named color and the other not, even if their
		/// RGB and alpha values are the same.
		/// </remarks>
		private class EqualColorPredicate
		{
			private Color pixel;

			public EqualColorPredicate()
			{
			}

			public Color Pixel
			{
				get { return this.pixel; }
				set { this.pixel = value; }
			}

			public bool Matches( Color color )
			{
				return color.R == pixel.R && color.G == pixel.G 
						&& color.B == pixel.B && color.A == pixel.A;
			}
		}

	}
}
