using System;
using System.Collections.Generic;
using System.Text;

namespace System
{
    static class ExtensionMethods
    {
        public static void Log(this string message)
        {
            System.Diagnostics.Debug.WriteLine( message );
        }
    }
}
