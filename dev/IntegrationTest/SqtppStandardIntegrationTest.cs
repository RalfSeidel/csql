using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace IntegrationTest
{
    [TestClass]
    public class SqtppStandardIntegrationTest
    {
        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "GeneratedTestData.csv", "GeneratedTestData#csv", DataAccessMethod.Sequential)]
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

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }


        [ClassInitialize()]
        public static void SqtppStandardIntegrationTestInitialize(TestContext testContext) 
        {
            InitializeDataSource(Path.Combine(testContext.TestDeploymentDir, "GeneratedTestData.csv"));
        }


        private static void InitializeDataSource(string csvFilePath)
        {
            StringBuilder stringBuilder = new StringBuilder("InputFile,ReferenceFile,AdditionalOptions\n");

            foreach (string item in Directory.GetFiles(Environment.TestFileDirectory))
                stringBuilder.Append(@"input\" + Path.GetFileName(item) + @",reference\" + Path.GetFileName(item) + ", \r\n");

            Utils.WriteStringToFile(stringBuilder.ToString(), csvFilePath);
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
