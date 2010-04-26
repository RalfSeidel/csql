using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace IntegrationTest
{
    public class SqtppComparer : IComparer
    {

        private string relativePathToInputFile;
        private string relativePathToReferenceOutputFile;
        private string sqtppOutputFile;
        SqtppComparerOptions options;

        public SqtppComparer(string relativePathToInputFile, string relativePathToReferenceOutputFile, SqtppComparerOptions options)
        {
            this.relativePathToInputFile = relativePathToInputFile;
            this.relativePathToReferenceOutputFile = relativePathToReferenceOutputFile;

            this.sqtppOutputFile = Path.GetTempFileName();
            this.options = options;
        }

      
        private void ExecuteSqtpp()
        {
            string arguments = this.options.OptionalArgumentsString + " -O" + this.sqtppOutputFile + " " + this.relativePathToInputFile;

            StringBuilder output = new StringBuilder();

            ProcessStartInfo processStartInfo = new ProcessStartInfo(options.PathToSqtpp, arguments);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = options.PathToWorkingDirectory;

            Process process = new Process();
            process.StartInfo = processStartInfo;

            process.Start();

            process.WaitForExit();
        }



        #region IComparer Members

        public ComparerResult Compare()
        {
            ExecuteSqtpp();

            FileComparer fileComparer = new FileComparer(this.sqtppOutputFile, this.options.PathToWorkingDirectory + this.relativePathToReferenceOutputFile);

            ComparerResult comparerResult = fileComparer.Compare();

            string identifier = this.options.PathToWorkingDirectory + this.relativePathToInputFile + " (" + comparerResult.LineNumber + "," + comparerResult.ColumnNumber + ") : ";
            comparerResult.Message = identifier + comparerResult.Message;

            return comparerResult;
        }

        #endregion
    }
}
