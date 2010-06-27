// ------------------------------------------------------------------------------
//  <auto-generated>
//    Generated by Xsd2Code. Version 3.0.0.16749
//    <NameSpace>Sqt.DbcProvider</NameSpace><Collection>List</Collection><codeType>CSharp</codeType><EnableDataBinding>True</EnableDataBinding><HidePrivateFieldInIDE>True</HidePrivateFieldInIDE><EnableSummaryComment>True</EnableSummaryComment><IncludeSerializeMethod>False</IncludeSerializeMethod><UseBaseClass>True</UseBaseClass><GenerateCloneMethod>False</GenerateCloneMethod><GenerateDataContracts>False</GenerateDataContracts><CodeBaseTag>Net35</CodeBaseTag><SerializeMethodName>Serialize</SerializeMethodName><DeserializeMethodName>Deserialize</DeserializeMethodName><SaveToFileMethodName>SaveToFile</SaveToFileMethodName><LoadFromFileMethodName>LoadFromFile</LoadFromFileMethodName><GenerateXMLAttributes>True</GenerateXMLAttributes><AutomaticProperties>False</AutomaticProperties><DisableDebug>False</DisableDebug><CustomUsings></CustomUsings>
//  </auto-generated>
// ------------------------------------------------------------------------------
namespace Sqt.DbcProvider
{
	using System;
	using System.Diagnostics;
	using System.Xml.Serialization;
	using System.Collections;
	using System.Xml.Schema;
	using System.ComponentModel;
	using System.Collections.Generic;


	#region Base entity class
	public partial class EntityBase<T> : System.ComponentModel.INotifyPropertyChanged
	{

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public virtual void OnPropertyChanged( string info )
		{
			PropertyChangedEventHandler handler = PropertyChanged;
			if ( handler != null ) {
				handler( this, new PropertyChangedEventArgs( info ) );
			}
		}
	}
	#endregion

