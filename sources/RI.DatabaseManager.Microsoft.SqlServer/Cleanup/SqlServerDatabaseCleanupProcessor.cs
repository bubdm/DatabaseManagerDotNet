using System;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Implements a database cleanup processor for Microsoft SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseCleanupProcessor" /> can be used with either a default SQL Server cleanup script or with a custom batch.
    /// See <see cref="SqlServerDbManagerOptions"/> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseCleanupProcessor : DbCleanupProcessorBase<SqlConnection, SqlTransaction>
    {
        #region Instance Constructor/Destructor

        private SqlServerDbManagerOptions Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseCleanupProcessor" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDatabaseCleanupProcessor (SqlServerDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        #endregion




        /// <inheritdoc />
        public override bool Cleanup (IDbManager<SqlConnection, SqlTransaction> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            try
            {
                this.Log(LogLevel.Information, "Beginning SQL Server database cleanup");

                IDbBatch<SqlConnection, SqlTransaction> batch;

                if (!this.Options.CustomCleanupBatch.IsEmpty())
                {
                    batch = this.Options.CustomCleanupBatch;
                }
                else if (!string.IsNullOrWhiteSpace(this.Options.CustomCleanupBatchName))
                {
                    batch = manager.GetBatch(this.Options.CustomCleanupBatchName);
                }
                else
                {
                    batch = new DbBatch<SqlConnection, SqlTransaction>();

                    foreach (string command in this.Options.GetDefaultCleanupScript())
                    {
                        batch.AddScript(command, DbBatchTransactionRequirement.Disallowed);
                    }
                }

                bool result = manager.ExecuteBatch(batch, false, true);

                if (result)
                {
                    this.Log(LogLevel.Information, "Finished SQL Server database cleanup");
                }
                else
                {
                    this.Log(LogLevel.Error, "SQL Server database cleanup failed");
                }

                return result;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "SQL Server database cleanup failed:{0}{1}", Environment.NewLine, exception.ToString());
                return false;
            }
        }
    }
}
