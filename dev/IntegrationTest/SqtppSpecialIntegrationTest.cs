using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace IntegrationTest
{
    [TestClass]
    public class SqtppSpecialIntegrationTest
    {
        [TestMethod]
        public void ExecuteTest()
        {
			string testFileDirectory = Environment.TestFileDirectory;
			string pathToSqtpp = Environment.PathToSqtpp;

			SqtppComparerOptions options = new SqtppComparerOptions
			{
				PathToSqtpp = pathToSqtpp,
				PathToWorkingDirectory = Environment.WorkingDirectory,
				OptionalArgumentsString = "/E /e+"
			};


            ComparerContainer comparerContainer = new ComparerContainer();
                   
            comparerContainer.Comparers.Add(new SqtppComparer( @"input\special\emitline2_1.h", @"reference\special_emitline2_1.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer( @"input\special\emitline2_2.h", @"reference\special_emitline2_2.h", options));

            options.OptionalArgumentsString += @" /Iinput\special\include";
            comparerContainer.Comparers.Add(new SqtppComparer(@"input\special\findfile.h", @"reference\special_findfile.h", options));

            ComparerResult comparerResult = comparerContainer.Compare();

            if (!comparerResult.IsEqual)
                Debug.WriteLine(comparerResult.Message);

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }
    }
}
