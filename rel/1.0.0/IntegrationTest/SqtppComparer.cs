using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace IntegrationTest
{
    internal class SqtppComparer : IComparer
    {
        private string relativePathToInputFile;
        private string relativePathToReferenceFile;
        private string sqtppOutputFile;
        private SqtppComparerOptions options;

        public SqtppComparer(string relativePathToInputFile, string relativePathToReferenceOutputFile, SqtppComparerOptions options)
        {
            this.relativePathToInputFile = relativePathToInputFile;
            this.relativePathToReferenceFile = relativePathToReferenceOutputFile;

            this.sqtppOutputFile = Path.GetTempFileName();
            this.options = options;
        }

      
        #region IComparer Members

		/// <summary>
		/// Execute sqtpp and compare its output with the reference output file.
		/// </summary>
		/// <returns></returns>
        public ComparerResult Compare()
        {
            ExecuteSqtpp();

            FileComparer fileComparer = new FileComparer(this.sqtppOutputFile, this.options.PathToWorkingDirectory + this.relativePathToReferenceFile);

            ComparerResult comparerResult = fileComparer.Compare();

			if ( !comparerResult.AreEqual ) {
				string absoluteInputPath = System.IO.Path.GetFullPath( this.options.PathToWorkingDirectory + this.relativePathToInputFile );
				string absoluteOutputPath = System.IO.Path.GetFullPath( this.sqtppOutputFile );
				string absoluteReferencePath = System.IO.Path.GetFullPath( this.options.PathToWorkingDirectory + this.relativePathToReferenceFile );
				Trace.WriteLine( "Test failed for " );
				Trace.Write( '\t' );
				Trace.WriteLine( absoluteInputPath );
				Trace.WriteLine( "when comparing to " );
				Trace.Write( '\t' );
				Trace.WriteLine( absoluteOutputPath );
				Trace.WriteLine( "with reference output in" );
				Trace.Write( '\t' );
				Trace.WriteLine( absoluteReferencePath );
				Trace.WriteLine( "Error message:" );
				Trace.Write( '\t' );
				Trace.WriteLine( comparerResult.Message );
				Trace.WriteLine( "" );

				string identifier = this.relativePathToInputFile + " (" + comparerResult.LineNumber + "," + comparerResult.ColumnNumber + "): ";
				comparerResult.Message = identifier + comparerResult.Message;
			} else {
				System.IO.File.Delete( this.sqtppOutputFile );
			}

            return comparerResult;
        }

        #endregion

		private void ExecuteSqtpp()
		{
			string arguments = this.options.OptionalArgumentsString + " -O" + this.sqtppOutputFile + " " + this.relativePathToInputFile;

			StringBuilder output = new StringBuilder();

			ProcessStartInfo processStartInfo = new ProcessStartInfo( options.PathToSqtpp, arguments );
			processStartInfo.CreateNoWindow = true;
			processStartInfo.UseShellExecute = false;
			processStartInfo.WorkingDirectory = options.PathToWorkingDirectory;

			Process process = new Process();
			process.StartInfo = processStartInfo;

			process.Start();

			process.WaitForExit();
		}

	}
}
