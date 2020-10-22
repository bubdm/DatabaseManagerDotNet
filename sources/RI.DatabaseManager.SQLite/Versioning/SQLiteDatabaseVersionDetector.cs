using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Implements a database version detector for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDatabaseVersionDetector" /> can be used with either a default SQLite cleanup script or with a custom batch.
    /// See <see cref="SQLiteDbManagerOptions"/> for more information.
    ///     </para>
    ///     <para>
    ///         The script must return a scalar value which indicates the current version of the database.
    ///         The script must return -1 to indicate when the database is damaged or in an invalid state or 0 to indicate that the database does not yet exist and needs to be created.
    ///     </para>
    ///     <para>
    ///         If the script contains multiple batches, each batch is executed consecutively.
    ///         The execution stops on the first batch which returns -1.
    ///         If no batch returns -1, the last batch determines the version.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class SQLiteDatabaseVersionDetector : DbVersionDetectorBase<SQLiteConnection, SQLiteTransaction>
    {
        #region Instance Constructor/Destructor

        private SQLiteDbManagerOptions Options { get; }

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseVersionDetector" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options.</param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDatabaseVersionDetector(SQLiteDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        #endregion




        /// <inheritdoc />
        public override bool Detect (IDbManager<SQLiteConnection, SQLiteTransaction> manager, out DbState? state, out int version)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            state = null;
            version = -1;

            try
            {
                List<string> batches = manager.GetScriptBatch(this.ScriptName, true);
                if (batches == null)
                {
                    throw new Exception("Batch retrieval failed for script: " + (this.ScriptName ?? "[null]"));
                }

                using (SQLiteConnection connection = manager.CreateInternalConnection(null, false))
                {
                    using (SQLiteTransaction transaction = connection.BeginTransaction(IsolationLevel.Serializable))
                    {
                        foreach (string batch in batches)
                        {
                            using (SQLiteCommand command = new SQLiteCommand(batch, connection, transaction))
                            {
                                object value = command.ExecuteScalar();
                                version = value.ToInt32FromSQLiteResult() ?? -1;
                                if (version <= -1)
                                {
                                    break;
                                }
                            }
                        }

                        transaction?.Rollback();
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                //TODO: Log: this.Log(LogLevel.Error, "SQLite database version detection failed:{0}{1}", Environment.NewLine, exception.ToDetailedString());
                return false;
            }
        }
    }
}
