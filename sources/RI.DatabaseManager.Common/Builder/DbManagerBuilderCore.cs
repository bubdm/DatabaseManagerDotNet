using System;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.Abstractions.Logging;




namespace RI.DatabaseManager.Builder
{
    internal sealed class DbManagerBuilderCore : BuilderBase, IDbManagerBuilder
    {
        #region Instance Methods

        private object CreateNullInstance (Type connection, Type transaction, Type parameterTypes)
        {
            return
                Activator.CreateInstance(typeof(DbManagerBuilder.NullInstance<,,>).MakeGenericType(connection, transaction, parameterTypes),
                                         true);
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override void PrepareRegistrations (ILogger logger, ICompositionContainer compositionContainer)
        {
            base.PrepareRegistrations(logger, compositionContainer);

            (Type connection, Type transaction, Type parameterTypes, Type manager, Type versionDetector,
                    Type backupCreator, Type cleanupProcessor, Type versionUpgrader, Type batchLocator, Type creator) =
                this.DetectDbManagerTypes();

            Type temporaryBatchLogatorRegistration =
                typeof(IDbManagerBuilderExtensions.TemporaryBatchLocatorRegistration<,,>)
                    .MakeGenericType(connection, transaction, parameterTypes);

            this.ThrowIfNotMinContractCount(temporaryBatchLogatorRegistration, 1);
            this.MergeBatchLocators(connection, transaction, parameterTypes, manager);

            this.ThrowIfNotExactContractCount(manager, 1);
            this.ThrowIfNotExactContractCount(versionDetector, 1);
            this.ThrowIfNotExactContractCount(batchLocator, 1);

            this.ThrowIfTemporary(manager);
            this.ThrowIfTemporary(versionDetector);
            this.ThrowIfTemporary(batchLocator);

            this.ThrowIfNotMaxContractCount(backupCreator, 1);
            this.ThrowIfNotMaxContractCount(cleanupProcessor, 1);
            this.ThrowIfNotMaxContractCount(versionUpgrader, 1);
            this.ThrowIfNotMaxContractCount(creator, 1);

            this.AddDefaultSingleton(backupCreator, this.CreateNullInstance(connection, transaction, parameterTypes));

            this.AddDefaultSingleton(cleanupProcessor,
                                     this.CreateNullInstance(connection, transaction, parameterTypes));

            this.AddDefaultSingleton(versionUpgrader, this.CreateNullInstance(connection, transaction, parameterTypes));
            this.AddDefaultSingleton(creator, this.CreateNullInstance(connection, transaction, parameterTypes));

            this.ThrowIfTemporary(backupCreator);
            this.ThrowIfTemporary(cleanupProcessor);
            this.ThrowIfTemporary(versionUpgrader);
            this.ThrowIfTemporary(creator);
        }

        #endregion
    }
}
