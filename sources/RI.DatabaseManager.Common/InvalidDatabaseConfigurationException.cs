using System;
using System.Runtime.Serialization;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     The <see cref="InvalidDatabaseConfigurationException" /> is thrown when the configuration of a <see cref="IDatabaseManager" /> is not valid.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    [Serializable]
    public class InvalidDatabaseConfigurationException : Exception
    {
        #region Constants

        private const string GenericExceptionMessage = "Invalid database manager configuration.";

        private const string SpecificExceptionMessage = "Invalid database manager configuration: {0}";

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
        /// </summary>
        public InvalidDatabaseConfigurationException ()
            : base(InvalidDatabaseConfigurationException.GenericExceptionMessage)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        public InvalidDatabaseConfigurationException (string message)
            : base(string.Format(InvalidDatabaseConfigurationException.SpecificExceptionMessage, message))
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
        /// </summary>
        /// <param name="message"> The message which describes the exception. </param>
        /// <param name="innerException"> The exception which triggered this exception. </param>
        public InvalidDatabaseConfigurationException (string message, Exception innerException)
            : base(string.Format(InvalidDatabaseConfigurationException.SpecificExceptionMessage, message), innerException)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="InvalidDatabaseConfigurationException" />.
        /// </summary>
        /// <param name="info"> The serialization data. </param>
        /// <param name="context"> The type of the source of the serialization data. </param>
        private InvalidDatabaseConfigurationException (SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        #endregion
    }
}
