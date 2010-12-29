using System;
using System.Globalization;
using System.Xml.Serialization;
using System.Text;

namespace Sqt.DbcProvider
{
	public partial class Authentication
	{
		/// <summary>
		/// Gets a value indicating whether the authentication data is usable to try to connect to a database.
		/// </summary>
		[XmlIgnore()]
		public bool IsUsable
		{
			get
			{
				if ( Integrated )
					return true;

				if ( String.IsNullOrEmpty( UserId ) )
					return false;

				return true;
			}
		}


		#region Common Object Overrides

		/// <inheritdoc/>
		public override string ToString()
		{
			if ( this.Integrated ) {
				return "Integrated Security";
			}
			StringBuilder sb = new StringBuilder();
			sb.Append( "User=" ).Append( this.UserId ?? "[null]" );
			sb.Append( ", Password=" ).Append( this.Password ?? "[null]" );
			return sb.ToString();
		}

		/// <inheritdoc/>
		public override bool Equals( object obj )
		{
			if ( obj == null || GetType() != obj.GetType() ) {
				return false;
			}

			Authentication that = (Authentication)obj;

			if ( this.Integrated != that.Integrated )
				return false;

			if ( !String.Equals(this.UserId, that.UserId, StringComparison.CurrentCultureIgnoreCase ) )
				return false;

			if ( !String.Equals(this.Password, that.Password ) )
				return false;

			return true;
		}

		/// <inheritdoc/>
		public override int GetHashCode()
		{
			unchecked {
				int hashCode = this.Integrated ? 1 : 0;
				if ( UserId != null )
					hashCode = hashCode * 31 + this.UserId.ToLower( CultureInfo.CurrentCulture ).GetHashCode();
				if ( Password != null )
					hashCode = hashCode * 31 + this.Password.GetHashCode();
				return hashCode;
			}
		}

		#endregion
	}
}
