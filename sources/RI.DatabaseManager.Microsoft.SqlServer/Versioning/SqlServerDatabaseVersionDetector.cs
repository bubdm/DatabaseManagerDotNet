using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Implements a database version detector for SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseVersionDetector" /> can be used with either a default SQL Server cleanup script or with a custom batch.
    /// See <see cref="SqlServerDbManagerOptions"/> for more information.
    ///     </para>
    ///     <para>
    ///         The script must return a scalar value which indicates the current version of the database.
    ///         The script must return -1 to indicate when the database is damaged or in an invalid state or 0 to indicate that the database does not yet exist and needs to be created.
    ///     </para>
    ///     <para>
    ///         If the script contains multiple batches, each batch is executed consecutively.
    ///         The execution stops on the first batch which returns 0 or -1.
    ///         If no batch returns 0 or -1, the last batch determines the version.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseVersionDetector : DbVersionDetectorBase<SqlConnection, SqlTransaction>
    {
        #region Instance Constructor/Destructor

        private SqlServerDbManagerOptions Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseCleanupProcessor" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDatabaseVersionDetector(SqlServerDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        #endregion




        /// <inheritdoc />
        public override bool Detect (IDbManager<SqlConnection, SqlTransaction> manager, out DbState? state, out int version)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            state = null;
            version = -1;

            try
            {
                List<string> batches = manager.GetScriptBatch(this.ScriptName, true);
                if (batches == null)
                {
                    throw new Exception("Batch retrieval failed for script: " + (this.ScriptName ?? "[null]"));
                }

                using (SqlConnection connection = manager.CreateInternalConnection(null))
                {
                    using (SqlTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                    {
                        foreach (string batch in batches)
                        {
                            using (SqlCommand command = new SqlCommand(batch, connection, transaction))
                            {
                                object value = command.ExecuteScalar();
                                version = value.Int32FromSqlServerResult() ?? -1;
                                if (version <= 0)
                                {
                                    break;
                                }
                            }
                        }

                        transaction.Rollback();
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQL Server database version detection failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }
    }
}
