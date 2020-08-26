using System.Collections.Generic;

using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Upgrading;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a single SQL Server database processing step.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseProcessingStep" /> is used by <see cref="SqlServerDatabaseCleanupProcessor" /> and <see cref="SqlServerDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="DatabaseProcessingStep{TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SqlServerDatabaseProcessingStep : DatabaseProcessingStep<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Overrides

        /// <inheritdoc />
        protected override void ExecuteBatchesImpl (List<string> batches, SqlServerDatabaseManager manager, SqlConnection connection, SqlTransaction transaction)
        {
            foreach (string batch in batches)
            {
                //TODO: Log: this.Log(LogLevel.Debug, "Executing SQL Server database processing command:{0}{1}", Environment.NewLine, batch);
                using (SqlCommand command = transaction == null ? new SqlCommand(batch, connection) : new SqlCommand(batch, connection, transaction))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        #endregion
    }
}
