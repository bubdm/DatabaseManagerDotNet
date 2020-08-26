using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.Data.SqlClient;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements a database version upgrader for SQL Server databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseVersionUpgrader" /> uses upgrade steps associated to specific source versions to perform the upgrade.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDatabaseVersionUpgrader : DatabaseVersionUpgrader<SqlConnection, SqlTransaction, SqlConnectionStringBuilder, SqlServerDatabaseManager, SqlServerDatabaseManagerConfiguration>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="upgradeSteps"> The sequence of upgrade steps supported by this version upgrader. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="upgradeSteps" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="upgradeSteps" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="upgradeSteps" /> is an empty sequence or contains the same source version multiple times. </exception>
        public SqlServerDatabaseVersionUpgrader (IEnumerable<SqlServerDatabaseVersionUpgradeStep> upgradeSteps)
        {
            if (upgradeSteps == null)
            {
                throw new ArgumentNullException(nameof(upgradeSteps));
            }

            this.UpgradeSteps = new List<SqlServerDatabaseVersionUpgradeStep>(upgradeSteps);

            if (this.UpgradeSteps.Count == 0)
            {
                throw new ArgumentException("No upgrade steps.", nameof(upgradeSteps));
            }

            foreach (int sourceVersion in this.UpgradeSteps.Select(x => x.SourceVersion))
            {
                int count = this.UpgradeSteps.Count(x => x.SourceVersion == sourceVersion);
                if (count != 1)
                {
                    throw new ArgumentException("Source version " + sourceVersion + " specified multiple times.", nameof(upgradeSteps));
                }
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="upgradeSteps"> The array of upgrade steps supported by this version upgrader. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="upgradeSteps" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="upgradeSteps" /> is an empty array or contains the same source version multiple times. </exception>
        public SqlServerDatabaseVersionUpgrader (params SqlServerDatabaseVersionUpgradeStep[] upgradeSteps)
            : this((IEnumerable<SqlServerDatabaseVersionUpgradeStep>)upgradeSteps)
        {
        }

        #endregion




        #region Instance Properties/Indexer

        private List<SqlServerDatabaseVersionUpgradeStep> UpgradeSteps { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the list of available upgrade steps.
        /// </summary>
        /// <returns>
        ///     The list of available upgrade steps.
        ///     The list is never empty.
        /// </returns>
        public List<SqlServerDatabaseVersionUpgradeStep> GetUpgradeSteps () => new List<SqlServerDatabaseVersionUpgradeStep>(this.UpgradeSteps);

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => this.UpgradeSteps.Any(x => x.RequiresScriptLocator);

        /// <inheritdoc />
        public override int GetMaxVersion (SqlServerDatabaseManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Max() + 1;

        /// <inheritdoc />
        public override int GetMinVersion (SqlServerDatabaseManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Min();

        /// <inheritdoc />
        public override bool Upgrade (SqlServerDatabaseManager manager, int sourceVersion)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            if (sourceVersion < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceVersion));
            }

            try
            {
                //TODO: Log: this.Log(LogLevel.Debug, "Beginning SQL Server database upgrade step: SourceVersion=[{0}]; Connection=[{1}]", sourceVersion, manager.Configuration.ConnectionString);

                SqlServerDatabaseVersionUpgradeStep upgradeStep = this.UpgradeSteps.FirstOrDefault(x => x.SourceVersion == sourceVersion);
                if (upgradeStep == null)
                {
                    throw new Exception("No upgrade step found for source version: " + sourceVersion);
                }

                using (SqlConnection connection = manager.CreateInternalConnection(null))
                {
                    using (SqlTransaction transaction = upgradeStep.RequiresTransaction ? connection.BeginTransaction(IsolationLevel.Serializable) : null)
                    {
                        upgradeStep.Execute(manager, connection, transaction);
                        transaction?.Commit();
                    }
                }

                //TODO: Log: this.Log(LogLevel.Debug, "Finished SQL Server database upgrade step: SourceVersion=[{0}]; Connection=[{1}]", sourceVersion, manager.Configuration.ConnectionString);

                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQL Server database upgrade step failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }

        #endregion
    }
}
