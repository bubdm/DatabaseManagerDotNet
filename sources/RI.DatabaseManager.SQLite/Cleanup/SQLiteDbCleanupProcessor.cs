using System;
using System.Data.SQLite;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Implements a database cleanup processor for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDbCleanupProcessor" /> can be used with either a default SQLite cleanup script or with a custom batch.
    /// See <see cref="SQLiteDbManagerOptions"/> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDbCleanupProcessor : DbCleanupProcessorBase<SQLiteConnection, SQLiteTransaction>
    {
        #region Instance Constructor/Destructor

        private SQLiteDbManagerOptions Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbCleanupProcessor" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDbCleanupProcessor(SQLiteDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        #endregion




        /// <inheritdoc />
        public override bool Cleanup (IDbManager<SQLiteConnection, SQLiteTransaction> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            try
            {
                this.Log(LogLevel.Information, "Beginning SQLite database cleanup");

                IDbBatch<SQLiteConnection, SQLiteTransaction> batch;

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
                    batch = new DbBatch<SQLiteConnection, SQLiteTransaction>();

                    foreach (string command in this.Options.GetDefaultCleanupScript())
                    {
                        batch.AddScript(command, DbBatchTransactionRequirement.Disallowed);
                    }
                }

                bool result = manager.ExecuteBatch(batch, false, true);

                if (result)
                {
                    this.Log(LogLevel.Information, "Finished SQLite database cleanup");
                }
                else
                {
                    this.Log(LogLevel.Error, "SQLite database cleanup failed");
                }

                return result;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "SQLite database cleanup failed:{0}{1}", Environment.NewLine, exception.ToString());
                return false;
            }
        }
    }
}
