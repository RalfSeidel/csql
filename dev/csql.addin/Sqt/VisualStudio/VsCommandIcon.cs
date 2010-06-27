using Microsoft.VisualStudio.CommandBars;
using stdole;
using System;
using System.Drawing;
using Sqt.VisualStudio.Util;
using ImageConverter=Sqt.VisualStudio.Util.ImageConverter;
using System.Reflection;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// Visual style properties for the command buttons. 
	/// </summary>
	public class VsCommandIcon
	{
		private static readonly Color defaultTransparencyColor = Color.FromArgb( 0, 254, 0 );
		private readonly Assembly resourceAssembly;
		private readonly string resourcePath;
		private readonly Color transparencyColor;

		public StdPicture IconPicture
		{
			get 
			{
				if ( String.IsNullOrEmpty( resourcePath )  ) {
					return null;
				} else {
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
				} else {
					using ( Image image = ImageHelper.LoadEmbeddedImageResource( resourceAssembly, resourcePath ) ) {
						StdPicture iconMask = ImageConverter.ToPictureMask( image, System.Drawing.Color.White, System.Drawing.Color.Black, transparencyColor );
						return iconMask;
					}
				}
			}
		}

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
		/// <param name="image">The icon bitmap.</param>
		public VsCommandIcon( Assembly resourceAssembly, string embeddedResourcePath )
			: this( resourceAssembly, embeddedResourcePath, defaultTransparencyColor )
		{
		}
	}
}
