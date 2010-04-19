using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IntegrationTest
{
    public class FileComparer : IComparer
    {
        private string pathOfFirstFile;
        private string pathOfSecondFile;

        public FileComparer(string pathOfFirstFile, string pathOfSecondFile)
        {
            this.pathOfFirstFile = pathOfFirstFile;
            this.pathOfSecondFile = pathOfSecondFile;
        }

        private void LoadLinesFromFileIntoList(string path, List<string> list)
        {
            if (!File.Exists(path))
                throw new FileNotFoundException("File '" + path + "' was not found.");

            StreamReader streamReader = new StreamReader(path, System.Text.Encoding.Default);

            try
            {
                while (!streamReader.EndOfStream)
                    list.Add(streamReader.ReadLine());
            }
            catch (Exception exception)
            {
                throw exception;
            }
            finally
            {
                streamReader.Close();
            }
        }

        #region IComparer Members

        public ComparerResult Compare()
        {
            List<string> linesOfFirstFile = new List<string>();
            List<string> linesOfSecondFile = new List<string>();

            LoadLinesFromFileIntoList(this.pathOfFirstFile, linesOfFirstFile);
            LoadLinesFromFileIntoList(this.pathOfSecondFile, linesOfSecondFile);

            ComparerResult comparerResult = new ComparerResult();

            if (linesOfFirstFile.Count != linesOfSecondFile.Count)
            {
                comparerResult.IsEqual = false;
                comparerResult.Message = "Line count of Input and ReferenceOutput ist not equal.";

                return comparerResult;
            }

            for (int i = 0; i < linesOfFirstFile.Count; i++)
            {
                if (linesOfFirstFile[i] != linesOfSecondFile[i])
                {
                    comparerResult.IsEqual = false;
                    comparerResult.Message = "Input-line '" + linesOfFirstFile[i] + "' is not equal to ReferenceOutput-line '" + linesOfSecondFile[i] + "'.";
                    comparerResult.LineNumber = i;
                    comparerResult.ColumnNumber = GetPositionOfFirstDifference(linesOfFirstFile[i], linesOfSecondFile[i]);

                    return comparerResult;
                }
            }

            comparerResult.IsEqual = true;
            return comparerResult;
        }

        #endregion


        private int GetPositionOfFirstDifference(string input, string reference)
        {
            int i;

            for (i = 0; i < input.Length; i++)
            {
                if (reference.Length < (i + 1)) //refenrence < input
                    return (i + 1);

                if (input[i] != reference[i]) //first position thats not equal
                    return (i + 1);
            }

            return (i + 1); //input < reference
        }
    }
}
