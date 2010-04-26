using System;
using System.Text;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace IntegrationTest
{
    /// <summary>
    /// Summary description for DataDrivenTest
    /// </summary>
    [TestClass]
    public class DataDrivenTest
    {
        public DataDrivenTest()
        {
            //
            // TODO: Add constructor logic here
            //

            //CreateDataSource();
        }

        private static string csvFile = @"C:\test\generatedcsv.csv";

        private void CreateDataSource()
        {
            StringBuilder stringBuilder = new StringBuilder("Filename,AdditionalOptions\n");
            stringBuilder.Append(@"C:\testrrr.txt,/E /e" + "\n");
            stringBuilder.Append(@"C:\test12.txt,/g /e");

            StreamWriter streamWriter = new StreamWriter(csvFile);
            streamWriter.Write(stringBuilder.ToString());
            streamWriter.Close();
        }


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
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

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "C:\\test\\test.xml", "StandardFile", DataAccessMethod.Sequential)]
        [TestMethod]
        public void DataDrivenTestMethod()
        {
            Debug.WriteLine("Stepping throuw" + this.TestContext.DataRow["Filename"]);

            Assert.IsTrue(runned);
            runned = true;

            //
            // TODO: Add test logic	here
            //
        }

        private bool runned = false;

        [DataSource("Microsoft.VisualStudio.TestTools.DataSource.CSV", "C:\\test\\generatedcsv.csv", "generatedcsv#csv", DataAccessMethod.Sequential), TestMethod]
        public void DataDrivenTestMethod2()
        {

            Debug.WriteLine("Stepping throuw" + this.TestContext);
            //
            // TODO: Add test logic	here
            //
        }
    }
}
