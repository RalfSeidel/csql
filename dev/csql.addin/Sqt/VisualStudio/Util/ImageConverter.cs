using System.Drawing;
using stdole;

namespace Sqt.VisualStudio.Util
{
	/// <summary>
	/// Translate .NET images into OLE picture objects.
	/// </summary>
	/// <remarks>
	/// This class wraps some methods defined in the AxHost
	/// windows forms classes  of the .NET framework.
	/// </remarks>
	internal static class ImageConverter
	{
		/// <summary>
		/// Convert the given image to an OLE picture object.
		/// </summary>
		public static StdPicture ToPicture( Image image )
		{
			return AxHostWrapper.GetPicture( image );
		}

		public static StdPicture ToPictureMask( Image image, Color mask, Color unmask, params Color[] colorsToMask )
		{
			return AxHostWrapper.GetPictureMask( image, mask, unmask, colorsToMask );
		}

		/// <summary>
		/// Extension of <see cref="T:System.Windows.Forms.AxHost"/> to gain 
		/// access to some of the proctected methods.
		/// </summary>
		private class AxHostWrapper : System.Windows.Forms.AxHost
		{
			public AxHostWrapper()
				: base( "430FC000-2761-4d1d-BCC5-93BE05FDAA27" )
			{
			}

			/// <remarks>
			/// Don't know why Christoph implemented this method.
			/// </remarks>
			[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Don't know why this method is implemented" )]
			public static Image GetImage( object pictureDisp )
			{
				return GetPictureFromIPictureDisp( pictureDisp );
			}

			public static StdPicture GetPicture( Image image )
			{
				var picture = GetIPictureDispFromPicture( image );
				var olePicture = (StdPicture)picture;
				return olePicture;
			}

			public static StdPicture GetPictureMask( Image image, Color mask, Color unmask, params Color[] toMask )
			{
				Image maskImage = ImageHelper.GetMask( image, mask, unmask, toMask );
				object picture = GetIPictureDispFromPicture( maskImage );
				return (StdPicture)picture;
			}
		}
	}
}
