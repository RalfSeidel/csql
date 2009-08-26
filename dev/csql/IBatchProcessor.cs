using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
    /// <summary>
    /// An abstract declaration of a processor of SQL command batches.
    /// </summary>
    interface IBatchProcessor
    {
		/// <summary>
		/// This method is called before the first batch is executed.
		/// </summary>
        /// <remarks>
        /// The implementation may for example emit an entry message.
        /// </remarks>
        void SignIn();


        /// <summary>
        /// The batch execution service. 
        /// </summary>
        /// <remarks>
        /// Implement this method to execute the commands.
        /// </remarks>
        /// <param name="batch">
        /// The current sql command batch.
        /// </param>
        void ProcessBatch( string batch );


		/// <summary>
		/// The method called after the last batch has been processed.
		/// </summary>
        /// <remarks>
        /// The implementation may for example emit an exit message.
        /// </remarks>
        void SignOut();
    }
}
