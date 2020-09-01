using System;
using System.Collections.Generic;
using System.Data.Common;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Scripts;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Database manager builder used to configure and build database managers.
    /// </summary>
    /// <remarks>
    ///     <note type="important">
    ///         <see cref="IBuilder.Build" /> must be called for actually constructing a database manager.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbManagerBuilder : BuilderBase
    {
        /// <inheritdoc />
        protected override void PrepareRegistrations (ILogger logger)
        {
            base.PrepareRegistrations(logger);
            
            (Type connection, Type transaction, Type manager, Type versionDetector, Type backupCreator, Type cleanupProcessor, Type versionUpgrader, Type scriptLocator) = this.DetectDbManagerTypes();

            this.ThrowIfNotExactContractCount(manager, 1);
            this.ThrowIfNotExactContractCount(versionDetector, 1);

            this.ThrowIfTemporary(manager);
            this.ThrowIfTemporary(versionDetector);

            this.ThrowIfNotMaxContractCount(backupCreator, 1);
            this.ThrowIfNotMaxContractCount(cleanupProcessor, 1);
            this.ThrowIfNotMaxContractCount(versionUpgrader, 1);
            this.ThrowIfNotMaxContractCount(scriptLocator, 1);

            this.AddDefaultSingleton(backupCreator, this.CreateNullInstance(connection, transaction, manager));
            this.AddDefaultSingleton(cleanupProcessor, this.CreateNullInstance(connection, transaction, manager));
            this.AddDefaultSingleton(versionUpgrader, this.CreateNullInstance(connection, transaction, manager));
            this.AddDefaultSingleton(scriptLocator, this.CreateNullInstance(connection, transaction, manager));

            this.ThrowIfTemporary(backupCreator);
            this.ThrowIfTemporary(cleanupProcessor);
            this.ThrowIfTemporary(versionUpgrader);
            this.ThrowIfTemporary(scriptLocator);
        }

        private (Type Connection, Type Transaction, Type Manager, Type VersionDetector, Type BackupCreator, Type CleanupProcessor, Type VersionUpgrader, Type ScriptLocator) DetectDbManagerTypes ()
        {
            foreach (CompositionRegistration registration in this.Registrations)
            {
                if (registration.Contract.IsGenericType)
                {
                    if (registration.Contract.GetGenericTypeDefinition() == typeof(IDbManager<,,>))
                    {
                        Type[] genericArguments = registration.Contract.GetGenericArguments();

                        Type connection = genericArguments[0];
                        Type transaction = genericArguments[1];

                        Type manager = typeof(IDbManager<,,>).MakeGenericType(genericArguments);
                        Type versionDetector = typeof(IDatabaseVersionDetector<,,>).MakeGenericType(genericArguments);

                        Type backupCreator = typeof(IDatabaseBackupCreator<,,>).MakeGenericType(genericArguments);
                        Type cleanupProcessor = typeof(IDatabaseCleanupProcessor<,,>).MakeGenericType(genericArguments);
                        Type versionUpgrader = typeof(IDatabaseVersionUpgrader<,,>).MakeGenericType(genericArguments);

                        Type scriptLocator = typeof(IDbScriptLocator);

                        return (connection, transaction, manager, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, scriptLocator);
                    }
                }
            }

            throw new InvalidOperationException("Database manager type could not be detected.");
        }

        private object CreateNullInstance (Type connection, Type transaction, Type manager) => Activator.CreateInstance(typeof(NullInstance<,,>).MakeGenericType(connection, transaction, manager), true);

        /// <summary>
        /// Null object used if no registration for <see cref="IDatabaseBackupCreator{TConnection,TTransaction,TManager}"/>, <see cref="IDatabaseCleanupProcessor{TConnection,TTransaction,TManager}"/>, <see cref="IDatabaseVersionUpgrader{TConnection,TTransaction,TManager}"/>, or <see cref="IDbScriptLocator"/> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance<TConnection, TTransaction, TManager> : IDatabaseBackupCreator<TConnection, TTransaction, TManager>, IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>, IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>, IDbScriptLocator
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TManager : class, IDbManager<TConnection, TTransaction, TManager>
        {
            internal NullInstance () { }

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>.Cleanup (TManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMaxVersion (IDbManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMinVersion (IDbManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.Upgrade (IDbManager manager, int sourceVersion) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.Cleanup (IDbManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.SupportsRestore => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Backup (IDbManager manager, object backupTarget) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Restore (IDbManager manager, object backupSource) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            string IDbScriptLocator.DefaultBatchSeparator
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            /// <inheritdoc />
            List<string> IDbScriptLocator.GetScriptBatches (IDbManager manager, string name, string batchSeparator, bool preprocess) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction, TManager>.Backup (TManager manager, object backupTarget) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator<TConnection, TTransaction, TManager>.Restore (TManager manager, object backupTarget) => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.GetMaxVersion (TManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.GetMinVersion (TManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>.Upgrade (TManager manager, int sourceVersion) => throw new NotImplementedException();
        }
    }
}
