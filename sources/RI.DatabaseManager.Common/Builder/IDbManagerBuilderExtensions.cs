using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

using RI.Abstractions.Builder;
using RI.Abstractions.Composition;
using RI.DatabaseManager.Backup;
using RI.DatabaseManager.Batches;
using RI.DatabaseManager.Batches.Locators;
using RI.DatabaseManager.Cleanup;
using RI.DatabaseManager.Manager;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbManagerBuilder" /> and <see cref="IDbManagerBuilder{TConnection,TTransaction,TParameterTypes,TManager}" />type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class IDbManagerBuilderExtensions
    {
        #region Static Methods

        /// <summary>
        ///     Finishes the configuration and registers all necessary objects/services in an independent and standalone container to construct the intended database manager and its dependencies.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <returns> The built database manager instance. </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        /// <exception cref="InvalidOperationException"> This builder has already been used to build the database manager. </exception>
        /// <exception cref="BuilderException"> Configuration or registration of objects/services failed. </exception>
        public static TManager BuildDbManager <TBuilder, TConnection, TTransaction, TParameterTypes, TManager> (this TBuilder builder)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
            where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            (Type connection, Type transaction, Type parameterTypes, Type manager, Type _, Type _, Type _, Type _, Type _) = builder.DetectDbManagerTypes();

            if (!manager.IsAssignableFrom(typeof(TManager)))
            {
                throw new BuilderException($"The registered database manager type {manager.Name} is not compatible with the requested type {typeof(TManager).Name}.");
            }

            if (connection != typeof(TConnection))
            {
                throw new BuilderException($"The registered database connection type {connection.Name} is not the same as the requested type {typeof(TConnection).Name}.");
            }

            if (transaction != typeof(TTransaction))
            {
                throw new BuilderException($"The registered database transaction type {transaction.Name} is not the same as the requested type {typeof(TTransaction).Name}.");
            }

            if (!parameterTypes.IsEnum)
            {
                throw new BuilderException($"The registered database parameter type {parameterTypes.Name} is not an enumeration.");
            }

            IServiceProvider serviceProvider = builder.BuildStandalone();
            object instance = serviceProvider.GetService(manager);

            if (!(instance is TManager))
            {
                throw new BuilderException($"The built database manager type {instance.GetType().Name} is not compatible with the requested type {typeof(TManager).Name}.");
            }

            return (TManager)instance;
        }

        /// <summary>
        /// Uses a specified batch locator to locate batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="locator"></param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="locator"/> is null.</exception>
        /// <exception cref="InvalidOperationException"> This builder has already been used to build the database manager. </exception>
        public static TBuilder UseBatchLocator <TBuilder, TConnection, TTransaction, TParameterTypes, TManager> (this TBuilder builder, IDbBatchLocator<TConnection, TTransaction, TParameterTypes> locator)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
            where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (locator == null)
            {
                throw new ArgumentNullException(nameof(locator));
            }

            builder.ThrowIfAlreadyBuilt();

            builder.AddTemporary(typeof(TemporaryBatchLocatorRegistration<TConnection, TTransaction, TParameterTypes>), new TemporaryBatchLocatorRegistration<TConnection, TTransaction, TParameterTypes>(locator));

            return builder;
        }

        /// <summary>
        /// Uses scripts from assembly resources as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="nameFormat"> The optional name format used to search for script assembly resources or null to use the default value of <see cref="AssemblyScriptBatchLocator{TConnection,TTransaction,TParameterTypes}.NameFormat"/>. The default value of this parameter is null.</param>
        /// <param name="encoding">The optional encoding used to read the assembly resources or null to use the default value of <see cref="AssemblyScriptBatchLocator{TConnection,TTransaction,TParameterTypes}.Encoding"/>. The default value of this parameter is null.</param>
        /// <param name="commandSeparator">The optional command separator used to split scripts into commands or null to use the default value of <see cref="DbBatchLocatorBase{TConnection,TTransaction,TParameterTypes}.CommandSeparator"/>. The default value of this parameter is null.</param>
        /// <param name="optionsFormat">The optional options format used to extract options from scripts or null to use the default value of <see cref="DbBatchLocatorBase{TConnection,TTransaction,TParameterTypes}.OptionsFormat"/>. The default value of this parameter is null.</param>
        /// <param name="assemblies"> The used assemblies to locate script assembly resources or null or an empty array if the calling assembly should be used.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// <see cref="UseAssemblyScriptBatches{TBuilder,TConnection,TTransaction,TParameterTypes,TManager}"/> creates an <see cref="AssemblyScriptBatchLocator{TConnection,TTransaction,TParameterTypes}"/>. See <see cref="AssemblyScriptBatchLocator{TConnection,TTransaction,TParameterTypes}"/> for more details.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public static TBuilder UseAssemblyScriptBatches<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(this TBuilder builder, string nameFormat = null, Encoding encoding = null, string commandSeparator = null, string optionsFormat = null, params Assembly[] assemblies)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
            where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            assemblies ??= new Assembly[0];

            if (assemblies.Length == 0)
            {
                assemblies = new []{Assembly.GetCallingAssembly()};
            }

            AssemblyScriptBatchLocator<TConnection, TTransaction, TParameterTypes> locator = new AssemblyScriptBatchLocator<TConnection, TTransaction, TParameterTypes>(assemblies);

            if (nameFormat != null)
            {
                locator.NameFormat = nameFormat;
            }

            if (encoding != null)
            {
                locator.Encoding = encoding;
            }

            if (commandSeparator != null)
            {
                locator.CommandSeparator = commandSeparator;
            }
            
            if (optionsFormat != null)
            {
                locator.OptionsFormat = optionsFormat;
            }

            builder.UseBatchLocator<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(locator);

            return builder;
        }

        /// <summary>
        /// Uses callback implementations from assemblies as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="assemblies"> The used assemblies to locate callback implementations or null or an empty array if the calling assembly should be used.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// <see cref="UseAssemblyCallbackBatches{TBuilder,TConnection,TTransaction,TParameterTypes,TManager}"/> creates an <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}"/>. See <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}"/> for more details.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public static TBuilder UseAssemblyCallbackBatches<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(this TBuilder builder, params Assembly[] assemblies)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
            where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            assemblies ??= new Assembly[0];

            if (assemblies.Length == 0)
            {
                assemblies = new[] { Assembly.GetCallingAssembly() };
            }

            AssemblyCallbackBatchLocator<TConnection, TTransaction, TParameterTypes> locator = new AssemblyCallbackBatchLocator<TConnection, TTransaction, TParameterTypes>(assemblies);
            builder.UseBatchLocator<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(locator);

            return builder;
        }

        /// <summary>
        /// Uses programmatically defined scripts and/or callbacks as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <typeparam name="TConnection"> The database connection type. </typeparam>
        /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
        /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="dictionary"> The callback used to configure and populate the dictionary (<see cref="DictionaryBatchLocator{TConnection,TTransaction,TParameterTypes}"/>) with scripts/callbacks.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="dictionary"/> is null.</exception>
        public static TBuilder UseBatches<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(this TBuilder builder, Action<DictionaryBatchLocator<TConnection, TTransaction, TParameterTypes>> dictionary)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TParameterTypes, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
            where TManager : class, IDbManager<TConnection, TTransaction, TParameterTypes>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            
            DictionaryBatchLocator<TConnection, TTransaction, TParameterTypes> locator = new DictionaryBatchLocator<TConnection, TTransaction, TParameterTypes>();
            dictionary(locator);
            builder.UseBatchLocator<TBuilder, TConnection, TTransaction, TParameterTypes, TManager>(locator);

            return builder;
        }

        internal static (Type Connection, Type Transaction, Type ParameterTypes, Type Manager, Type VersionDetector, Type BackupCreator, Type CleanupProcessor, Type VersionUpgrader, Type BatchLocator) DetectDbManagerTypes (this IDbManagerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            List<Type> managerContracts = new List<Type>();

            foreach (CompositionRegistration registration in builder.Registrations)
            {
                if (registration.Contract.IsGenericType)
                {
                    if (registration.Contract.GetGenericTypeDefinition() == typeof(IDbManager<,,>))
                    {
                        managerContracts.Add(registration.Contract);
                    }
                }
            }

            if (managerContracts.Count == 0)
            {
                throw new BuilderException("Database manager type could not be detected (no matching contracts).");
            }

            if (managerContracts.Count != 1)
            {
                throw new BuilderException("Database manager type could not be detected (too many matching contracts).");
            }

            Type[] genericArguments = managerContracts[0]
                .GetGenericArguments();

            Type connection = genericArguments[0];
            Type transaction = genericArguments[1];
            Type parameterTypes = genericArguments[2];

            Type manager = typeof(IDbManager<,,>).MakeGenericType(genericArguments);
            Type versionDetector = typeof(IDbVersionDetector<,,>).MakeGenericType(genericArguments);

            Type backupCreator = typeof(IDbBackupCreator<,,>).MakeGenericType(genericArguments);
            Type cleanupProcessor = typeof(IDbCleanupProcessor<,,>).MakeGenericType(genericArguments);
            Type versionUpgrader = typeof(IDbVersionUpgrader<,,>).MakeGenericType(genericArguments);
            Type batchLocator = typeof(IDbBatchLocator<,,>).MakeGenericType(genericArguments);

            return (connection, transaction, parameterTypes, manager, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, batchLocator);
        }

        [SuppressMessage("ReSharper", "CoVariantArrayConversion")]
        internal static void MergeBatchLocators (this IDbManagerBuilder builder, Type connectionType, Type transactionType)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            Type temporaryRegistrationType = typeof(TemporaryBatchLocatorRegistration<,,>).MakeGenericType(connectionType, transactionType);
            Type mergedLocatorType = typeof(AggregateBatchLocator<,,>).MakeGenericType(connectionType, transactionType);
            Type locatorSequenceType = typeof(IEnumerable<>).MakeGenericType(typeof(IDbBatchLocator<,,>).MakeGenericType(connectionType, transactionType));
            Type finalRegistrationType = typeof(IDbBatchLocator<,,>).MakeGenericType(connectionType, transactionType);

            IList<CompositionRegistration> locatorRegistrations = builder.GetContracts(temporaryRegistrationType);
            object[] locators = locatorRegistrations.Select(x => ((TemporaryBatchLocatorRegistration)x.GetOrCreateInstance()).Instance).ToArray();

            ConstructorInfo constructor = mergedLocatorType.GetConstructor(new []{ locatorSequenceType });
            object mergedLocator = constructor?.Invoke(locators);

            builder.RemoveContracts(temporaryRegistrationType);
            builder.AddSingleton(finalRegistrationType, mergedLocator);
        }

        private abstract class TemporaryBatchLocatorRegistration
        {
            public TemporaryBatchLocatorRegistration(IDbBatchLocator instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                this.Instance = instance;
            }

            public IDbBatchLocator Instance { get; }
        }

        private sealed class TemporaryBatchLocatorRegistration<TConnection, TTransaction, TParameterTypes> : TemporaryBatchLocatorRegistration
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TParameterTypes : Enum
        {
            public TemporaryBatchLocatorRegistration (IDbBatchLocator<TConnection, TTransaction, TParameterTypes> instance)
            : base(instance)
            {
            }

            public new IDbBatchLocator<TConnection, TTransaction, TParameterTypes> Instance => (IDbBatchLocator<TConnection, TTransaction, TParameterTypes>)base.Instance;
        }

        #endregion
    }
}
