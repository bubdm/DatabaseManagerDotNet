using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;




namespace RI.DatabaseManager.Upgrading
{
    /// <summary>
    ///     Provides an utility to extract all version upgrade steps from an assembly.
    /// </summary>
    /// <typeparam name="TProcessingStep"> The type of processing steps this utility creates. </typeparam>
    /// <typeparam name="TConnection"> The database connection type, subclass of <see cref="DbConnection" />. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type, subclass of <see cref="DbTransaction" />. </typeparam>
    /// <typeparam name="TConnectionStringBuilder"> The connection string builder type, subclass of <see cref="DbConnectionStringBuilder" />. </typeparam>
    /// <typeparam name="TManager"> The type of the database manager. </typeparam>
    /// <typeparam name="TConfiguration"> The type of database configuration. </typeparam>
    /// <remarks>
    ///     <para>
    ///         It is assumed that:
    ///         Each version upgrade step has its own script file which is added to the assembly as an embedded resource and all script files share a common name format (e.g. <c> upgrade01 </c>)
    ///         -and/or-
    ///         Each version upgrade step has its own implementation of <see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> which is marked with <see cref="AssemblyResourceVersionUpgraderStepAttribute" /> in the assembly.
    ///     </para>
    ///     <para>
    ///         Script files and <see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> implementations can be combined, meaning that a single upgrade step can have both at the same time.
    ///     </para>
    ///     <note type="note">
    ///         Implementations of <see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> must have a parameterless constructor in order to be usable.
    ///         A new instance is created for each call to <see cref="GetUpgradeStepsFromAssembly(Assembly,string,out List{TProcessingStep},out AssemblyScriptBatchLocator)" /> or <see cref="GetUpgradeStepsFromAssembly(Assembly,Encoding,string,out List{TProcessingStep},out AssemblyScriptBatchLocator)" />.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class AssemblyResourceVersionUpgraderUtility <TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TProcessingStep : IDatabaseProcessingStep<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TConnectionStringBuilder : DbConnectionStringBuilder
        where TManager : class, IDatabaseManager<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>
        where TConfiguration : class, IDatabaseManagerConfiguration<TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>, new()
    {
        #region Instance Methods

        /// <summary>
        ///     Extracts all version upgrade steps from an assembly.
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
        /// <param name="nameFormat"> The common name format of the embedded script files. </param>
        /// <param name="steps"> Returns the list of upgrade steps. </param>
        /// <param name="scriptLocator"> Returns the script locator for the assembly. </param>
        /// <returns>
        ///     true if at least one version upgrade step was extracted, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The default encoding <see cref="AssemblyScriptBatchLocator.DefaultEncoding" /> is used.
        ///     </para>
        ///     <para>
        ///         <paramref name="nameFormat" /> must be a format string, as used by <see cref="string.Format(IFormatProvider,string,object[])" />.
        ///         This method provides one argument to the format string, <c> {0} </c>, which is an incrementing number, starting at zero, which indicates the source version of a version upgrade script.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="assembly" /> or <paramref name="nameFormat" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="nameFormat" /> is an empty string. </exception>
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod"),]
        public bool GetUpgradeStepsFromAssembly (Assembly assembly, string nameFormat, out List<TProcessingStep> steps, out AssemblyScriptBatchLocator scriptLocator)
        {
            return this.GetUpgradeStepsFromAssembly(assembly, null, nameFormat, out steps, out scriptLocator);
        }

        /// <summary>
        ///     Extracts all version upgrade steps from an assembly.
        /// </summary>
        /// <param name="assembly"> The assembly. </param>
        /// <param name="encoding"> The used encoding or null to use the default encoding <see cref="AssemblyScriptBatchLocator.DefaultEncoding" />. </param>
        /// <param name="nameFormat"> The common name format of the embedded script files. </param>
        /// <param name="steps"> Returns the list of upgrade steps. The returned list is empty if no configuration steps could be found. </param>
        /// <param name="scriptLocator"> Returns the script locator for the assembly or null if no script locator for the assembly is required. </param>
        /// <returns>
        ///     true if at least one version upgrade step was extracted, false otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="nameFormat" /> must be a format string, as used by <see cref="string.Format(IFormatProvider,string,object[])" />.
        ///         This method provides one argument to the format string, <c> {0} </c>, which is an incrementing number, starting at zero, which indicates the source version of a version upgrade script.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="assembly" /> or <paramref name="nameFormat" /> is null. </exception>
        /// <exception cref="EmptyStringArgumentException"> <paramref name="nameFormat" /> is an empty string. </exception>
        [SuppressMessage("ReSharper", "InvokeAsExtensionMethod"),]
        public bool GetUpgradeStepsFromAssembly (Assembly assembly, Encoding encoding, string nameFormat, out List<TProcessingStep> steps, out AssemblyScriptBatchLocator scriptLocator)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            if (nameFormat == null)
            {
                throw new ArgumentNullException(nameof(nameFormat));
            }

            if (StringExtensions.IsNullOrEmptyOrWhitespace(nameFormat))
            {
                throw new EmptyStringArgumentException(nameof(nameFormat));
            }

            Dictionary<int, IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>> configurators = new Dictionary<int, IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>>();
            Type[] types = assembly.GetTypes();

            foreach (Type type in types)
            {
                if (!type.IsClass || type.IsAbstract)
                {
                    continue;
                }

                if (!typeof(IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>).IsAssignableFrom(type))
                {
                    continue;
                }

                AssemblyResourceVersionUpgraderStepAttribute attribute = CustomAttributeExtensions.GetCustomAttribute<AssemblyResourceVersionUpgraderStepAttribute>(type);

                if (attribute == null)
                {
                    continue;
                }

                if (configurators.ContainsKey(attribute.SourceVersion))
                {
                    throw new Exception("Found same source version twice: " + attribute.SourceVersion);
                }

                IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> instance = (IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration>)Activator.CreateInstance(type);
                configurators.Add(attribute.SourceVersion, instance);
            }

            string[] names = assembly.GetManifestResourceNames();

            steps = new List<TProcessingStep>();

            for (int i1 = 0; i1 < (names.Length + configurators.Count + 1); i1++)
            {
                string script = string.Format(CultureInfo.InvariantCulture, nameFormat, i1);
                IAssemblyResourceVersionUpgraderStepConfigurator<TProcessingStep, TConnection, TTransaction, TConnectionStringBuilder, TManager, TConfiguration> configurator = IDictionaryExtensions.GetValueOrDefault(configurators, i1);

                bool hasScript = Enumerable.Contains(names, script, StringComparer.InvariantCultureIgnoreCase);
                bool hasConfigurator = configurator != null;

                TProcessingStep step = default;

                if (hasScript && hasConfigurator)
                {
                    step = this.CreateProcessingStep(i1, null);
                    configurator.ConfigureStep(step, i1, script);
                }
                else if (hasScript)
                {
                    step = this.CreateProcessingStep(i1, script);
                }
                else if (hasConfigurator)
                {
                    step = this.CreateProcessingStep(i1, null);
                    configurator.ConfigureStep(step, i1, null);
                }

                if (step == null)
                {
                    break;
                }

                steps.Add(step);
            }

            if (steps.Count == 0)
            {
                scriptLocator = null;
                return false;
            }

            scriptLocator = new AssemblyScriptBatchLocator(assembly, encoding);
            return true;
        }

        #endregion




        #region Abstracts

        /// <summary>
        ///     Creates the actual processing step.
        /// </summary>
        /// <param name="sourceVersion"> The source version. </param>
        /// <param name="resourceName"> The corresponding embedded script file name or null if no script file is associated with this processing step. </param>
        /// <returns>
        ///     The created processing step.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         <paramref name="resourceName" /> is also null when there is a script and(!) a configurator (<see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" />), where the script is not associated when creating the step but when using the configurator - by the configurator.
        ///     </para>
        /// </remarks>
        protected abstract TProcessingStep CreateProcessingStep (int sourceVersion, string resourceName);

        #endregion
    }
}
