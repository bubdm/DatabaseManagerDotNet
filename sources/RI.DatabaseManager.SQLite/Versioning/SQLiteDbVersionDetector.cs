using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Implements a database version detector for SQLite databases.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="SQLiteDbVersionDetector" /> can be used with either a default SQLite cleanup script or with a custom batch.
    ///         See <see cref="SQLiteDbManagerOptions" /> for more information.
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
    public sealed class SQLiteDbVersionDetector : DbVersionDetectorBase<SQLiteConnection, SQLiteTransaction, DbType>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SQLiteDbVersionDetector" />.
        /// </summary>
        /// <param name="options"> The used SQLite database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        public SQLiteDbVersionDetector (SQLiteDbManagerOptions options, ILogger logger) : base(logger)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            this.Options = options;
        }

        #endregion




        #region Instance Properties/Indexer

        private SQLiteDbManagerOptions Options { get; }

        private int? ToInt32FromSQLiteResult(object value)
        {
            if (value == null)
            {
                return null;
            }

            if (value is DBNull)
            {
                return null;
            }

            if (value is sbyte)
            {
                return (sbyte)value;
            }

            if (value is byte)
            {
                return (byte)value;
            }

            if (value is short)
            {
                return (short)value;
            }

            if (value is ushort)
            {
                return (ushort)value;
            }

            if (value is int)
            {
                return (int)value;
            }

            if (value is uint)
            {
                uint testValue = (uint)value;
                return (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is long)
            {
                long testValue = (long)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is ulong)
            {
                ulong testValue = (ulong)value;
                return (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is float)
            {
                float testValue = (float)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is double)
            {
                double testValue = (double)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is decimal)
            {
                decimal testValue = (decimal)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? (int?)null : (int)testValue;
            }

            if (value is string)
            {
                return int.Parse((string)value, CultureInfo.InvariantCulture);
            }

            return null;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Detect (IDbManager<SQLiteConnection, SQLiteTransaction, DbType> manager, out DbState? state, out int version)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            state = null;
            version = -1;

            try
            {
                IDbBatch<SQLiteConnection, SQLiteTransaction, DbType> batch;

                if (!this.Options.CustomVersionDetectionBatch.IsEmpty())
                {
                    batch = this.Options.CustomVersionDetectionBatch;
                }
                else if (!string.IsNullOrWhiteSpace(this.Options.CustomVersionDetectionBatchName))
                {
                    batch = manager.GetBatch(this.Options.CustomVersionDetectionBatchName);
                }
                else
                {
                    //TODO: Specify isolation level
                    batch = new DbBatch<SQLiteConnection, SQLiteTransaction, DbType>();

                    foreach (string command in this.Options.GetDefaultVersionDetectionScript())
                    {
                        batch.AddScript(command, DbBatchTransactionRequirement.Disallowed);
                    }
                }

                IList<DbBatch<SQLiteConnection, SQLiteTransaction, DbType>> commands = batch.SplitCommands();

                foreach (DbBatch<SQLiteConnection, SQLiteTransaction, DbType> command in commands)
                {
                    if (!manager.ExecuteBatch(command, false, false))
                    {
                        return false;
                    }

                    object value = command.GetResult();
                    version = this.ToInt32FromSQLiteResult(value) ?? -1;

                    if (version <= -1)
                    {
                        break;
                    }
                }

                return true;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "SQLite database version detection failed:{0}{1}", Environment.NewLine, exception.ToString());
                return false;
            }
        }

        #endregion
    }
}
