using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Linq;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements a database version upgrader for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         See <see cref="SQLiteDbManagerOptions" /> for more information.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDatabaseVersionUpgrader : DbVersionUpgraderBase<SQLiteConnection,SQLiteTransaction>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDatabaseVersionUpgrader(SQLiteDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        private SQLiteDbManagerOptions Options { get; }

        #endregion




        #region Instance Properties/Indexer

        private List<SQLiteDbVersionUpgradeStep> UpgradeSteps { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Gets the list of available upgrade steps.
        /// </summary>
        /// <returns>
        ///     The list of available upgrade steps.
        ///     The list is never empty.
        /// </returns>
        public List<SQLiteDbVersionUpgradeStep> GetUpgradeSteps () => new List<SQLiteDbVersionUpgradeStep>(this.UpgradeSteps);

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => this.UpgradeSteps.Any(x => x.RequiresScriptLocator);

        /// <inheritdoc />
        public override int GetMaxVersion (SQLiteDbManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Max() + 1;

        /// <inheritdoc />
        public override int GetMinVersion (SQLiteDbManager manager) => this.UpgradeSteps.Select(x => x.SourceVersion).Min();

        /// <inheritdoc />
        public override bool Upgrade (SQLiteDbManager manager, int sourceVersion)
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
                //TODO: Log: this.Log(LogLevel.Debug, "Beginning SQLite database upgrade step: SourceVersion=[{0}]; Connection=[{1}]", sourceVersion, manager.Configuration.ConnectionString);

                SQLiteDbVersionUpgradeStep upgradeStep = this.UpgradeSteps.FirstOrDefault(x => x.SourceVersion == sourceVersion);
                if (upgradeStep == null)
                {
                    throw new Exception("No upgrade step found for source version: " + sourceVersion);
                }

                using (SQLiteConnection connection = manager.CreateInternalConnection(null, false))
                {
                    using (SQLiteTransaction transaction = upgradeStep.RequiresTransaction ? connection.BeginTransaction(IsolationLevel.Serializable) : null)
                    {
                        upgradeStep.Execute(manager, connection, transaction);
                        transaction?.Commit();
                    }
                }

                //TODO: Log: this.Log(LogLevel.Debug, "Finished SQLite database upgrade step: SourceVersion=[{0}]; Connection=[{1}]", sourceVersion, manager.Configuration.ConnectionString);

                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQLite database upgrade step failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }

        #endregion




        /// <inheritdoc />
        public override int GetMaxVersion (IDbManager<SQLiteConnection, SQLiteTransaction> manager)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override int GetMinVersion (IDbManager<SQLiteConnection, SQLiteTransaction> manager)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc />
        public override bool Upgrade (IDbManager<SQLiteConnection, SQLiteTransaction> manager, int sourceVersion)
        {
            throw new NotImplementedException();
        }
    }
}
