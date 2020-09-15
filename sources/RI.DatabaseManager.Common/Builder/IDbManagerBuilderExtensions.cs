using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Reflection;

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
        ///     Adds multiple database script locators.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="scriptLocators"> The array of database script locators. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="scriptLocators" /> is null. </exception>
        /// TODO: Remove/change
        public static IDbManagerBuilder UseScripts (this IDbManagerBuilder builder, params IDbScriptLocator[] scriptLocators)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            scriptLocators ??= new IDbScriptLocator[0];

            builder.AddSingleton(typeof(IDbScriptLocator), _ => new AggregateDbScriptLocator(scriptLocators));

            return builder;
        }

        /// <summary>
        ///     Adds a database script locator which searches assemblies for scripts.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="assemblies"> The array of searched assemblies or null or an empty array if only the calling assembly shall be searched. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        /// TODO: Remove/change
        public static IDbManagerBuilder UseScriptsFromAssembly (this IDbManagerBuilder builder, params Assembly[] assemblies)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            assemblies ??= new Assembly[0];

            if (assemblies.Length == 0)
            {
                assemblies = new[]
                {
                    Assembly.GetCallingAssembly(),
                };
            }

            builder.AddSingleton(typeof(IDbScriptLocator), sp => new AssemblyScriptBatchLocator((ILogger)sp.GetService(typeof(ILogger)), assemblies));

            return builder;
        }

        /// <summary>
        ///     Adds a database script locator which stores the scripts in a dictionary.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="scripts"> The dictionary with key/value pairs or name/script pairs respectively. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="scripts" /> is null. </exception>
        /// TODO: Remove/change
        public static IDbManagerBuilder UseScriptsFromDictionary (this IDbManagerBuilder builder, IDictionary<string, string> scripts)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            if (scripts == null)
            {
                throw new ArgumentNullException(nameof(scripts));
            }

            builder.AddSingleton(typeof(IDbScriptLocator), sp => new DictionaryDbScriptLocator((ILogger)sp.GetService(typeof(ILogger)), scripts));

            return builder;
        }

        //TODO: Add batch locator using extensions (using aggregated locator if necessary)

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
            Type versionDetector = typeof(IDatabaseVersionDetector<,>).MakeGenericType(genericArguments);

            Type backupCreator = typeof(IDatabaseBackupCreator<,>).MakeGenericType(genericArguments);
            Type cleanupProcessor = typeof(IDatabaseCleanupProcessor<,>).MakeGenericType(genericArguments);
            Type versionUpgrader = typeof(IDatabaseVersionUpgrader<,>).MakeGenericType(genericArguments);

            Type scriptLocator = typeof(IDbScriptLocator);

            return (connection, transaction, manager, versionDetector, backupCreator, cleanupProcessor, versionUpgrader, scriptLocator);
        }

        #endregion
    }
}
