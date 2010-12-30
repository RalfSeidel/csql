using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.addin.Settings
{
	/// <summary>
	/// Summary description for CSqlParameterCollectionTest
	/// </summary>
	[TestClass]
	public class CSqlParameterCollectionTest
	{
		[TestMethod]
		public void SerializeDeserializeEmpty()
		{
			ScriptParameterCollection collection = new ScriptParameterCollection();
			ScriptParameterCollection collectionClone; 

			XmlSerializer serializer = new XmlSerializer( typeof( ScriptParameterCollection ) );
			using ( Stream stream = new MemoryStream() ) {
				serializer.Serialize( stream, collection );

				stream.Seek( 0, SeekOrigin.Begin );
				object clone = serializer.Deserialize( stream );
				collectionClone = (ScriptParameterCollection)clone;
			}
			Assert.AreEqual( 0, collectionClone.Count );
		}

		[TestMethod]
		public void SerializeDeserializeOne()
		{
			ScriptParameterCollection collection = new ScriptParameterCollection();
			collection.Add( new ScriptParameter() { Name = "A" } );
			collection.Add( new ScriptParameter() { Name = "B" } );
			ScriptParameterCollection collectionClone;

			XmlSerializer serializer = new XmlSerializer( typeof( ScriptParameterCollection ) );
			using ( Stream stream = new MemoryStream() ) {
				serializer.Serialize( stream, collection );

				stream.Seek( 0, SeekOrigin.Begin );
				object clone = serializer.Deserialize( stream );
				collectionClone = (ScriptParameterCollection)clone;
			}
			Assert.AreEqual( collection.Count, collectionClone.Count );
		}
	}
}
