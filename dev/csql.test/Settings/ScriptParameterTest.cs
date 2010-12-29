using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace csql.addin.Settings
{
	/// <summary>
	/// Unit tests for <see cref="T:CSqlParameter"/>
	/// </summary>
	[TestClass]
	public class ScriptParameterTest
	{
		[TestMethod]
		public void SerializeIncludeDirectoriesTest()
		{
			ScriptParameter parameterIn = new ScriptParameter();

			parameterIn.IncludeDirectories = new List<string>() { "A", "B" };
			ScriptParameter parameterOut = CloneParameterBySerialization( parameterIn );

			Assert.AreEqual( 2, parameterOut.IncludeDirectories.Count );
			Assert.AreEqual( "A", parameterOut.IncludeDirectories[0] );
			Assert.AreEqual( "B", parameterOut.IncludeDirectories[1] );
		}


		private ScriptParameter CloneParameterBySerialization( ScriptParameter parameter )
		{
			XmlSerializer serializer = new XmlSerializer( parameter.GetType() );
			using ( Stream stream = new MemoryStream() ) {
				serializer.Serialize( stream, parameter );
				stream.Seek( 0, SeekOrigin.Begin );
				object clone = serializer.Deserialize( stream );
				return (ScriptParameter)clone;
			}
		}
	}
}
