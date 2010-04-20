using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace IntegrationTest
{
    [TestClass]
    public class SqtppStandardIntegrationTest
    {
        [TestMethod]
        public void ExecuteTest()
        {
			string testFileDirectory = Environment.TestFileDirectory;
			string pathToSqtpp = Environment.PathToSqtpp;


            SqtppComparerOptions options = new SqtppComparerOptions {
				PathToSqtpp = pathToSqtpp,
				PathToWorkingDirectory = Environment.WorkingDirectory,
                OptionalArgumentsString = ""
            };


			ComparerContainer comparerContainer = new ComparerContainer();

			foreach ( string item in Directory.GetFiles( testFileDirectory ) )
                comparerContainer.Comparers.Add(new SqtppComparer( @"input\" + Path.GetFileName(item), @"reference\" + Path.GetFileName(item), options));

            ComparerResult comparerResult = comparerContainer.Compare();

            if (!comparerResult.IsEqual)
                Debug.WriteLine(comparerResult.Message);

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }
    }
}
