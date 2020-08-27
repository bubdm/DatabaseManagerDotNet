using System;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Implements a single SQLite database upgrade step which upgrades from a source version to source version + 1.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDbVersionUpgradeStep" /> is used by <see cref="SQLiteDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         Each upgrade step is associated with a source version (<see cref="SourceVersion" />).
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDbVersionUpgradeStep : SQLiteDbProcessingStep
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbVersionUpgradeStep" />.
        /// </summary>
        /// <param name="sourceVersion"> The source version this upgrade steps updates. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="sourceVersion" /> is less than zero. </exception>
        public SQLiteDbVersionUpgradeStep (int sourceVersion)
        {
            if (sourceVersion < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceVersion));
            }

            this.SourceVersion = sourceVersion;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the source version this upgrade steps updates.
        /// </summary>
        /// <value>
        ///     The source version this upgrade steps updates.
        /// </value>
        public int SourceVersion { get; }

        #endregion
    }
}
