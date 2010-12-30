using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Sqt.DbcProvider
{
	/// <summary>
	/// Services and meta information for the MruConnections schema
	/// </summary>
	public partial class MruConnections
	{
		/// <summary>
		/// Gets the xml schema of the mru connections defintion.
		/// </summary>
		/// <value>The xml schema that defines the structure of this object.</value>
		[XmlIgnore]
		public static XmlSchema Schema
		{
			get
			{
				Type thisType = typeof( MruConnections );
				string resourceId = thisType.Namespace + ".MruConnections.xsd";
				Assembly assembly = Assembly.GetExecutingAssembly();
				Stream resourceStream = assembly.GetManifestResourceStream( resourceId );
				XmlReader xmlReader = XmlReader.Create( resourceStream );
				XmlSchema xmlSchema = XmlSchema.Read( xmlReader, (s, e ) => Trace.WriteLine( e.ToString() ) );
				return xmlSchema;
			}
		}



		/// <summary>
		/// Create a new instances deserialize from an XML file.
		/// </summary>
		/// <param name="filePath">The file path.</param>
		/// <returns>The loaded object.</returns>
		public static MruConnections LoadFromFile( string filePath )
		{
			using ( Stream stream = new FileStream( filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, FileOptions.SequentialScan ) ) 
			using ( TextReader reader = new StreamReader( stream ) ) {
				return LoadFromTextReader( reader );
			}
		}

		/// <summary>
		/// Create a new instances deserialize from text containing the xml document.
		/// </summary>
		/// <param name="xmlContent">The serialized XML content.</param>
		/// <returns>The loaded object.</returns>
		public static MruConnections LoadFromString( string xmlContent )
		{
			using ( StringReader reader = new StringReader( xmlContent ) ) {
				return LoadFromTextReader( reader );
			}
		}

		private static MruConnections LoadFromTextReader( TextReader reader )
		{
			XmlSchema xmlSchema = MruConnections.Schema;
			XmlReaderSettings xmlReaderSettings = new XmlReaderSettings();
			xmlReaderSettings.Schemas.Add( xmlSchema );
			xmlReaderSettings.ValidationType = ValidationType.Schema;
			using ( XmlReader xmlReader = XmlReader.Create( reader, xmlReaderSettings ) ) {
				XmlSerializer serializer = new XmlSerializer( typeof( MruConnections ), xmlSchema.TargetNamespace );
				object o = serializer.Deserialize( xmlReader );
				MruConnections mruConnections = (MruConnections)o;

				return mruConnections;
			}
		}

		/// <summary>
		/// Find the datasources stored in the connection history for the specified provider.
		/// </summary>
		/// <param name="provider">The id of the provider.</param>
		/// <returns>
		/// The most recently used datasources used for the specified provider or <c>null</c>
		/// if the history does not contain any record for it.
		/// </returns>
		public Datasources FindDatasources( ProviderType provider )
		{
			foreach ( var ds in this.Datasources ) {
				if ( ds.Provider == provider )
					return ds;
			}
			return null;
		}

		/// <summary>
		/// Find the datasource with the specified address (i.e. server name).
		/// </summary>
		/// <param name="provider">The id of the provider.</param>
		/// <param name="datasourceAddress">The name of the datasource.</param>
		/// <param name="datasourcePort">The server connection ip port or zero if not used.</param>
		/// <returns>
		/// The datasource for the specified provider and address or <c>null</c>
		/// if the history does not contain any record for it.
		/// </returns>
		public Datasource FindDatasourceByAddress( ProviderType provider, string datasourceAddress, int datasourcePort )
		{
			Datasources datasourcesOfProvider = FindDatasources( provider );
			if ( datasourcesOfProvider == null )
				return null;

			Datasource datasource = datasourcesOfProvider.FindDatasourceByAddress( datasourceAddress, datasourcePort );
			return datasource;
		}


		/// <summary>
		/// Finds the datasource by the user specified comment.
		/// </summary>
		/// <param name="provider">The id of the provider.</param>
		/// <param name="datasourceComment">The comment for the datasource.</param>
		/// <returns>
		/// The datasource for the specified provider and comment or <c>null</c>
		/// if the history does not contain any record for it.
		/// </returns>
		public Datasource FindDatasourceByComment( ref ProviderType provider, string datasourceComment )
		{
			// First lookup the datasource for the current provider.
			Datasources datasourcesOfProvider = FindDatasources( provider );
			if ( datasourcesOfProvider == null )
				return null;

			Datasource datasource = datasourcesOfProvider.FindDatasourceByComment( datasourceComment );
			if ( datasource != null )
				return datasource;

			// If the datasource was not found seek all datasources and return the first matching entry.
			foreach ( var datasources in this.Datasources ) {
				datasource = datasources.FindDatasourceByComment( datasourceComment );
				if ( datasource != null ) {
					provider = datasources.Provider;
					return datasource;
				}
			}

			return null;
		}
	}
}