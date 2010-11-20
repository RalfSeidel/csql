using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml.Serialization;
using System.IO;

namespace csql.addin.Settings
{
	/// <summary>
	/// Unit tests for <see cref="T:CSqlParameter"/>
	/// </summary>
	[TestClass]
	public class CSqlParameterTest
	{
		[TestMethod]
		public void SerializeIncludeDirectoriesTest()
		{
			CSqlParameter parameterIn = new CSqlParameter();

			parameterIn.IncludeDirectories = new List<string>() { "A", "B" };
			CSqlParameter parameterOut = CloneParameterBySerialization( parameterIn );

			Assert.AreEqual( 2, parameterOut.IncludeDirectories.Count );
			Assert.AreEqual( "A", parameterOut.IncludeDirectories[0] );
			Assert.AreEqual( "B", parameterOut.IncludeDirectories[1] );
		}


		private CSqlParameter CloneParameterBySerialization( CSqlParameter parameter )
		{
			XmlSerializer serializer = new XmlSerializer( parameter.GetType() );
			using ( Stream stream = new MemoryStream() ) {
				serializer.Serialize( stream, parameter );
				stream.Seek( 0, SeekOrigin.Begin );
				object clone = serializer.Deserialize( stream );
				return (CSqlParameter)clone;
			}
		}
	}
}
