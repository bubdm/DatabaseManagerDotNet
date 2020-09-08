using System;
using System.Collections.Generic;

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
    internal sealed class DbManagerBuilderCore : BuilderBase, IDbManagerBuilder
    {
        #region Instance Methods

        private object CreateNullInstance (Type connection, Type transaction, Type manager)
        {
            return Activator.CreateInstance(typeof(DbManagerBuilder.NullInstance<,,>).MakeGenericType(connection, transaction, manager), true);
        }

        #endregion




        #region Overrides

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

        #endregion
    }
}
