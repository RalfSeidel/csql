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
            SqtppComparerOptions options = new SqtppComparerOptions {
                PathToSqtpp = @"D:\src\sqtpp\dev\Debug\sqtpp.exe",
                PathToWorkingDirectory = @"D:\src\sqtpp\dev\IntegrationTest\Files\sqtpp\",
                OptionalArgumentsString = ""
            };

            string directory = @"D:\src\sqtpp\dev\IntegrationTest\Files\sqtpp\input\";

            ComparerContainer comparerContainer = new ComparerContainer();
            
            foreach (string item in Directory.GetFiles(directory))
                comparerContainer.Comparers.Add(new SqtppComparer( @"input\" + Path.GetFileName(item), @"reference\" + Path.GetFileName(item), options));

            ComparerResult comparerResult = comparerContainer.Compare();

            if (!comparerResult.IsEqual)
                Debug.WriteLine(comparerResult.Message);

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }
    }
}
