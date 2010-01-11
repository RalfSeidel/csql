using System;
using System.Collections.Generic;
using System.Drawing;
using stdole;

namespace csql.addin
{

	/// <summary>
	/// Translate .NET images into OLE picture objects.
	/// </summary>
	/// <remarks>
	/// This class wraps some methods defined in the AxHost
	/// windows forms classes  of the .NET framework.
	/// </remarks>
	public static class ImageTranslator
	{
		private static readonly AxHostWrapper axHostWrapper = new AxHostWrapper();

		/// <summary>
		/// Convert the given image to an OLE picture object.
		/// </summary>
		public static StdPicture GetIPictureDisp( Image image )
		{
			return AxHostWrapper.GetPicture( image );
		}

		public static StdPicture GetIPictureDispMask( Image image, Color mask, Color unmask, params Color[] toMask )
		{
			return AxHostWrapper.GetPictureMask( image, mask, unmask, toMask );
		}

		/// <summary>
		/// Extenstion of <see cref="T:System.Windows.Forms.AxHost"/> to gain 
		/// access to some of the proctected methods.
		/// </summary>
		private class AxHostWrapper : System.Windows.Forms.AxHost
		{
			public AxHostWrapper()
				: base( String.Empty )
			{
			}

			/// <remarks>
			/// Don't knoe why Christoph implemented this methode.
			/// </remarks>
			[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Don't know why this method is implemented" )]
			public static Image GetImage( object pictureDisp )
			{
				return GetPictureFromIPictureDisp( pictureDisp );
			}

			public static StdPicture GetPicture( Image image )
			{
				object picture = GetIPictureDispFromPicture( image );
				return (StdPicture)picture;
			}

			public static StdPicture GetPictureMask( Image image, Color mask, Color unmask, params Color[] toMask )
			{
				Image maskImage = GetMask( image, mask, unmask, toMask );
				object picture = GetIPictureDispFromPicture( image );
				return (StdPicture)picture;
			}

			/// <summary>
			/// Gets the mask.
			/// </summary>
			/// <param name="image">The image to convert to a mask.</param>
			/// <param name="mask">The color used for masking.</param>
			/// <param name="unmask">
			/// The color used when an image pixel has not a masking color.
			/// If the value is <see cref="P:Color.Empty"/> the original color
			/// will be used.
			///.</param>
			/// <param name="toMask">The colors to replace with the mask color.</param>
			/// <returns></returns>
			private static Image GetMask( Image image, Color mask, Color unmask, params Color[] toMask )
			{
				if ( image == null )
					throw new ArgumentNullException( "image" );

				if ( toMask == null )
					throw new ArgumentNullException( "toMask" );

				List<Color> colorsToMaskList = new List<Color>( toMask );
				bool isUnmaskDefined = unmask != Color.Empty;
				Bitmap bitmap = new Bitmap( image );
				EqualColorPredicate equalColorPredicate = new EqualColorPredicate( );

				for ( int y = 0; y < bitmap.Height; y++ ) {
					for ( int x = 0; x < bitmap.Width; x++ ) {
						Color pixel = bitmap.GetPixel( x, y );
						equalColorPredicate.Pixel = pixel;
						
						if ( colorsToMaskList.Exists( equalColorPredicate.Matches ) )
							bitmap.SetPixel( x, y, mask );
						else if ( isUnmaskDefined )
							bitmap.SetPixel( x, y, unmask );
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

				public Color Pixel 
				{
					get { return this.pixel; }
					set { this.pixel = value; } 
				}

				public EqualColorPredicate()
				{
				}

				public bool Matches( Color color )
				{
					return color.R == pixel.R && color.G == pixel.G 
						&& color.B == pixel.B && color.A == pixel.A;
				}
			}
		}
	}
}
