using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which searches assemblies for callbacks.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         <see cref="Assemblies" /> is the list of assemblies used to lookup callbacks.
    ///     </para>
    ///     <para>
    ///         <see cref="AssemblyCallbackBatchLocator" /> searches the assemblies for non-abstract class types which implement <see cref="ICallbackBatch" /> and which have a public parameterless constructor.
    ///         Each found type is considered an independent batch. Each type such a batch is executed, a new instance of the corresponding type is instantiated (using <see cref="Activator.CreateInstance(Type,bool)" />) and <see cref="ICallbackBatch.Execute" /> is called.
    ///     </para>
    ///     <para>
    ///         By default, the name of the types which implement <see cref="ICallbackBatch" /> are used as the batch names and <see cref="DbBatchTransactionRequirement.DontCare" /> is used as transaction requirement.
    ///         This can be overriden with the optional <see cref="CallbackBatchAttribute" />, applied to the type, which overrides the name and/or transaction requirement.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class AssemblyCallbackBatchLocator : DbBatchLocatorBase
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator" />.
        /// </summary>
        public AssemblyCallbackBatchLocator ()
            : this((IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator" />.
        /// </summary>
        /// <param name="assemblies"> The sequence of assemblies. </param>
        /// <remarks>
        ///     <para>
        ///         <paramref name="assemblies" /> is enumerated only once.
        ///     </para>
        /// </remarks>
        public AssemblyCallbackBatchLocator (IEnumerable<Assembly> assemblies)
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
        /// <param name="assemblies"> The array of assemblies. </param>
        public AssemblyCallbackBatchLocator (params Assembly[] assemblies)
            : this((IEnumerable<Assembly>)assemblies) { }

        #endregion




        #region Instance Properties/Indexer

        /// <summary>
        ///     Gets the list of used assemblies to lookup callbacks.
        /// </summary>
        /// <value>
        ///     The list of used assemblies to lookup callbacks.
        /// </value>
        public List<Assembly> Assemblies { get; }

        #endregion




        #region Instance Methods

        private CallbackTypeCollection GetCallbackTypes ()
        {
            CallbackTypeCollection callbackTypes = new CallbackTypeCollection();

            foreach (Assembly assembly in this.Assemblies)
            {
                List<Type> types = assembly.GetTypes()
                                           .Where(x => x.IsClass)
                                           .Where(x => !x.IsAbstract)
                                           .Where(x => typeof(ICallbackBatch).IsAssignableFrom(x))
                                           .Where(x => x.GetConstructor(new Type[0])
                                                        ?.IsPublic ?? false)
                                           .ToList();

                foreach (Type type in types)
                {
                    string name = type.Name;
                    DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare;
                    CallbackBatchAttribute attribute = type.GetCustomAttribute<CallbackBatchAttribute>(true);

                    if (attribute != null)
                    {
                        name = (string.IsNullOrWhiteSpace(attribute.Name) ? null : attribute.Name) ?? name;
                        transactionRequirement = attribute.TransactionRequirement;
                    }

                    callbackTypes.Add(new CallbackType(type, name, transactionRequirement));
                }
            }

            return callbackTypes;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch batch, string name, string commandSeparator)
        {
            CallbackTypeCollection callbackTypes = this.GetCallbackTypes();

            if (!callbackTypes.Contains(name))
            {
                return false;
            }

            CallbackType callbackType = callbackTypes[name];

            batch.AddCode(callbackType.CreateCallback(), callbackType.TransactionRequirement);

            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetNames ()
        {
            CallbackTypeCollection callbackTypes = this.GetCallbackTypes();

            return callbackTypes.Select(x => x.Name);
        }

        /// <inheritdoc />
        public override bool SupportsScripts => false;

        /// <inheritdoc />
        public override bool SupportsCallbacks => true;

        #endregion




        #region Type: CallbackType

        private sealed class CallbackType
        {
            #region Instance Constructor/Destructor

            public CallbackType (Type type, string name, DbBatchTransactionRequirement transactionRequirement)
            {
                if (type == null)
                {
                    throw new ArgumentNullException(nameof(type));
                }

                if (name == null)
                {
                    throw new ArgumentNullException(nameof(name));
                }

                if (string.IsNullOrWhiteSpace(name))
                {
                    throw new ArgumentException("The string argument is empty.", nameof(name));
                }

                this.Type = type;
                this.Name = name;
                this.TransactionRequirement = transactionRequirement;
            }

            #endregion




            #region Instance Properties/Indexer

            public string Name { get; }

            public DbBatchTransactionRequirement TransactionRequirement { get; }

            public Type Type { get; }

            #endregion




            #region Instance Methods

            public Func<DbConnection, DbTransaction, object> CreateCallback ()
            {
                return (connection, transaction) =>
                {
                    ICallbackBatch instance = (ICallbackBatch)Activator.CreateInstance(this.Type, false);
                    return instance.Execute(connection, transaction);
                };
            }

            #endregion
        }

        #endregion




        #region Type: CallbackTypeCollection

        private sealed class CallbackTypeCollection : KeyedCollection<string, CallbackType>
        {
            #region Instance Constructor/Destructor

            public CallbackTypeCollection ()
                : base(StringComparer.InvariantCultureIgnoreCase) { }

            #endregion




            #region Overrides

            /// <inheritdoc />
            protected override string GetKeyForItem (CallbackType item)
            {
                return item.Name;
            }

            #endregion
        }

        #endregion
    }
}
