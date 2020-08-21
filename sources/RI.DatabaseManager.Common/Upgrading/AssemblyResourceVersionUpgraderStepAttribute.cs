using System;




namespace RI.Framework.Data.Database.Upgrading
{
    /// <summary>
    ///     The attribute which defines the source version associated with a <see cref="IAssemblyResourceVersionUpgraderStepConfigurator{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> implementation.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="AssemblyResourceVersionUpgraderStepAttribute" /> is used with <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" />.
    ///         See <see cref="AssemblyResourceVersionUpgraderUtility{TProcessingStep,TConnection,TTransaction,TConnectionStringBuilder,TManager,TConfiguration}" /> for more details.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public sealed class AssemblyResourceVersionUpgraderStepAttribute : Attribute
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyResourceVersionUpgraderStepAttribute" />.
        /// </summary>
        /// <param name="sourceVersion"> The source version. </param>
        /// <exception cref="ArgumentOutOfRangeException"> <paramref name="sourceVersion" /> is less than zero. </exception>
        public AssemblyResourceVersionUpgraderStepAttribute (int sourceVersion)
        {
            if (sourceVersion < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sourceVersion));
            }

            this.SourceVersion = sourceVersion;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the source version.
        /// </summary>
        /// <value>
        ///     The source version.
        /// </value>
        public int SourceVersion { get; }

        #endregion
    }
}
