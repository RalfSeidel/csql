using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IntegrationTest
{
	/// <summary>
	/// 
	/// </summary>
	[TestClass]
	public class SpecialSqtppTest
	{
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		[ClassInitialize()]
		public static void SqtppSpecialIntegrationTestInitialize( TestContext testContext )
		{
			Environment.InitializeSystemEnvironment();
			InitializeDataSource( Path.Combine( testContext.TestDeploymentDir, "SpecialSqtppTest.csv" ) );
		}

		[DataSource( "Microsoft.VisualStudio.TestTools.DataSource.CSV", "SpecialSqtppTest.csv", "SpecialSqtppTest#csv", DataAccessMethod.Sequential )]
		[TestMethod]
		public void SpecialSqtppTests()
		{
			SqtppComparerOptions options = new SqtppComparerOptions
			{
				PathToSqtpp = Environment.PathToSqtpp,
				PathToWorkingDirectory = Environment.WorkingDirectory,
				OptionalArgumentsString = this.TestContext.DataRow["AdditionalOptions"].ToString()
			};

			SqtppComparer sqtppComparer = new SqtppComparer( this.TestContext.DataRow["InputFile"].ToString(), this.TestContext.DataRow["ReferenceFile"].ToString(), options );

			ComparerResult comparerResult = sqtppComparer.Compare();

			Assert.IsTrue( comparerResult.AreEqual, comparerResult.Message );
		}


		private static void InitializeDataSource( string csvFilePath )
		{
			StringBuilder stringBuilder = new StringBuilder( "InputFile,ReferenceFile,AdditionalOptions\n" );

			AddEntryToString( stringBuilder, @"input\special\emitline1.h", @"reference\special_emitline1.h", @"/E /e+" );
			AddEntryToString( stringBuilder, @"input\special\emitline2.h", @"reference\special_emitline2.h", @"/E /e+" );
			AddEntryToString( stringBuilder, @"input\special\findfile.h", @"reference\special_findfile.h", @"/E /e+ /Iinput\special\include" );
			AddEntryToString( stringBuilder, @"input\ansiabc.txt", @"reference\Co1200_ansiabc.txt", "/Co1200" );
			AddEntryToString( stringBuilder, @"input\utf8abc.txt", @"reference\Co1200_utf8abc.txt", "/Co1200" );
			AddEntryToString( stringBuilder, @"input\unicode.txt", @"reference\Co1200_unicode.txt", "/Co1200" );
			

			Utils.WriteStringToFile( stringBuilder.ToString(), csvFilePath );
		}

		private static void AddEntryToString( StringBuilder stringBuilder, string inputFilePath, string referenceFilePath, string options )
		{
			stringBuilder.Append( inputFilePath + "," + referenceFilePath + "," + options + "\r\n" );
		}

	}
}
