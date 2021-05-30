using System;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;




namespace RI.DatabaseManager.Creation
{
    /// <summary>
    ///     Implements a database creator for Microsoft SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDbCreator : DbCreatorBase<SqlConnection, SqlTransaction, SqlDbType>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbCreator" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDbCreator (SqlServerDbManagerOptions options, ILogger logger) : base(options, logger) { }

        #endregion
    }
}
