using System;
using System.Data.SQLite;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;




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
    public sealed class SQLiteDatabaseVersionUpgrader : BatchNameBasedDbVersionUpgrader<SQLiteConnection,SQLiteTransaction>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseVersionUpgrader" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDatabaseVersionUpgrader(SQLiteDbManagerOptions options, ILogger logger) : base(options, logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        private new SQLiteDbManagerOptions Options { get; }

        #endregion
    }
}
