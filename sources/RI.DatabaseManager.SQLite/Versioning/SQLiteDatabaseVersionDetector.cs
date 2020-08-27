using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Implements a database version detector for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDatabaseVersionDetector" /> uses a custom SQL script which is loaded through a script locator using its script name.
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
    public sealed class SQLiteDatabaseVersionDetector : DatabaseVersionDetector<SQLiteConnection, SQLiteTransaction, SQLiteConnectionStringBuilder, SQLiteDatabaseManager, SQLiteDatabaseManagerConfiguration>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDatabaseVersionDetector" />.
        /// </summary>
        /// <param name="scriptName"> The name of the script which performs the version detection. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="scriptName" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="scriptName" /> is an empty string. </exception>
        public SQLiteDatabaseVersionDetector (string scriptName)
        {
            if (scriptName == null)
            {
                throw new ArgumentNullException(nameof(scriptName));
            }

            if (scriptName.IsNullOrEmptyOrWhitespace())
            {
                throw new EmptyStringArgumentException(nameof(scriptName));
            }

            this.ScriptName = scriptName;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the name of the script which performs the version detection.
        /// </summary>
        /// <value>
        ///     The name of the script which performs the version detection.
        /// </value>
        public string ScriptName { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool RequiresScriptLocator => true;

        /// <inheritdoc />
        public override bool Detect (SQLiteDatabaseManager manager, out DbState? state, out int version)
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

        #endregion
    }
}
