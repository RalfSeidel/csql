using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IntegrationTest
{

	/// <summary>
	/// This test runs sqtpp with the default parameters on all files
	/// in the folder files\sqtpp\input and compares the result with
	/// the files in files\sqtpp\reference.
	/// 
	/// The test succeeds when the results are equal i.e. no 
	/// difference in the sqtpp output is found when compared 
	/// with the output of the reference file.
	/// </summary>
    [TestClass]
    public class SqtppDefaultIntegrationTest
    {
		/// <summary>
		/// Gets or sets the test context which provides
		/// information about and functionality for the current test run.
		/// </summary>
		public TestContext TestContext { get; set; }

		/// <summary>
		/// Scan the input folder and add every file name found there
		/// to the "GeneratedTestData.csv". The later will be used
		/// to execute the data driven test.
		/// </summary>
		/// <param name="testContext"></param>
		[ClassInitialize()]
		public static void SqtppStandardIntegrationTestInitialize( TestContext testContext )
		{
			Environment.InitializeSystemEnvironment();
			InitializeDataSource( Path.Combine( testContext.TestDeploymentDir, "GeneratedTestData.csv" ) );
		}


		[DataSource( "Microsoft.VisualStudio.TestTools.DataSource.CSV", "GeneratedTestData.csv", "GeneratedTestData#csv", DataAccessMethod.Sequential )]
        [TestMethod]
        public void ExecuteSqtppStandardIntegrationTest()
        {
            SqtppComparerOptions options = new SqtppComparerOptions {
				PathToSqtpp = Environment.PathToSqtpp,
				PathToWorkingDirectory = Environment.WorkingDirectory,
                OptionalArgumentsString = this.TestContext.DataRow["AdditionalOptions"].ToString()
            };

            SqtppComparer sqtppComparer = new SqtppComparer(this.TestContext.DataRow["InputFile"].ToString(), this.TestContext.DataRow["ReferenceFile"].ToString(), options);
        
            ComparerResult comparerResult = sqtppComparer.Compare();

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message + "\r\n");
        }

		/// <summary>
		/// Build the csv file used as the data source for the data driven test.
		/// </summary>
		/// <param name="csvFilePath">The path of the csv file to be generated.</param>
        private static void InitializeDataSource(string csvFilePath)
        {
            StringBuilder stringBuilder = new StringBuilder("InputFile,ReferenceFile,AdditionalOptions\n");

            foreach (string item in Directory.GetFiles(Environment.TestFileDirectory))
                stringBuilder.Append(@"input\" + Path.GetFileName(item) + @",reference\" + Path.GetFileName(item) + ", \r\n");

            Utils.WriteStringToFile(stringBuilder.ToString(), csvFilePath);
        }
    }
}
