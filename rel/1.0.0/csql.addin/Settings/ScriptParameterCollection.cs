using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;
using System.Diagnostics;
using System.ComponentModel;
using Sqt.DbcProvider;

namespace csql.addin.Settings
{
	[SuppressMessage( "Microsoft.Naming", "CA1722:IdentifiersShouldNotHaveIncorrectPrefix", Justification="The letter C is not a prefix but part of the product name." )]
	[Serializable]
	[XmlRoot( "ScriptParameters" )]
	public sealed class ScriptParameterCollection : IEnumerable<ScriptParameter>
	{
		private readonly List<ScriptParameter> parameters = new List<ScriptParameter>();
		private ScriptParameter currentParameters;
		private DbConnectionParameter dbConnectionParameter;



		/// <summary>
		/// Initializes a new instance of the <see cref="CSqlParameterCollection"/> class.
		/// </summary>
		public ScriptParameterCollection()
		{
		}

		/// <summary>
		/// Gets the number of parameter items in this collection.
		/// </summary>
		[XmlIgnore]
		public int Count
		{
			get { return this.parameters.Count; }
		}

		/// <summary>
		/// Gets or sets the current parameter set used by the application.
		/// </summary>
		public ScriptParameter Current
		{
			get 
			{
				if ( this.parameters.Count == 0 ) {
					this.parameters.Add( new ScriptParameter() );
				}

				if ( this.currentParameters == null ) {
					this.currentParameters = this.parameters[0];
				}
				return this.currentParameters; 
			}
			set 
			{
				Debug.Assert( this.parameters.Contains( value ) );
				this.currentParameters = value; 
			}
		}

		/// <summary>
		/// The database connection parameter associated with the current parameter set.
		/// </summary>
		[Browsable( false )]
		public DbConnectionParameter DbConnection
		{
			get { return this.dbConnectionParameter; }
			set { this.dbConnectionParameter = value; }
		}

		public IEnumerator<ScriptParameter> GetEnumerator()
		{
			return parameters.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		/// <summary>
		/// Adds a parameter to the collection.
		/// </summary>
		/// <remarks>
		/// Note that the method is used by XML serialization as well.
		/// </remarks>
		public void Add( ScriptParameter parameter )
		{
			this.parameters.Add( parameter );
		}

		/// <summary>
		/// Removes the specified parameter from the collection.
		/// </summary>
		public void Remove( ScriptParameter parameter )
		{
			if ( parameter == this.currentParameters ) {
				int index = parameters.IndexOf( parameter );
				if ( index < this.parameters.Count - 2 ) {
					this.currentParameters = this.parameters[index+1];
				} else if ( index > 0 ) {
					this.currentParameters = this.parameters[index-1];
				} else {
					this.currentParameters = null;
				}
			}
			this.parameters.Remove( parameter );
		}
	}
}
