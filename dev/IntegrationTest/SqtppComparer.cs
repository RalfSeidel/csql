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
            string arguments = this.inputFile;

            StringBuilder output = new StringBuilder();

            ProcessStartInfo processStartInfo = new ProcessStartInfo(options.PathToSqtpp, arguments);
            processStartInfo.CreateNoWindow = true;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            processStartInfo.UseShellExecute = false;
            processStartInfo.WorkingDirectory = options.WorkingDirectory;

            Process process = new Process();
            process.StartInfo = processStartInfo;
            process.OutputDataReceived += (s, e) => { if (e.Data != null) output.Append(e.Data + "\r\n"); };
            process.ErrorDataReceived += (s, e) => { if (e.Data != null) output.Append(e.Data + "\r\n"); };

            process.Start();

            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit();

            StreamWriter streamWriter = new StreamWriter(this.outputFile);
            streamWriter.Write(output);
            streamWriter.Close();
        }



        #region IComparer Members

        public ComparerResult Compare()
        {
            ExecuteSqtpp();

            FileComparer fileComparer = new FileComparer(this.outputFile, this.options.WorkingDirectory + this.referenceOutput);

            ComparerResult comparerResult = fileComparer.Compare();

            string identifier = this.inputFile + " (" + comparerResult.LineNumber + "," + comparerResult.ColumnNumber + ") : ";
            comparerResult.Message = identifier + comparerResult.Message;

            return comparerResult;
        }

        #endregion
    }
}
