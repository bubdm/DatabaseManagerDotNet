using System;
using System.Collections.Generic;
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
    ///     Boilerplate implementation of <see cref="IDbVersionUpgrader" /> and <see cref="IDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}" /> which uses upgrade steps provided by batches.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    /// <para>
    /// This boilerplate implementation uses upgrade steps provided by batches according to their names.
    /// <see cref="GetBatchNamePattern"/> must be implemented in derived types to provide a RegEx pattern which filters the batch names and extracts the source version numbers from each name.
    /// See <see cref="GetBatchNamePattern"/> for additional information.
    /// </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class BatchNameBasedDbVersionUpgrader<TConnection, TTransaction, TParameterTypes> : DbVersionUpgraderBase<TConnection, TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        /// <summary>
        ///     Gets the used database manager options.
        /// </summary>
        /// <value>
        ///     The used database manager options.
        /// </value>
        protected ISupportVersionUpgradeNameFormat Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="BatchNameBasedDbVersionUpgrader{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="options"> The used database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger"/> is null. </exception>
        protected BatchNameBasedDbVersionUpgrader(ISupportVersionUpgradeNameFormat options, ILogger logger)
        : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        /// <inheritdoc />
        public override int GetMaxVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            this.GetSteps(manager, steps);

            if (steps.Count == 0)
            {
                return -1;
            }

            return steps.Keys.OrderByDescending(x => x)
                 .First() + 1;
        }

        /// <inheritdoc />
        public override int GetMinVersion (IDbManager<TConnection, TTransaction, TParameterTypes> manager)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            this.GetSteps(manager, steps);

            if (steps.Count == 0)
            {
                return -1;
            }

            return steps.Keys.OrderBy(x => x)
                        .First();
        }

        /// <inheritdoc />
        public override bool Upgrade (IDbManager<TConnection, TTransaction, TParameterTypes> manager, int sourceVersion)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps = new Dictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>>();
            string namePattern = this.GetSteps(manager, steps);

            if (steps.Count == 0)
            {
                throw new InvalidOperationException($"No batches were provided to {this.GetType().Name} using the name pattern \"{namePattern}\".");
            }

            int minVersion = steps.Keys.OrderBy(x => x)
                                  .First();

            int maxVersion = steps.Keys.OrderByDescending(x => x)
                                  .First() + 1;

            if ((sourceVersion < minVersion) || (sourceVersion > (maxVersion - 1)))
            {
                throw new ArgumentOutOfRangeException(nameof(sourceVersion), $"The specified source version is not within the supported range ({minVersion}...{maxVersion - 1}).");
            }

            try
            {
                this.Log(LogLevel.Information, "Beginning version upgrade step: {0} -> {1}", sourceVersion, sourceVersion + 1);

                IDbBatch<TConnection, TTransaction, TParameterTypes> batch = steps[sourceVersion];

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

        /// <summary>
        /// Gets the upgrade steps as batches, based on <see cref="GetBatchNamePattern"/>.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="steps">The dictionary which is to be filled with the available steps (batches). Keys are source versions, values are the corresponding batch.</param>
        /// <returns>
        /// The RegEx pattern used to filter batches used as version upgrade steps and to extract their source version.
        /// </returns>
        /// <remarks>
        /// <note type="implement">
        /// The default implementation searches all available batch names for matches using <see cref="GetBatchNamePattern"/>.
        /// </note>
        /// </remarks>
        protected virtual string GetSteps (IDbManager<TConnection, TTransaction, TParameterTypes> manager, IDictionary<int, IDbBatch<TConnection, TTransaction, TParameterTypes>> steps)
        {
            string namePattern = this.GetBatchNamePattern();

            var candidates = manager.GetBatchNames();

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
                    if (steps.ContainsKey(sourceVersionValue))
                    {
                        //TODO: #14: Use merging here
                        throw new InvalidOperationException($"Multiple batches provided to {this.GetType().Name} apply to the same source version {sourceVersion}.");

                    }

                    steps.Add(sourceVersionValue, manager.GetBatch(candidate));
                }
            }

            if (steps.Count < 2)
            {
                return namePattern;
            }

            int[] sourceVersions = steps.Keys.OrderBy(x => x).ToArray();

            for (int i1 = 1; i1 < sourceVersions.Length; i1++)
            {
                if (sourceVersions[i1 - 1] != (sourceVersions[i1] - 1))
                {
                    throw new InvalidOperationException($"A non-contiguous set of batches was provided to {this.GetType().Name} using the name pattern \"{namePattern}\" {string.Join(",", sourceVersions.Select(x => x.ToString(CultureInfo.InvariantCulture)))}.");
                }
            }

            return namePattern;
        }

        /// <summary>
        /// Gets the RegEx pattern used to filter batches used as version upgrade steps and to extract their source version.
        /// </summary>
        /// <returns>
        /// The used RegEx pattern.
        /// </returns>
        /// <remarks>
        /// <para>
        /// The extracted version must be the source version, meaning that the corresponding batch upgrades from that source version to source version + 1.
        /// </para>
        /// <note type="implement">
        /// The default implementation returns the value of  the <see cref="ISupportVersionUpgradeNameFormat.VersionUpgradeNameFormat"/> property from <see cref="Options"/>.
        /// </note>
        /// </remarks>
        protected virtual string GetBatchNamePattern () => this.Options.VersionUpgradeNameFormat;
    }
}
