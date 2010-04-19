using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace IntegrationTest
{
    public class SqtppComparer : IComparer
    {

        private string inputFile;
        private string outputFile;
        private string referenceOutput;
        SqtppComparerOptions options;

        public SqtppComparer(string pathToInputFile, string pathToReferenceOutputFile, SqtppComparerOptions options)
        {
            this.inputFile = pathToInputFile;
            this.referenceOutput = pathToReferenceOutputFile;
            this.outputFile = Path.GetTempFileName();
            this.options = options;
        }

        public SqtppComparer(string fileName, SqtppComparerOptions options)
        {
            this.inputFile = options.PathToInputFiles + fileName;
            this.referenceOutput = options.PathToReferenceFiles + fileName;
            this.outputFile = Path.GetTempFileName();
            this.options = options;
        }


        private void ExecuteSqtpp()
        {
            string argumens = "-o" + this.outputFile + " " + this.inputFile;

            ProcessStartInfo processStartInfo = new ProcessStartInfo(options.PathToSqtpp,argumens);
            processStartInfo.CreateNoWindow = true;
            Process process = new Process();
            process.StartInfo = processStartInfo;

            process.Start();
            process.WaitForExit();
        }

        #region IComparer Members

        public ComparerResult Compare()
        {
            ExecuteSqtpp();

            FileComparer fileComparer = new FileComparer(this.outputFile, this.referenceOutput);

            ComparerResult comparerResult = fileComparer.Compare();

            if (!comparerResult.IsEqual)
                comparerResult.Message = this.inputFile + " (" + comparerResult.LineNumber + "," + comparerResult.ColumnNumber + ") : " + comparerResult.Message;

            return comparerResult;
        }

        #endregion
    }
}
