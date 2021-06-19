﻿using System;
using System.Collections.Generic;
using System.Data;

using Microsoft.Data.SqlClient;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Builder;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Creation;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Manager
{
    /// <summary>
    ///     Implements a database manager for Microsoft SQL Server databases.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class SqlServerDbManager : DbManagerBase<SqlConnection, SqlTransaction, SqlDbType,
        SqlParameterCollection, SqlParameter, SqlServerDbManagerOptions>
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="SqlServerDbManager" />.
        /// </summary>
        /// <param name="options"> The used SQL Server database manager options. </param>
        /// <param name="logger"> The used logger. </param>
        /// <param name="batchLocator"> The used batch locator. </param>
        /// <param name="versionDetector"> The used version detector. </param>
        /// <param name="backupCreator"> The used backup creator, if any. </param>
        /// <param name="cleanupProcessor"> The used cleanup processor, if any. </param>
        /// <param name="versionUpgrader"> The used version upgrader, if any. </param>
        /// <param name="creator"> The used database creator, if any. </param>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="options" />, <paramref name="logger" />,
        ///     <paramref name="batchLocator" />, or <paramref name="versionDetector" /> is null.
        /// </exception>
        public SqlServerDbManager (SqlServerDbManagerOptions options, ILogger logger,
                                   IDbBatchLocator<SqlConnection, SqlTransaction, SqlDbType> batchLocator,
                                   IDbVersionDetector<SqlConnection, SqlTransaction, SqlDbType> versionDetector,
                                   IDbBackupCreator<SqlConnection, SqlTransaction, SqlDbType> backupCreator,
                                   IDbCleanupProcessor<SqlConnection, SqlTransaction, SqlDbType> cleanupProcessor,
                                   IDbVersionUpgrader<SqlConnection, SqlTransaction, SqlDbType> versionUpgrader,
                                   IDbCreator<SqlConnection, SqlTransaction, SqlDbType> creator) :
            base(logger, batchLocator, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, creator,
                 options) { }

        #endregion




        #region Instance Methods

        internal SqlConnection CreateInternalConnection (string connectionStringOverride)
        {
            SqlConnectionStringBuilder connectionString =
                new SqlConnectionStringBuilder(connectionStringOverride ??
                                               this.Options.ConnectionString.ConnectionString);

            SqlConnection connection = new SqlConnection(connectionString.ConnectionString);
            connection.Open();

            return connection;
        }

        internal SqlTransaction CreateInternalTransaction (string connectionStringOverride,
                                                           IsolationLevel isolationLevel)
        {
            SqlConnection connection = this.CreateInternalConnection(connectionStringOverride);
            return connection.BeginTransaction(isolationLevel);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool SupportsBackupImpl => false;

        /// <inheritdoc />
        protected override bool SupportsCleanupImpl => true;

        /// <inheritdoc />
        protected override bool SupportsCreateImpl => true;

        /// <inheritdoc />
        protected override bool SupportsReadOnlyConnectionsImpl => false;

        /// <inheritdoc />
        protected override bool SupportsRestoreImpl => false;

        /// <inheritdoc />
        protected override bool SupportsUpgradeImpl => true;

        /// <inheritdoc />
        protected override SqlConnection CreateConnectionImpl (bool readOnly) => this.CreateInternalConnection(null);

        /// <inheritdoc />
        protected override SqlTransaction CreateTransactionImpl (bool readOnly, IsolationLevel isolationLevel) =>
            this.CreateInternalTransaction(null, isolationLevel);

        /// <inheritdoc />
        protected override List<object> ExecuteCommandScriptImpl (SqlConnection connection, SqlTransaction transaction,
                                                                  DbBatchExecutionType executionType,
                                                                  string script,
                                                                  IDbBatchCommandParameterCollection<SqlDbType>
                                                                      parameters,
                                                                  out string error, out Exception exception)
        {
            error = null;
            exception = null;

            List<object> results = new List<object>();

            this.Log(LogLevel.Debug, "Executing SQL Server database processing command script:{0}{1}",
                     Environment.NewLine, script);

            using (SqlCommand command = transaction == null ? new SqlCommand(script, connection)
                                            : new SqlCommand(script, connection, transaction))
            {
                foreach (IDbBatchCommandParameter<SqlDbType> parameter in parameters)
                {
                    command.Parameters.Add(parameter.Name, parameter.Type)
                           .Value = parameter.Value;
                }

                switch (executionType)
                {
                    case DbBatchExecutionType.Reader:
                        SqlDataReader reader = command.ExecuteReader(CommandBehavior.Default);

                        while (reader.Read())
                        {
                            for (int i1 = 0; i1 < reader.FieldCount; i1++)
                            {
                                object readerResult = reader[i1];
                                results.Add(readerResult);
                            }
                        }

                        break;

                    case DbBatchExecutionType.Scalar:
                        object resultScalar = command.ExecuteScalar();
                        results.Add(resultScalar);
                        break;

                    case DbBatchExecutionType.NonQuery:
                        int resultNonQuery = command.ExecuteNonQuery();
                        results.Add(resultNonQuery);
                        break;
                }

                return results;
            }
        }

        /// <inheritdoc />
        protected override IsolationLevel GetDefaultIsolationLevel () => IsolationLevel.ReadCommitted;

        #endregion
    }
}
