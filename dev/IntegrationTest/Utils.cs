using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace IntegrationTest
{
    public static class Utils
    {
        public static void WriteStringToFile(string theString, string path)
        {
			StreamWriter streamWriter = new StreamWriter( path, false, Encoding.Default );
            streamWriter.Write(theString);
            streamWriter.Close();
        }
    }
}
