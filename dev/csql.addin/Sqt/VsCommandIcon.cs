using System.Drawing;
using Microsoft.VisualStudio.CommandBars;
using stdole;
using System;

namespace Sqt.VisualStudio
{
	/// <summary>
	/// Visual style properties for the command buttons. 
	/// </summary>
	public class VsCommandIcon
	{
		private static readonly Color defaultTransparencyColor = Color.Magenta;
		private readonly string resourcePath;
		private readonly Color transparencyColor;

		public StdPicture IconPicture
		{
			get 
			{
				if ( String.IsNullOrEmpty( resourcePath )  ) {
					return null;
				} else {
					using ( Image image = ImageHelper.LoadImageResource( resourcePath ) ) {
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
					using ( Image image = ImageHelper.LoadImageResource( resourcePath ) ) {
						StdPicture iconMask = ImageConverter.ToPictureMask( image, System.Drawing.Color.White, System.Drawing.Color.Black, transparencyColor );
						return iconMask;
					}
				}
			}
		}

		public VsCommandIcon( string resourcePath, Color transparencyColor )
		{
			this.resourcePath = resourcePath;
			this.transparencyColor = transparencyColor;
		}

		/// <summary>
		/// Initializes a new <see cref="VsCommandIcon"/>
		/// using the default color for transparency.
		/// </summary>
		/// <param name="image">The icon bitmap.</param>
		public VsCommandIcon( string resourcePath )
			: this( resourcePath, defaultTransparencyColor )
		{
		}
	}
}
