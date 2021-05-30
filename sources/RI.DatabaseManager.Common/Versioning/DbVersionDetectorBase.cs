using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Globalization;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder.Options;
using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Versioning
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbVersionDetector" /> and
    ///     <see cref="IDbVersionDetector{TConnection,TTransaction,TParameterTypes}" />.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database version detector implementations use this base class as it already implements
    ///         most of the database-independent logic defined by <see cref="IDbVersionDetector" /> and
    ///         <see cref="IDbVersionDetector{TConnection,TTransaction,TParameterTypes}" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class
        DbVersionDetectorBase <TConnection, TTransaction, TParameterTypes> : IDbVersionDetector<TConnection,
            TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbVersionDetectorBase{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        /// <param name="options"> The used database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="options" /> or <paramref name="logger" /> is null. </exception>
        protected DbVersionDetectorBase (IDbManagerOptions options, ILogger logger)
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
        ///     Gets the used logger.
        /// </summary>
        /// <value>
        ///     The used logger.
        /// </value>
        protected ILogger Logger { get; }

        /// <summary>
        ///     Gets the used database manager options.
        /// </summary>
        /// <value>
        ///     The used database manager options.
        /// </value>
        protected IDbManagerOptions Options { get; }

        #endregion




        #region Instance Methods

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="format">
        ///     Log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>, etc.
        ///     which are expanded by <paramref name="args" />).
        /// </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log (LogLevel level, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, null, format, args);
        }

        /// <summary>
        ///     Writes a log message.
        /// </summary>
        /// <param name="level"> The log level of the log message. </param>
        /// <param name="exception"> Exception associated with the log message. </param>
        /// <param name="format">
        ///     Optional log message (with optional string expansion arguments such as <c> {0} </c>, <c> {1} </c>
        ///     , etc. which are expanded by <paramref name="args" />).
        /// </param>
        /// <param name="args"> Optional message arguments expanded into <paramref name="format" />. </param>
        protected void Log (LogLevel level, Exception exception, string format, params object[] args)
        {
            this.Logger.Log(level, this.GetType()
                                       .Name, exception, format, args);
        }

        #endregion




        #region Virtuals

        /// <summary>
        ///     Gets the version detection step as batch.
        /// </summary>
        /// <param name="manager"> The used database manager. </param>
        /// <param name="steps"> The available step (batch). </param>
        /// <returns>
        ///     true if the version detection step could be retrieved, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <note type="implement">
        ///         The default implementation uses <see cref="ISupportDefaultDatabaseVersioning.GetDefaultVersioningScript" /> to
        ///         retrieve all version detection commands which are converted to a batch.
        ///     </note>
        /// </remarks>
        protected virtual bool GetVersionDetectionSteps (IDbManager<TConnection, TTransaction, TParameterTypes> manager,
                                                         out IDbBatch<TConnection, TTransaction, TParameterTypes> steps)
        {
            steps = null;
            DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare;
            IsolationLevel? isolationLevel = null;

            string[] commands =
                (this.Options as ISupportDefaultDatabaseVersioning)
                ?.GetDefaultVersioningScript(out transactionRequirement, out isolationLevel);

            if (commands == null)
            {
                return false;
            }

            if (commands.Length == 0)
            {
                return false;
            }

            IDbBatch<TConnection, TTransaction, TParameterTypes> batch = manager.CreateBatch();

            foreach (string command in commands)
            {
                batch.AddScript(command, transactionRequirement, isolationLevel);
            }

            steps = batch;

            return true;
        }

        /// <summary>
        ///     Tries to convert the result of an executed version detection command to an <see cref="int" />.
        /// </summary>
        /// <param name="value"> The command result. </param>
        /// <returns>
        ///     The converted value or null if the result cannot be converted.
        /// </returns>
        protected virtual int? ToInt32FromResult (object value)
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
                return (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is long)
            {
                long testValue = (long)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is ulong)
            {
                ulong testValue = (ulong)value;
                return (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is float)
            {
                float testValue = (float)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is double)
            {
                double testValue = (double)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is decimal)
            {
                decimal testValue = (decimal)value;
                return (testValue < int.MinValue) || (testValue > int.MaxValue) ? null : (int)testValue;
            }

            if (value is string)
            {
                return int.Parse((string)value, CultureInfo.InvariantCulture);
            }

            return null;
        }

        #endregion




        #region Interface: IDbVersionDetector

        /// <inheritdoc />
        bool IDbVersionDetector.Detect (IDbManager manager, out DbState? state, out int version) =>
            this.Detect((IDbManager<TConnection, TTransaction, TParameterTypes>)manager, out state, out version);

        #endregion




        #region Interface: IDbVersionDetector<TConnection,TTransaction,TParameterTypes>

        /// <inheritdoc />
        public virtual bool Detect (IDbManager<TConnection, TTransaction, TParameterTypes> manager, out DbState? state,
                                    out int version)
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            state = null;
            version = -1;

            IDbBatch<TConnection, TTransaction, TParameterTypes> steps;
            bool result = this.GetVersionDetectionSteps(manager, out steps);

            if ((!result) || (steps == null))
            {
                this.Log(LogLevel.Error, "No version detection steps available to create database.");
                return false;
            }

            IList<DbBatch<TConnection, TTransaction, TParameterTypes>> commands = steps.SplitCommands();

            try
            {
                this.Log(LogLevel.Information, "Beginning version detection of database.");

                foreach (DbBatch<TConnection, TTransaction, TParameterTypes> command in commands)
                {
                    if (!manager.ExecuteBatch(command, false, false))
                    {
                        this.Log(LogLevel.Error, "Failed version detection of database.");
                        return false;
                    }

                    object value = command.GetResult();
                    version = this.ToInt32FromResult(value) ?? -1;

                    if (version <= -1)
                    {
                        break;
                    }
                }

                this.Log(LogLevel.Information, "Finished version detection of database.");
                return true;
            }
            catch (Exception exception)
            {
                this.Log(LogLevel.Error, "Failed version detection of database:{0}{1}", Environment.NewLine,
                         exception.ToString());

                return false;
            }
        }

        #endregion
    }
}
