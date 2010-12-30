using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    public class ComparerResult
    {
        public bool AreEqual { get; set; }
        public string Message { get; set; }
        public string Identifier { get; set; }
        public int LineNumber { get; set; }
        public int ColumnNumber { get; set; }

        public ComparerResult()
        {
            this.Identifier = "";
            this.AreEqual = false;
            this.Message = "";
            this.LineNumber = 0;
            this.ColumnNumber = 0;
        }
    }
}
