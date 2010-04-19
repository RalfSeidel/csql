using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows.Forms;

namespace IntegrationTest
{
    [TestClass]
    public class IntegrationTestWrapper
    {
        [TestMethod]
        public void IntegrationTestWrapperMethod()
        {
            SqtppComparerOptions options = new SqtppComparerOptions {
                PathToSqtpp = @"D:\src\sqtpp\dev\Debug\sqtpp.exe",
                PathToInputFiles = @"D:\src\sqtpp\dev\IntegrationTest\Files\sqtpp\input\",
                PathToReferenceFiles = @"D:\src\sqtpp\dev\IntegrationTest\Files\sqtpp\reference\"
            };

            ComparerContainer comparerContainer = new ComparerContainer();
            comparerContainer.Comparers.Add(new SqtppComparer("Define1Test.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer("Define6Test.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer("If1Test.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer("ErrC1020.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer("ErrC1082.h", options));
            comparerContainer.Comparers.Add(new SqtppComparer("ErrC2008.h", options));

            string path = Application.ExecutablePath;

            ComparerResult comparerResult = comparerContainer.Compare();

            Assert.IsTrue(comparerResult.IsEqual, comparerResult.Message);
        }
    }
}
