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

            this.ThrowIfNotMaxContractCount(backupCreator, 1);
            this.ThrowIfNotMaxContractCount(cleanupProcessor, 1);
            this.ThrowIfNotMaxContractCount(versionUpgrader, 1);
            this.ThrowIfNotMaxContractCount(scriptLocator, 1);

            if (this.CountContracts(backupCreator) == 0)
            {
                this.AddSingleton(backupCreator, this.CreateNullInstance(connection, transaction, manager));
            }

            if (this.CountContracts(cleanupProcessor) == 0)
            {
                this.AddSingleton(cleanupProcessor, this.CreateNullInstance(connection, transaction, manager));
            }

            if (this.CountContracts(versionUpgrader) == 0)
            {
                this.AddSingleton(versionUpgrader, this.CreateNullInstance(connection, transaction, manager));
            }

            if (this.CountContracts(scriptLocator) == 0)
            {
                this.AddSingleton(scriptLocator, this.CreateNullInstance(connection, transaction, manager));
            }
        }

        private (Type Connection, Type Transaction, Type Manager, Type VersionDetector, Type BackupCreator, Type CleanupProcessor, Type VersionUpgrader, Type ScriptLocator) DetectDbManagerTypes ()
        {
            foreach (CompositionRegistration registration in this.Registrations)
            {
                if (registration.Contract.IsGenericType)
                {
                    if (registration.Contract.GetGenericTypeDefinition() == typeof(IDatabaseManager<,,>))
                    {
                        Type[] genericArguments = registration.Contract.GetGenericArguments();

                        Type connection = genericArguments[0];
                        Type transaction = genericArguments[1];

                        Type manager = typeof(IDatabaseManager<,,>).MakeGenericType(genericArguments);
                        Type versionDetector = typeof(IDatabaseVersionDetector<,,>).MakeGenericType(genericArguments);

                        Type backupCreator = typeof(IDatabaseBackupCreator<,,>).MakeGenericType(genericArguments);
                        Type cleanupProcessor = typeof(IDatabaseCleanupProcessor<,,>).MakeGenericType(genericArguments);
                        Type versionUpgrader = typeof(IDatabaseVersionUpgrader<,,>).MakeGenericType(genericArguments);

                        Type scriptLocator = typeof(IDatabaseScriptLocator);

                        return (connection, transaction, manager, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, scriptLocator);
                    }
                }
            }

            throw new InvalidOperationException("Database manager type could not be detected.");
        }

        private object CreateNullInstance (Type connection, Type transaction, Type manager) => Activator.CreateInstance(typeof(NullInstance<,,>).MakeGenericType(connection, transaction, manager), true);

        /// <summary>
        /// Null object used if no registration for <see cref="IDatabaseBackupCreator{TConnection,TTransaction,TManager}"/>, <see cref="IDatabaseCleanupProcessor{TConnection,TTransaction,TManager}"/>, <see cref="IDatabaseVersionUpgrader{TConnection,TTransaction,TManager}"/>, or <see cref="IDatabaseScriptLocator"/> is provided.
        /// </summary>
        /// <threadsafety static="false" instance="false" />
        public sealed class NullInstance<TConnection, TTransaction, TManager> : IDatabaseBackupCreator<TConnection, TTransaction, TManager>, IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>, IDatabaseVersionUpgrader<TConnection, TTransaction, TManager>, IDatabaseScriptLocator
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TManager : class, IDatabaseManager<TConnection, TTransaction, TManager>
        {
            internal NullInstance () { }

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor<TConnection, TTransaction, TManager>.Cleanup (TManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMaxVersion (IDatabaseManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            int IDatabaseVersionUpgrader.GetMinVersion (IDatabaseManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.Upgrade (IDatabaseManager manager, int sourceVersion) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.Cleanup (IDatabaseManager manager) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.SupportsRestore => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Backup (IDatabaseManager manager, object backupTarget) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseBackupCreator.Restore (IDatabaseManager manager, object backupSource) => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseCleanupProcessor.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            bool IDatabaseVersionUpgrader.RequiresScriptLocator => throw new NotImplementedException();

            /// <inheritdoc />
            string IDatabaseScriptLocator.BatchSeparator
            {
                get => throw new NotImplementedException();
                set => throw new NotImplementedException();
            }

            /// <inheritdoc />
            List<string> IDatabaseScriptLocator.GetScriptBatch (IDatabaseManager manager, string name, bool preprocess) => throw new NotImplementedException();

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
