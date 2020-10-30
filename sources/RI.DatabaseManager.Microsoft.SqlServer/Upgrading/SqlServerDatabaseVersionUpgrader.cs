using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




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
    public sealed class SqlServerDatabaseVersionUpgrader : DbVersionUpgraderBase<SqlConnection,SqlTransaction>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SqlServerDatabaseVersionUpgrader(SqlServerDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        private SqlServerDbManagerOptions Options { get; }

        #endregion




        #region Instance Properties/Indexer

        private List<SqlServerDbVersionUpgradeStep> UpgradeSteps { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the list of available upgrade steps.
        /// </summary>
        /// <returns>
        ///     The list of available upgrade steps.
        ///     The list is never empty.
        /// </returns>
        public List<SqlServerDbVersionUpgradeStep> GetUpgradeSteps () => new List<SqlServerDbVersionUpgradeStep>(this.UpgradeSteps);

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => this.UpgradeSteps.Any(x => x.RequiresScriptLocator);

        /// <inheritdoc />
        public override int GetMaxVersion (SqlServerDbManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Max() + 1;

        /// <inheritdoc />
        public override int GetMinVersion (SqlServerDbManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Min();

        /// <inheritdoc />
        public override bool Upgrade (SqlServerDbManager manager, int sourceVersion)
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

                SqlServerDbVersionUpgradeStep upgradeStep = this.UpgradeSteps.FirstOrDefault(x => x.SourceVersion == sourceVersion);
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




        /// <inheritdoc />
        public override int GetMaxVersion (IDbManager<SqlConnection, SqlTransaction> manager)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetMinVersion (IDbManager<SqlConnection, SqlTransaction> manager)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Upgrade (IDbManager<SqlConnection, SqlTransaction> manager, int sourceVersion)
        {
            throw new NotImplementedException();
        }
    }
}
