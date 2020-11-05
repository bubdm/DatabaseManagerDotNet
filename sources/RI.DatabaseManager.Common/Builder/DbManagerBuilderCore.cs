using System;

using RI.Abstractions.Builder;
using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Builder
{
    internal sealed class DbManagerBuilderCore : BuilderBase, IDbManagerBuilder
    {
        #region Instance Methods

        private object CreateNullInstance (Type connection, Type transaction, Type parameterTypes)
        {
            return Activator.CreateInstance(typeof(DbManagerBuilder.NullInstance<,,>).MakeGenericType(connection, transaction, parameterTypes), true);
        }

        #endregion




        #region Overrides

        protected override void PrepareRegistrations (ILogger logger)
        {
            base.PrepareRegistrations(logger);

            (Type connection, Type transaction, Type parameterTypes, Type manager, Type versionDetector, Type backupCreator, Type cleanupProcessor, Type versionUpgrader, Type batchLocator) = this.DetectDbManagerTypes();

            this.MergeBatchLocators(connection, transaction);

            this.ThrowIfNotExactContractCount(manager, 1);
            this.ThrowIfNotExactContractCount(versionDetector, 1);
            this.ThrowIfNotExactContractCount(batchLocator, 1);

            this.ThrowIfTemporary(manager);
            this.ThrowIfTemporary(versionDetector);
            this.ThrowIfTemporary(batchLocator);

            this.ThrowIfNotMaxContractCount(backupCreator, 1);
            this.ThrowIfNotMaxContractCount(cleanupProcessor, 1);
            this.ThrowIfNotMaxContractCount(versionUpgrader, 1);

            this.AddDefaultSingleton(backupCreator, this.CreateNullInstance(connection, transaction, parameterTypes));
            this.AddDefaultSingleton(cleanupProcessor, this.CreateNullInstance(connection, transaction, parameterTypes));
            this.AddDefaultSingleton(versionUpgrader, this.CreateNullInstance(connection, transaction, parameterTypes));

            this.ThrowIfTemporary(backupCreator);
            this.ThrowIfTemporary(cleanupProcessor);
            this.ThrowIfTemporary(versionUpgrader);
        }

        #endregion
    }
}
