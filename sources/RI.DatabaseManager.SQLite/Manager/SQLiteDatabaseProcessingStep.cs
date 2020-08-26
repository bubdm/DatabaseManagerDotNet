using System;
using System.Collections.Generic;
using System.Data.SQLite;

using RI.Framework.Data.Database.Backup;
using RI.Framework.Data.Database.Cleanup;
using RI.Framework.Data.Database.Upgrading;




namespace RI.Framework.Data.Database
{
    /// <summary>
    ///     Implements a single SQLite database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDatabaseProcessingStep" /> is used by <see cref="SQLiteDatabaseBackupCreator" />, <see cref="SQLiteDatabaseCleanupProcessor" />, and <see cref="SQLiteDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="DatabaseProcessingStep{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SQLiteDatabaseProcessingStep : DatabaseProcessingStep<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override void ExecuteBatchesImpl (List<string> batches, SQLiteDatabaseManager manager, SQLiteConnection connection, SQLiteTransaction transaction)
        {
            foreach (string batch in batches)
            {
                //TODO: Log: this.Log(LogLevel.Debug, "Executing SQLite database processing command:{0}{1}", Environment.NewLine, batch);
                using (SQLiteCommand command = transaction == null ? new SQLiteCommand(batch, connection) : new SQLiteCommand(batch, connection, transaction))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
