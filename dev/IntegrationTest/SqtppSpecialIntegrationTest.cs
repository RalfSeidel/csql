using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IntegrationTest
{
    [TestClass]
    public class SqtppSpecialIntegrationTest
    {
        
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "GeneratedTestData.csv", "GeneratedTestData#csv", DataAccessMethod.Sequential)]
        [TestMethod]
        public void ExecuteSqtppSpecialIntegrationTest()
        {
            SqtppComparerOptions options = new SqtppComparerOptions
            {
                PathToSqtpp = Environment.PathToSqtpp,
                PathToWorkingDirectory = Environment.WorkingDirectory,
                OptionalArgumentsString = this.TestContext.DataRow["AdditionalOptions"].ToString()
            };

            SqtppComparer sqtppComparer = new SqtppComparer(this.TestContext.DataRow["InputFile"].ToString(), this.TestContext.DataRow["ReferenceFile"].ToString(), options);

            ComparerResult comparerResult = sqtppComparer.Compare();

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }


        [ClassInitialize()]
        public static void SqtppSpecialIntegrationTestInitialize(TestContext testContext)
        {
            InitializeDataSource(Path.Combine(testContext.TestDeploymentDir, "GeneratedTestData.csv"));
        }


        private static void InitializeDataSource(string csvFilePath)
        {
            StringBuilder stringBuilder = new StringBuilder("InputFile,ReferenceFile,AdditionalOptions\n");

            AddEntryToString(stringBuilder, @"input\special\emitline2_1.h", @"reference\special_emitline2_1.h", @"/E /e+");
            AddEntryToString(stringBuilder, @"input\special\emitline2_2.h", @"reference\special_emitline2_2.h", @"/E /e+");
            AddEntryToString(stringBuilder, @"input\special\findfile.h", @"reference\special_findfile.h", @"/E /e+ /Iinput\special\include");

            Utils.WriteStringToFile(stringBuilder.ToString(), csvFilePath);
        }

        private static void AddEntryToString(StringBuilder stringBuilder, string inputFilePath, string referenceFilePath, string options)
        {
            stringBuilder.Append(inputFilePath + "," + referenceFilePath + "," + options + "\r\n" );
        }


        private TestContext testContextInstance;

        // <summary>
        //Gets or sets the test context which provides
        //information about and functionality for the current test run.
        //</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }


    }
}
