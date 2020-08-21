using System;
using System.IO;
using System.Reflection;
using System.Text;




namespace RI.Framework.Data.Database.Scripts
{
    /// <summary>
    ///     Implements a database script locator which uses assembly resources to locate scripts.
    /// </summary>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyRessourceScriptLocator : DatabaseScriptLocator
    {
        #region Constants

        /// <summary>
        ///     Thedefault encoding used to read assembly resources.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The default encoding is UTF-8.
        ///     </para>
        /// </remarks>
        public static readonly Encoding DefaultEncoding = Encoding.UTF8;

        #endregion




        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyRessourceScriptLocator" />.
        /// </summary>
        /// <param name="assembly"> The used assembly. </param>
        /// <remarks>
        ///     <para>
        ///         The default encoding <see cref="DefaultEncoding" /> is used.
        ///     </para>
        /// </remarks>
        /// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
        public AssemblyRessourceScriptLocator (Assembly assembly)
            : this(assembly, null)
        {
        }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyRessourceScriptLocator" />.
        /// </summary>
        /// <param name="assembly"> The used assembly. </param>
        /// <param name="encoding"> The used encoding or null to use the default encoding <see cref="DefaultEncoding" />. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="assembly" /> is null. </exception>
        public AssemblyRessourceScriptLocator (Assembly assembly, Encoding encoding)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            this.Assembly = assembly;
            this.Encoding = encoding ?? AssemblyRessourceScriptLocator.DefaultEncoding;
        }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the used assembly.
        /// </summary>
        /// <value>
        ///     The used assembly.
        /// </value>
        public Assembly Assembly { get; }

        /// <summary>
        ///     Gets the used encoding.
        /// </summary>
        /// <value>
        ///     The used encoding.
        /// </value>
        public Encoding Encoding { get; }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override string LocateAndReadScript (IDatabaseManager manager, string name)
        {
            using (Stream stream = this.Assembly.GetManifestResourceStream(name))
            {
                if (stream == null)
                {
                    return null;
                }

                using (StreamReader sr = new StreamReader(stream, this.Encoding))
                {
                    return sr.ReadToEnd();
                }
            }
        }

        #endregion
    }
}