	[System.CodeDom.Compiler.GeneratedCodeAttribute( "System.Xml", "2.0.50727.4927" )]
	[System.Xml.Serialization.XmlTypeAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd" )]
	[System.Xml.Serialization.XmlRootAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd", IsNullable=false )]
	public partial class MruConnections : EntityBase<MruConnections>
	{

		[EditorBrowsable( EditorBrowsableState.Never )]
		private List<Datasources> datasourcesField;

		[System.Xml.Serialization.XmlElementAttribute( "Datasources" )]
		public List<Datasources> Datasources
		{
			get
			{
				if ( (this.datasourcesField == null) ) {
					this.datasourcesField = new List<Datasources>();
				}
				return this.datasourcesField;
			}
			set
			{
				if ( (this.datasourcesField != null) ) {
					if ( (datasourcesField.Equals( value ) != true) ) {
						this.datasourcesField = value;
						this.OnPropertyChanged( "Datasources" );
					}
				} else {
					this.datasourcesField = value;
					this.OnPropertyChanged( "Datasources" );
				}
			}
		}
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute( "System.Xml", "2.0.50727.4927" )]
	[System.Xml.Serialization.XmlTypeAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd" )]
	public partial class Datasources : EntityBase<Datasources>
	{

		[EditorBrowsable( EditorBrowsableState.Never )]
		private List<Datasource> datasourceField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private ProviderType providerField;

		[System.Xml.Serialization.XmlElementAttribute( "Datasource" )]
		public List<Datasource> Datasource
		{
			get
			{
				if ( (this.datasourceField == null) ) {
					this.datasourceField = new List<Datasource>();
				}
				return this.datasourceField;
			}
			set
			{
				if ( (this.datasourceField != null) ) {
					if ( (datasourceField.Equals( value ) != true) ) {
						this.datasourceField = value;
						this.OnPropertyChanged( "Datasource" );
					}
				} else {
					this.datasourceField = value;
					this.OnPropertyChanged( "Datasource" );
				}
			}
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public ProviderType Provider
		{
			get
			{
				return this.providerField;
			}
			set
			{
				if ( (providerField.Equals( value ) != true) ) {
					this.providerField = value;
					this.OnPropertyChanged( "Provider" );
				}
			}
		}
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute( "System.Xml", "2.0.50727.4927" )]
	[System.Xml.Serialization.XmlTypeAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd" )]
	public partial class Datasource : EntityBase<Datasource>
	{

		[EditorBrowsable( EditorBrowsableState.Never )]
		private List<Authentication> authenticationsField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private List<string> catalogsField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private string addressField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private int portField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private string commentField;

		public Datasource()
		{
			this.portField = 0;
			this.commentField = "";
		}

		[System.Xml.Serialization.XmlArrayItemAttribute( IsNullable=false )]
		public List<Authentication> Authentications
		{
			get
			{
				if ( (this.authenticationsField == null) ) {
					this.authenticationsField = new List<Authentication>();
				}
				return this.authenticationsField;
			}
			set
			{
				if ( (this.authenticationsField != null) ) {
					if ( (authenticationsField.Equals( value ) != true) ) {
						this.authenticationsField = value;
						this.OnPropertyChanged( "Authentications" );
					}
				} else {
					this.authenticationsField = value;
					this.OnPropertyChanged( "Authentications" );
				}
			}
		}

		[System.Xml.Serialization.XmlArrayItemAttribute( "Catalog", IsNullable=false )]
		public List<string> Catalogs
		{
			get
			{
				if ( (this.catalogsField == null) ) {
					this.catalogsField = new List<string>();
				}
				return this.catalogsField;
			}
			set
			{
				if ( (this.catalogsField != null) ) {
					if ( (catalogsField.Equals( value ) != true) ) {
						this.catalogsField = value;
						this.OnPropertyChanged( "Catalogs" );
					}
				} else {
					this.catalogsField = value;
					this.OnPropertyChanged( "Catalogs" );
				}
			}
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		public string Address
		{
			get
			{
				return this.addressField;
			}
			set
			{
				if ( (this.addressField != null) ) {
					if ( (addressField.Equals( value ) != true) ) {
						this.addressField = value;
						this.OnPropertyChanged( "Address" );
					}
				} else {
					this.addressField = value;
					this.OnPropertyChanged( "Address" );
				}
			}
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		[System.ComponentModel.DefaultValueAttribute( 0 )]
		public int Port
		{
			get
			{
				return this.portField;
			}
			set
			{
				if ( (portField.Equals( value ) != true) ) {
					this.portField = value;
					this.OnPropertyChanged( "Port" );
				}
			}
		}

		[System.Xml.Serialization.XmlAttributeAttribute()]
		[System.ComponentModel.DefaultValueAttribute( "" )]
		public string Comment
		{
			get
			{
				return this.commentField;
			}
			set
			{
				if ( (this.commentField != null) ) {
					if ( (commentField.Equals( value ) != true) ) {
						this.commentField = value;
						this.OnPropertyChanged( "Comment" );
					}
				} else {
					this.commentField = value;
					this.OnPropertyChanged( "Comment" );
				}
			}
		}
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute( "System.Xml", "2.0.50727.4927" )]
	[System.Xml.Serialization.XmlTypeAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd" )]
	public partial class Authentication : EntityBase<Authentication>
	{

		[EditorBrowsable( EditorBrowsableState.Never )]
		private bool integratedField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private string userIdField;

		[EditorBrowsable( EditorBrowsableState.Never )]
		private string passwordField;

		public Authentication()
		{
			this.integratedField = true;
			this.userIdField = "";
			this.passwordField = "";
		}

		/// <summary>
		/// Option to use integrated security/windows authentication.
		/// </summary>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		[System.ComponentModel.DefaultValueAttribute( true )]
		public bool Integrated
		{
			get
			{
				return this.integratedField;
			}
			set
			{
				if ( (integratedField.Equals( value ) != true) ) {
					this.integratedField = value;
					this.OnPropertyChanged( "Integrated" );
				}
			}
		}

		/// <summary>
		/// The user id to use if login is done using the server authentication.
		/// </summary>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		[System.ComponentModel.DefaultValueAttribute( "" )]
		public string UserId
		{
			get
			{
				return this.userIdField;
			}
			set
			{
				if ( (this.userIdField != null) ) {
					if ( (userIdField.Equals( value ) != true) ) {
						this.userIdField = value;
						this.OnPropertyChanged( "UserId" );
					}
				} else {
					this.userIdField = value;
					this.OnPropertyChanged( "UserId" );
				}
			}
		}

		/// <summary>
		/// The password to use if login is done using the server authentication.
		/// </summary>
		[System.Xml.Serialization.XmlAttributeAttribute()]
		[System.ComponentModel.DefaultValueAttribute( "" )]
		public string Password
		{
			get
			{
				return this.passwordField;
			}
			set
			{
				if ( (this.passwordField != null) ) {
					if ( (passwordField.Equals( value ) != true) ) {
						this.passwordField = value;
						this.OnPropertyChanged( "Password" );
					}
				} else {
					this.passwordField = value;
					this.OnPropertyChanged( "Password" );
				}
			}
		}
	}

	[System.CodeDom.Compiler.GeneratedCodeAttribute( "System.Xml", "2.0.50727.4927" )]
	[System.Xml.Serialization.XmlTypeAttribute( Namespace="http://schemas.sql-service.de/etc/MruConnections.xsd" )]
	public enum ProviderType
	{

		/// <remarks/>
		[Browsable(false)]
		Undefined,

		/// <remarks/>
		MsSql,

		/// <remarks/>
		Sybase,

		/// <remarks/>
		[Browsable( false )]
		Oracle,

		/// <remarks/>
		[Browsable( false )]
		IbmDb2,
	}
}
