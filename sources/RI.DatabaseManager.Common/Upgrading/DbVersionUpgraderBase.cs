using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbVersionUpgrader" /> and <see cref="IDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database version upgrader implementations use this base class as it already implements most of the database-independent logic defined by <see cref="IDbVersionUpgrader" /> and <see cref="IDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbVersionUpgraderBase <TConnection, TTransaction, TParameterTypes> : IDbVersionUpgrader<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbVersionUpgraderBase{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="options"> The used database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        protected DbVersionUpgraderBase (IDbManagerOptions options, ILogger logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this.Options = options;
            this.Logger = logger;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used database manager options.
        /// </summary>
        /// <value>
        ///     The used database manager options.
        /// </value>
        protected IDbManagerOptions Options { get; }

        /// <summary>
        ///     Gets the used logger.
        /// </summary>
        /// <value>
        ///     The used logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="format"> Log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded by <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log(LogLevel level, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, null, format, args);
        }

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="exception"> Exception associated with the log message. </param>
        /// <param name="format"> Optional log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc. which are expanded by <paramref name="args" />). </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log(LogLevel level, Exception exception, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, exception, format, args);
        }

        #endregion




        #region Interface: IDbVersionUpgrader<TConnection,TTransaction>

        /// <summary>
        /// Gets the upgrade steps as batches.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="steps">The dictionary which is to be filled with the available steps (batches). Keys are source versions, values are the corresponding batch.</param>
        /// <returns>
        /// true if the upgrade steps could be retrieved, false otherwise.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// The default implementation searches all available batch names for matches using <see cref="GetUpgradeBatchNamePattern"/> which then are used as upgrade step batches.
        /// </note>
        /// </remarks>
        protected virtual bool GetUpgradeSteps(IDbManager<TConnection, TTransaction, TParameterTypes> manager, IDictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps)
        {
            string namePattern = this.GetUpgradeBatchNamePattern();

            if (string.IsNullOrWhiteSpace(namePattern))
            {
                return false;
            }

            ISet<string> candidates = manager.GetBatchNames();
            Dictionary<int, List<IDbBatch<TConnection, TTransaction, TParameterTypes>>> candidateSteps = new Dictionary<int, List<IDbBatch<TConnection, TTransaction, TParameterTypes>>>();

            foreach (string candidate in candidates)
            {
                Match match = Regex.Match(candidate, namePattern, RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

                if (!match.Success)
                {
                    continue;
                }

                string sourceVersion = match.Groups["sourceVersion"]
                                            ?.Value;

                if (string.IsNullOrWhiteSpace(sourceVersion))
                {
                    continue;
                }

                if (int.TryParse(sourceVersion, NumberStyles.None, CultureInfo.InvariantCulture, out int sourceVersionValue))
                {
                    if (!candidateSteps.ContainsKey(sourceVersionValue))
                    {
                        candidateSteps.Add(sourceVersionValue, new List<IDbBatch<TConnection, TTransaction, TParameterTypes>>());

                    }

                    candidateSteps[sourceVersionValue].Add(manager.GetBatch(candidate));
                }
            }

            foreach (KeyValuePair<int, List<IDbBatch<TConnection, TTransaction, TParameterTypes>>> candidate in candidateSteps)
            {
                steps.Add(candidate.Key, candidate.Value.MergeCommands());
            }

            if (steps.Count < 2)
            {
                return steps.Count > 0;
            }

            int[] sourceVersions = steps.Keys.OrderBy(x => x).ToArray();

            for (int i1 = 1; i1 < sourceVersions.Length; i1++)
            {
                if (sourceVersions[i1 - 1] != (sourceVersions[i1] - 1))
                {
                    throw new InvalidOperationException($"A non-contiguous set of batches was provided to {this.GetType().Name} using the name pattern \"{namePattern}\": {string.Join(",", sourceVersions.Select(x => x.ToString(CultureInfo.InvariantCulture)))}.");
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the creation steps as batches.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="steps">The list which is to be filled with the available steps (batches). </param>
        /// <returns>
        /// true if the creation steps could be retrieved, false otherwise.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// The default implementation uses <see cref="GetDefaultCreationCommands"/> to retrieve all creation commands which are converted to batches (one batch per command).
        /// </note>
        /// </remarks>
        protected virtual bool GetCreationSteps (IDbManager<TConnection, TTransaction, TParameterTypes> manager, IList<IDbBatch<TConnection, TTransaction, TParameterTypes>> steps)
        {
            string[] commands = this.GetDefaultCreationCommands(out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel) ?? new string[0];

            if (commands.Length == 0)
            {
                return false;
            }

            foreach (string command in commands)
            {
                IDbBatch<TConnection, TTransaction, TParameterTypes> batch = manager.CreateBatch();
                batch.AddScript(command, transactionRequirement, isolationLevel);
                steps.Add(batch);
            }

            return steps.Count > 0;
        }

        /// <summary>
        /// Gets the RegEx pattern used to filter batches used as version upgrade steps and to extract their source version.
        /// </summary>
        /// <returns>
        /// The used RegEx pattern or null if none is available.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The extracted version must be the source version, meaning that the corresponding batch upgrades from that source version to source version + 1.
        /// </para>
        /// <note type="implement">
        /// The default implementation returns the value of  the <see cref="ISupportVersionUpgradeNameFormat.VersionUpgradeNameFormat"/> property from <see cref="Options"/>.
        /// </note>
        /// </remarks>
        protected virtual string GetUpgradeBatchNamePattern() => (this.Options as ISupportVersionUpgradeNameFormat)?.VersionUpgradeNameFormat;

        /// <summary>
        /// Gets the default database creation commands.
        /// </summary>
        /// <param name="transactionRequirement">The transaction requirement.</param>
        /// <param name="isolationLevel">The isolation level requirement.</param>
        /// <returns>
        /// The default database creation commands or null if none are available.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// The default implementation returns the value of  the <see cref="ISupportDatabaseCreation.GetDefaultSetupScript"/> property from <see cref="Options"/>.
        /// </note>
        /// </remarks>
        protected virtual string[] GetDefaultCreationCommands(out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.DontCare;
            isolationLevel = null;
            return (this.Options as ISupportDatabaseCreation)?.GetDefaultSetupScript(out transactionRequirement, out isolationLevel);
        }

        /// <inheritdoc />
        int IDbVersionUpgrader.GetMaxVersion (IDbManager manager)
        {
            return this.GetMaxVersion((IDbManager<TConnection, TTransaction, TParameterTypes>)manager);
        }

        /// <inheritdoc />
        public virtual int GetMaxVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            this.GetUpgradeSteps(manager, steps);

            if (steps.Count == 0)
            {
                return -1;
            }

            return steps.Keys.OrderByDescending(x => x)
                        .First() + 1;
        }

        /// <inheritdoc />
        int IDbVersionUpgrader.GetMinVersion (IDbManager manager)
        {
            return this.GetMinVersion((IDbManager<TConnection, TTransaction, TParameterTypes>)manager);
        }

        /// <inheritdoc />
        public virtual int GetMinVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            this.GetUpgradeSteps(manager, steps);

            if (steps.Count == 0)
            {
                return -1;
            }

            return steps.Keys.OrderBy(x => x)
                        .First();
        }

        /// <inheritdoc />
        public virtual bool Upgrade (IDbManager<TConnection, TTransaction, TParameterTypes> manager, int sourceVersion)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            List<IDbBatch<TConnection, TTransaction, TParameterTypes>> creationSteps = new List<IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            bool creationStepsResult = this.GetCreationSteps(manager, creationSteps);

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> upgradeSteps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            bool upgradeStepsResult = this.GetUpgradeSteps(manager, upgradeSteps);

            if ((!creationStepsResult) || (creationSteps.Count == 0))
            {
                this.Log(LogLevel.Information, "No dedicated creation steps available to create database (might be part of first upgrade step).");
                creationStepsResult = false;
            }

            if ((!upgradeStepsResult) || (upgradeSteps.Count == 0))
            {
                this.Log(LogLevel.Error, "No upgrade steps available to create or upgrade database.");
                return false;
            }

            int minVersion = upgradeSteps.Keys.OrderBy(x => x)
                                         .First();

            int maxVersion = upgradeSteps.Keys.OrderByDescending(x => x)
                                         .First() + 1;

            if ((sourceVersion < minVersion) || (sourceVersion > (maxVersion - 1)))
            {
                throw new ArgumentOutOfRangeException(nameof(sourceVersion), $"The specified source version ({sourceVersion}) is not within the supported range ({minVersion}...{maxVersion - 1}).");
            }
            
            if ((manager.State == DbState.New) && creationStepsResult)
            {
                try
                {
                    this.Log(LogLevel.Information, "Beginning database creation");

                    foreach (IDbBatch<TConnection, TTransaction, TParameterTypes> step in creationSteps)
                    {
                        bool result = manager.ExecuteBatch(step, false, false);

                        if (!result)
                        {
                            this.Log(LogLevel.Error, "Failed database creation");
                            return false;
                        }
                    }

                    this.Log(LogLevel.Information, "Finished database creation");
                }
                catch (Exception exception)
                {
                    this.Log(LogLevel.Error, "Failed database creation:{0}{1}", Environment.NewLine, exception.ToString());
                    return false;
                }
            }
            
            try
            {
                this.Log(LogLevel.Information, "Beginning version upgrade step: {0} -> {1}", sourceVersion, sourceVersion + 1);

                IDbBatch<TConnection, TTransaction, TParameterTypes> batch = upgradeSteps[sourceVersion];

                bool result = manager.ExecuteBatch(batch, false, true);

                if (result)
                {
                    this.Log(LogLevel.Information, "Finished version upgrade step: {0} -> {1}", sourceVersion, sourceVersion + 1);
                }
                else
                {
                    this.Log(LogLevel.Error, "Failed version upgrade step: {0} -> {1}", sourceVersion, sourceVersion + 1);
                }

                return result;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "Failed version upgrade step:{0} -> {1}{2}{3}", sourceVersion, sourceVersion + 1, Environment.NewLine, exception.ToString());
                return false;
            }
        }

        /// <inheritdoc />
        bool IDbVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion)
        {
            return this.Upgrade((IDbManager<TConnection, TTransaction, TParameterTypes>)manager, sourceVersion);
        }

        #endregion
    }
}
