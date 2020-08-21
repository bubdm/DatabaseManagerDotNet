using System;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     Implements a single SQL Server database upgrade step which upgrades from a source version to source version + 1.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SqlServerDatabaseVersionUpgradeStep" /> is used by <see cref="SqlServerDatabaseVersionUpgrader" />.
    ///     </para>
    ///     <para>
    ///         Each upgrade step is associated with a source version (<see cref="SourceVersion" />).
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public class SqlServerDatabaseVersionUpgradeStep : SqlServerDatabaseProcessingStep
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDatabaseVersionUpgradeStep" />.
        /// </summary>
        /// <param name="sourceVersion"> The source version this upgrade steps updates. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="sourceVersion" /> is less than zero. </exception>
        public SqlServerDatabaseVersionUpgradeStep (int sourceVersion)
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
