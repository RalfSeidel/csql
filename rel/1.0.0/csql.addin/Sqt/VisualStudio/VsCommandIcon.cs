using System;
using System.Drawing;
using System.Reflection;
using Microsoft.VisualStudio.CommandBars;
using stdole;
using Sqt.VisualStudio.Util;
using ImageConverter = Sqt.VisualStudio.Util.ImageConverter;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// A wrapper class to created image and mask bitmaps for an image in the resources.
	/// </summary>
	public class VsCommandIcon
	{
		private static readonly Color defaultTransparencyColor = Color.FromArgb( 0, 254, 0 );
		private readonly Assembly resourceAssembly;
		private readonly string resourcePath;
		private readonly Color transparencyColor;

		public VsCommandIcon( Assembly resourceAssembly, string resourcePath, Color transparencyColor )
		{
			this.resourceAssembly = resourceAssembly;
			this.resourcePath = resourcePath;
			this.transparencyColor = transparencyColor;
		}

		/// <summary>
		/// Initializes a new <see cref="VsCommandIcon"/>
		/// using the default color for transparency.
		/// </summary>
		public VsCommandIcon( Assembly resourceAssembly, string embeddedResourcePath )
			: this( resourceAssembly, embeddedResourcePath, defaultTransparencyColor )
		{
		}


		public StdPicture IconPicture
		{
			get 
			{
				if ( String.IsNullOrEmpty( resourcePath ) ) {
					return null;
				}
				else {
					using ( Image image = ImageHelper.LoadEmbeddedImageResource( resourceAssembly, resourcePath ) ) {
						StdPicture iconPicture = ImageConverter.ToPicture( image );
						return iconPicture;
					}
				}
			}
		}

		public StdPicture IconMask
		{
			get 
			{
				if ( String.IsNullOrEmpty( resourcePath ) ) {
					return null;
				}
				else {
					using ( Image image = ImageHelper.LoadEmbeddedImageResource( resourceAssembly, resourcePath ) ) {
						StdPicture iconMask = ImageConverter.ToPictureMask( image, System.Drawing.Color.White, System.Drawing.Color.Black, transparencyColor );
						return iconMask;
					}
				}
			}
		}
	}
}
