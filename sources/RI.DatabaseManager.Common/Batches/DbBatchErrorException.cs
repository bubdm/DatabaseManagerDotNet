using System;
using System.Runtime.Serialization;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     The <see cref="DbBatchErrorException" /> is thrown when a database batch error occurred.
    /// </summary>
    [Serializable]
    public class DbBatchErrorException : Exception
    {
        #region Constants

        private const string ErrorExceptionMessage = "Database batch execution failed: {0}.";

        private const string GenericExceptionMessage = "Database batch execution failed.";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbBatchErrorException" />.
        /// </summary>
        public DbBatchErrorException()
            : base(DbBatchErrorException.GenericExceptionMessage) { }

        /// <summary>
        ///     Creates a new instance of <see cref="DbBatchErrorException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        public DbBatchErrorException(string message)
            : base(string.Format(DbBatchErrorException.ErrorExceptionMessage, message)) { }

        /// <summary>
        ///     Creates a new instance of <see cref="DbBatchErrorException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public DbBatchErrorException(string message, Exception innerException)
            : base(message, innerException) { }

        /// <summary>
        ///     Creates a new instance of <see cref="DbBatchErrorException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        private DbBatchErrorException(SerializationInfo info, StreamingContext context)
            : base(info, context) { }

        #endregion
    }
}
