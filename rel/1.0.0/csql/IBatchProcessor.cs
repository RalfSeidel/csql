using System;
using System.Collections.Generic;
using System.Text;

namespace csql
{
    /// <summary>
    /// An abstract declaration of a processor of SQL command batches.
    /// </summary>
    internal interface IBatchProcessor : IDisposable
    {
		/// <summary>
		/// Called immedialty after the creation of the processor. 
		/// Used to validate options, settings, environment and so on. If the validation
		/// fails the implementation should throw a <see cref="T:TerminateException"/>.
		/// </summary>
		void Validate();

		/// <summary>
		/// This method is called before the first batch is executed.
		/// </summary>
        /// <remarks>
        /// The implementation may for example emit an entry message.
        /// </remarks>
        void SignIn();

		/// <summary>
		/// Processes progress informations.
		/// </summary>
		/// <param name="context">Some information about the location in the current script.</param>
		/// <param name="progressInfo">Some informal text about the progress of script processing.</param>
		void ProcessProgress( ProcessorContext context, string progressInfo );

		/// <summary>
        /// The batch execution service. 
        /// </summary>
        /// <remarks>
        /// Implement this method to execute the commands.
        /// </remarks>
        /// <param name="batch">
        /// The current sql command batch.
        /// </param>
		void ProcessBatch( ProcessorContext context, string batch );

		/// <summary>
		/// The method called after the last batch has been processed.
		/// </summary>
        /// <remarks>
        /// The implementation may for example emit an exit message.
        /// </remarks>
        void SignOut();

		/// <summary>
		/// Cancels the execution of the current batch.
		/// </summary>
		void Cancel();

	}
}
