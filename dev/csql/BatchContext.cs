using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
    /// <summary>
    /// Stores information about the context of the lines in the current
    /// batch processed.
    /// </summary>
    internal class BatchContext
    {
        /// <summary>
        /// The line offset in the current batch where this context starts.
        /// </summary>
        private readonly int m_batchOffset;


        /// <summary>
        /// The current source file.
        /// </summary>
        private readonly string m_file;

        /// <summary>
        /// The line number in the source script that where the realm of this context begins.
        /// </summary>
        private readonly int m_lineNumber;

        public BatchContext( int batchOffset, string file, int lineNumber )
        {
            m_batchOffset = batchOffset;
            m_file = file;
            m_lineNumber = lineNumber;
        }

        /// <summary>
        /// Gets the batch offset.
        /// </summary>
        /// <value>The line offset in the batch where this context starts.</value>
        public int BatchOffset { get { return m_batchOffset; } }


        /// <summary>
        /// Gets the source file.
        /// </summary>
        /// <value>The source file name/path</value>
        public string File { get { return m_file; } }

        /// <summary>
        /// Gets the line number where the realm of this context begins.
        /// </summary>
        /// <value>The line number.</value>
        public int LineNumber { get { return m_lineNumber; } }
    }
}
