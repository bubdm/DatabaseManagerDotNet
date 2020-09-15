using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

using RI.Abstractions.Logging;
using RI.DatabaseManager.Scripts;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which searches assemblies for callbacks.
    /// </summary>
    /// <para>
    ///     <see cref="Assemblies" /> is the list of assemblies used to lookup callbacks.
    /// </para>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyCallbackBatchLocator : DbBatchLocatorBase
    {
        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyCallbackBatchLocator(ILogger logger)
            : this(logger, (IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The sequence of assemblies. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="assemblies" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyCallbackBatchLocator(ILogger logger, IEnumerable<Assembly> assemblies) : base(logger)
        {
            this.Assemblies = new List<Assembly>();

            if (assemblies != null)
            {
                this.Assemblies.AddRange(assemblies);
            }
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator" />.
        /// </summary>
        /// <param name="logger"> The used logger. </param>
        /// <param name="assemblies"> The array of assemblies. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="logger" /> is null. </exception>
        public AssemblyCallbackBatchLocator(ILogger logger, params Assembly[] assemblies)
            : this(logger, (IEnumerable<Assembly>)assemblies) { }

        /// <summary>
        ///     Gets the list of used assemblies to lookup callbacks.
        /// </summary>
        /// <value>
        ///     The list of used assemblies to lookup callbacks.
        /// </value>
        public List<Assembly> Assemblies { get; }
    }
}
