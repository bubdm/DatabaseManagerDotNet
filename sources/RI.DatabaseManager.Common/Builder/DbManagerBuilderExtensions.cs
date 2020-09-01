using System;
using System.Collections.Generic;
using System.Reflection;

using RI.Abstractions.Builder;
using RI.Abstractions.Logging;
using RI.DatabaseManager.Scripts;




namespace RI.DatabaseManager.Builder
{
    /// <summary>
    ///     Provides utility/extension methods for the <see cref="DbManagerBuilder" /> type.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public static class DbManagerBuilderExtensions
    {
        /// <summary>
        ///     Adds a database script locator which searches assemblies for scripts.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="assemblies"> The array of searched assemblies or null or an empty array if only the calling assembly shall be searched. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> is null. </exception>
        public static DbManagerBuilder UseScriptsFromAssembly (this DbManagerBuilder builder, params Assembly[] assemblies)
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
                    Assembly.GetCallingAssembly()
                };
            }

            builder.AddSingleton(typeof(IDbScriptLocator), sp => new AssemblyRessourceDbScriptLocator((ILogger)sp.GetService(typeof(ILogger)), assemblies));

            return builder;
        }

        /// <summary>
        ///     Adds a database script locator which stores the scripts in a dictionary.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="scripts"> The dictionary with key/value pairs or name/script pairs respectively. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="scripts"/> is null. </exception>
        public static DbManagerBuilder UseScriptsFromDictionary (this DbManagerBuilder builder, IDictionary<string, string> scripts)
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

        /// <summary>
        ///     Adds multiple database script locators.
        /// </summary>
        /// <param name="builder"> The builder being configured. </param>
        /// <param name="scriptLocators"> The array of database script locators. </param>
        /// <returns> The builder being configured. </returns>
        /// <exception cref="ArgumentNullException"> <paramref name="builder" /> or <paramref name="scriptLocators"/> is null. </exception>
        public static DbManagerBuilder UseScripts (this DbManagerBuilder builder, params IDbScriptLocator[] scriptLocators)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            scriptLocators ??= new IDbScriptLocator[0];

            builder.AddSingleton(typeof(IDbScriptLocator), _ => new AggregateDbScriptLocator(scriptLocators));

            return builder;
        }
    }
}
