using System;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Cleanup;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Implements a database version detector for Microsoft SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDbVersionDetector : DbVersionDetectorBase<SqlConnection, SqlTransaction, SqlDbType>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbCleanupProcessor" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDbVersionDetector (SqlServerDbManagerOptions options, ILogger logger) : base(options, logger)
        {
        }

        #endregion
    }
}
