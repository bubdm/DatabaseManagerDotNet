using System.Collections.Generic;
using System.Data.SQLite;

using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a single SQLite database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDbProcessingStep" /> is used by <see cref="SQLiteDatabaseBackupCreator" />, <see cref="SQLiteDatabaseCleanupProcessor" />, and <see cref="SQLiteDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="DbProcessingStepBase{TConnection,TTransaction,TManager}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SQLiteDbProcessingStep : DbProcessingStepBase<,,>
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
