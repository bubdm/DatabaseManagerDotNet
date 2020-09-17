using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Scripts;




namespace RI.DatabaseManager.Cleanup
{
    /// <summary>
    ///     Implements a database cleanup processor for SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseCleanupProcessor" /> can be used with either a default SQL Server cleanup script (<see cref="DefaultCleanupScript" />) or with a custom processing step.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseCleanupProcessor : DbCleanupProcessorBase<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Constants

        /// <summary>
        ///     The default cleanup script used when no custom processing step is specified.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default cleanup script uses <c> DBCC SHRINKDATABASE 0 </c>, executed as a single command.
        ///     </para>
        /// </remarks>
        public const string DefaultCleanupScript = "DBCC SHRINKDATABASE (0);" + DbScriptLocatorBase.DefaultBatchSeparator;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseCleanupProcessor" />.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default cleanup script is used (<see cref="DefaultCleanupScript" />).
        ///     </para>
        /// </remarks>
        public SqlServerDatabaseCleanupProcessor ()
        {
            this.CleanupStep = null;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseCleanupProcessor" />.
        /// </summary>
        /// <param name="cleanupStep"> The custom processing step which performs the cleanup or null if the default cleanup script is used (<see cref="DefaultCleanupScript" />). </param>
        /// <exception cref="ArgumentNullException"> <paramref name="cleanupStep" /> is null. </exception>
        public SqlServerDatabaseCleanupProcessor (SqlServerDbProcessingStep cleanupStep)
        {
            if (cleanupStep == null)
            {
                throw new ArgumentNullException(nameof(cleanupStep));
            }

            this.CleanupStep = cleanupStep;
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseCleanupProcessor" />.
        /// </summary>
        /// <param name="scriptName"> The script name which is used to perform the cleanup. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        public SqlServerDatabaseCleanupProcessor (string scriptName)
        {
            if (scriptName == null)
            {
                throw new ArgumentNullException(nameof(scriptName));
            }

            if (scriptName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(scriptName));
            }

            SqlServerDbProcessingStep step = new SqlServerDbProcessingStep();
            step.AddScript(scriptName);
            this.CleanupStep = step;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the custom processing step which performs the cleanup.
        /// </summary>
        /// <value>
        ///     The custom processing step which performs the cleanup or null if the default cleanup script is used (<see cref="DefaultCleanupScript" />).
        /// </value>
        public SqlServerDbProcessingStep CleanupStep { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => this.CleanupStep?.RequiresScriptLocator ?? false;

        /// <inheritdoc />
        public override bool Cleanup (SqlServerDatabaseManager manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            try
            {
                //TODO: Log: this.Log(LogLevel.Debug, "Beginning SQL Server database cleanup: Connection=[{0}]", manager.Configuration.ConnectionString);

                using (SqlConnection connection = manager.CreateInternalConnection(null))
                {
                    SqlServerDbProcessingStep cleanupStep = this.CleanupStep;
                    if (cleanupStep == null)
                    {
                        List<string> batches = DbScriptLocatorBase.SplitBatches(SqlServerDatabaseCleanupProcessor.DefaultCleanupScript, DbScriptLocatorBase.DefaultBatchSeparator);
                        cleanupStep = new SqlServerDbProcessingStep();
                        cleanupStep.AddBatches(batches);
                    }

                    using (SqlTransaction transaction = cleanupStep.RequiresTransaction ? connection.BeginTransaction(IsolationLevel.Serializable) : null)
                    {
                        cleanupStep.Execute(manager, connection, transaction);
                        transaction?.Commit();
                    }
                }

                //TODO: Log: this.Log(LogLevel.Debug, "Finished SQL Server database cleanup: Connection=[{0}]", manager.Configuration.ConnectionString);

                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQL Server database cleanup failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }

        #endregion
    }
}
