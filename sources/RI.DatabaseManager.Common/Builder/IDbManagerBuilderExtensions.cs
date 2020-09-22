using System;
using System.Collections.Generic;
using System.Data.Common;
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
using RI.DatabaseManager.Scripts;
using RI.DatabaseManager.Upgrading;
using RI.DatabaseManager.Versioning;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="IDbManagerBuilder" /> and <see cref="IDbManagerBuilder{TConnection,TTransaction,TManager}" />type.
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
        /// <typeparam name="TManager"> The type of the database manager. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <returns> The built database manager instance. </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        /// <exception cref="InvalidOperationException"> This builder has already been used to build the database manager. </exception>
        /// <exception cref="BuilderException"> Configuration or registration of objects/services failed. </exception>
        public static TManager BuildDbManager <TBuilder, TConnection, TTransaction, TManager> (this TBuilder builder)
            where TBuilder : IDbManagerBuilder<TConnection, TTransaction, TManager>
            where TConnection : DbConnection
            where TTransaction : DbTransaction
            where TManager : class, IDbManager<TConnection, TTransaction>
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            builder.ThrowIfAlreadyBuilt();

            (Type connection, Type transaction, Type manager, Type _, Type _, Type _, Type _, Type _) = builder.DetectDbManagerTypes();

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
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="locator"></param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="locator"/> is null.</exception>
        /// <exception cref="InvalidOperationException"> This builder has already been used to build the database manager. </exception>
        public static TBuilder UseBatchLocator <TBuilder> (this TBuilder builder, IDbBatchLocator locator)
            where TBuilder : IDbManagerBuilder
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

            builder.AddTemporary(typeof(TemporaryBatchLocatorRegistration), new TemporaryBatchLocatorRegistration(locator));

            return builder;
        }

        /// <summary>
        /// Uses scripts from assembly resources as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="nameFormat"> The optional name format used to search for script assembly resources or null to use the default value of <see cref="AssemblyScriptBatchLocator.NameFormat"/>. The default value of this parameter is null.</param>
        /// <param name="encoding">The optional encoding used to read the assembly resources or null to use the default value of <see cref="AssemblyScriptBatchLocator.Encoding"/>. The default value of this parameter is null.</param>
        /// <param name="commandSeparator">The optional command separator used to split scripts into commands or null to use the default value of <see cref="DbBatchLocatorBase.CommandSeparator"/>. The default value of this parameter is null.</param>
        /// <param name="optionsFormat">The optional options format used to extract options from scripts or null to use the default value of <see cref="DbBatchLocatorBase.OptionsFormat"/>. The default value of this parameter is null.</param>
        /// <param name="assemblies"> The used assemblies to locate script assembly resources or null or an empty array if the calling assembly should be used.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// <see cref="UseAssemblyScriptBatches{TBuilder}"/> creates an <see cref="AssemblyScriptBatchLocator"/>. See <see cref="AssemblyScriptBatchLocator"/> for more details.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public static TBuilder UseAssemblyScriptBatches<TBuilder>(this TBuilder builder, string nameFormat = null, Encoding encoding = null, string commandSeparator = null, string optionsFormat = null, params Assembly[] assemblies)
            where TBuilder : IDbManagerBuilder
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

            AssemblyScriptBatchLocator locator = new AssemblyScriptBatchLocator(assemblies);

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

            builder.UseBatchLocator(locator);

            return builder;
        }

        /// <summary>
        /// Uses callback implementations from assemblies as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="assemblies"> The used assemblies to locate callback implementations or null or an empty array if the calling assembly should be used.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <remarks>
        /// <note type="important">
        /// <see cref="UseAssemblyCallbackBatches{TBuilder}"/> creates an <see cref="AssemblyCallbackBatchLocator"/>. See <see cref="AssemblyCallbackBatchLocator"/> for more details.
        /// </note>
        /// </remarks>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> is null.</exception>
        public static TBuilder UseAssemblyCallbackBatches<TBuilder>(this TBuilder builder, params Assembly[] assemblies)
            where TBuilder : IDbManagerBuilder
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

            AssemblyCallbackBatchLocator locator = new AssemblyCallbackBatchLocator(assemblies);
            builder.UseBatchLocator(locator);

            return builder;
        }

        /// <summary>
        /// Uses programmatically defined scripts and/or callbacks as batches.
        /// </summary>
        /// <typeparam name="TBuilder"> The type of the used database manager builder. </typeparam>
        /// <param name="builder"> The used database manager builder. </param>
        /// <param name="dictionary"> The callback used to configure and populate the dictionary (<see cref="DictionaryBatchLocator"/>) with scripts/callbacks.</param>
        /// <returns>
        /// The used database manager builder.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="builder"/> or <paramref name="dictionary"/> is null.</exception>
        public static TBuilder UseBatches<TBuilder>(this TBuilder builder, Action<DictionaryBatchLocator> dictionary)
            where TBuilder : IDbManagerBuilder
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (dictionary == null)
            {
                throw new ArgumentNullException(nameof(dictionary));
            }
            
            DictionaryBatchLocator locator = new DictionaryBatchLocator();
            dictionary(locator);
            builder.UseBatchLocator(locator);

            return builder;
        }

        internal static (Type Connection, Type Transaction, Type Manager, Type VersionDetector, Type BackupCreator, Type CleanupProcessor, Type VersionUpgrader, Type ScriptLocator) DetectDbManagerTypes (this IDbManagerBuilder builder)
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
                    if (registration.Contract.GetGenericTypeDefinition() == typeof(IDbManager<,>))
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

            Type manager = typeof(IDbManager<,>).MakeGenericType(genericArguments);
            Type versionDetector = typeof(IDbVersionDetector<,>).MakeGenericType(genericArguments);

            Type backupCreator = typeof(IDbBackupCreator<,>).MakeGenericType(genericArguments);
            Type cleanupProcessor = typeof(IDbCleanupProcessor<,>).MakeGenericType(genericArguments);
            Type versionUpgrader = typeof(IDbVersionUpgrader<,>).MakeGenericType(genericArguments);

            Type scriptLocator = typeof(IDbScriptLocator);

            return (connection, transaction, manager, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, scriptLocator);
        }

        internal static void MergeBatchLocators (this IDbManagerBuilder builder)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            IList<CompositionRegistration> locators = builder.GetContracts(typeof(TemporaryBatchLocatorRegistration));
            AggregateBatchLocator mergedLocator = new AggregateBatchLocator(locators.Select(x => ((TemporaryBatchLocatorRegistration)x.GetOrCreateInstance()).Instance));

            builder.RemoveContracts(typeof(TemporaryBatchLocatorRegistration));
            builder.AddSingleton(typeof(IDbBatchLocator), mergedLocator);
        }

        private sealed class TemporaryBatchLocatorRegistration
        {
            public TemporaryBatchLocatorRegistration (IDbBatchLocator instance)
            {
                if (instance == null)
                {
                    throw new ArgumentNullException(nameof(instance));
                }

                this.Instance = instance;
            }

            public IDbBatchLocator Instance { get; }
        }

        #endregion
    }
}
