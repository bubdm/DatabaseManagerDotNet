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
    ///         <see cref="SqlServerDbProcessingStep" /> is used by <see cref="SqlServerDatabaseCleanupProcessor" /> and <see cref="SqlServerDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         See <see cref="DbProcessingStep{TConnection,TTransaction,TManager}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SqlServerDbProcessingStep : DbProcessingStep<,,>
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
