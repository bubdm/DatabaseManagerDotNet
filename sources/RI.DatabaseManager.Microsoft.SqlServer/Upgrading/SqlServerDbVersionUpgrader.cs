using System;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements a database version upgrader for Microsoft SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="SqlServerDbManagerOptions" /> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDbVersionUpgrader : BatchNameBasedDbVersionUpgrader<SqlConnection,SqlTransaction, SqlDbType>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbVersionUpgrader" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDbVersionUpgrader(SqlServerDbManagerOptions options, ILogger logger) : base(options, logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        private new SqlServerDbManagerOptions Options { get; }

        #endregion
    }
}
