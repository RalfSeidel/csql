using System;
using System.Collections.Generic;
using System.Text;

namespace IntegrationTest
{
    public class SqtppComparerOptions
    {
        public string PathToSqtpp { get; set; }
        public string PathToInputFiles { get; set; }
        public string PathToReferenceFiles { get; set; }
        public string WorkingDirectory { get; set; }
    }
}
