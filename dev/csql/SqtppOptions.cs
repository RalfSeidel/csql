using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
    public class SqtppOptions
    {
        /// <summary>
        /// Macro defintions passed to sqtpp with the /D argument
        /// </summary>
        private Dictionary<string, string> m_macroDefinitions = new Dictionary<string, string>();

        /// <summary>
        /// List of additional include directories
        /// </summary>
        private List<string> m_includeDirectories = new List<string>();

        /// <summary>
        /// Path of the main input file.
        /// </summary>
        private string m_inputFile;

        /// <summary>
        /// Path of the main output file.
        /// </summary>
        private string m_outputFile;

        /// <summary>
        /// Every other argument/option e.g. /E
        /// </summary>
        private string m_otherArguments = String.Empty;


        public string CommandLineArguments
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
