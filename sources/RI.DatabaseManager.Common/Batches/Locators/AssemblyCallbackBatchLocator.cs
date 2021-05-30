using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

using RI.DatabaseManager.Batches.Commands;




namespace RI.DatabaseManager.Batches.Locators
{
    /// <summary>
    ///     Database batch locator implementation which searches assemblies for callbacks.
    /// </summary>
    /// <typeparam name="TConnection"> The database connection type. </typeparam>
    /// <typeparam name="TTransaction"> The database transaction type. </typeparam>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <remarks>
    ///     <para>
    ///         <see cref="Assemblies" /> is the list of assemblies used to lookup callbacks.
    ///     </para>
    ///     <para>
    ///         <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}" /> searches the assemblies
    ///         for non-abstract class types which implement
    ///         <see cref="ICallbackBatch{TConnection,TTransaction,TParameterTypes}" /> and which have a public parameterless
    ///         constructor.
    ///         Each found type is considered an independent batch. Each type such a batch is executed, a new instance of the
    ///         corresponding type is instantiated (using <see cref="Activator.CreateInstance(Type,bool)" />) and
    ///         <see cref="ICallbackBatch{TConnection,TTransaction,TParameterTypes}.Execute" /> is called.
    ///     </para>
    ///     <para>
    ///         By default, the name of the types which implement
    ///         <see cref="ICallbackBatch{TConnection,TTransaction,TParameterTypes}" /> are used as the batch names and
    ///         <see cref="DbBatchTransactionRequirement.DontCare" /> is used as transaction requirement.
    ///         This can be overriden with the optional <see cref="CallbackBatchAttribute" />, applied to the type, which
    ///         overrides the name and/or transaction requirement.
    ///     </para>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public sealed class
        AssemblyCallbackBatchLocator <TConnection, TTransaction, TParameterTypes> : DbBatchLocatorBase<TConnection,
            TTransaction, TParameterTypes>
        where TConnection : DbConnection
        where TTransaction : DbTransaction
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}" />.
        /// </summary>
        public AssemblyCallbackBatchLocator ()
            : this((IEnumerable<Assembly>)null) { }

        /// <summary>
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}" />.
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
        ///     Creates a new instance of <see cref="AssemblyCallbackBatchLocator{TConnection,TTransaction,TParameterTypes}" />.
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

        private Dictionary<string, List<CallbackType>> GetCallbackTypes ()
        {
            Dictionary<string, List<CallbackType>> callbackTypes =
                new Dictionary<string, List<CallbackType>>(this.DefaultNameComparer);

            foreach (Assembly assembly in this.Assemblies)
            {
                List<Type> types = assembly.GetTypes()
                                           .Where(x => x.IsClass)
                                           .Where(x => !x.IsAbstract)
                                           .Where(x =>
                                                      typeof(ICallbackBatch<TConnection, TTransaction, TParameterTypes>)
                                                          .IsAssignableFrom(x))
                                           .Where(x => x.GetConstructor(new Type[0])
                                                        ?.IsPublic ?? false)
                                           .ToList();

                foreach (Type type in types)
                {
                    string name = type.Name;
                    DbBatchTransactionRequirement transactionRequirement = DbBatchTransactionRequirement.DontCare;
                    IsolationLevel? isolationLevel = null;
                    CallbackBatchAttribute attribute = type.GetCustomAttribute<CallbackBatchAttribute>(true);

                    if (attribute != null)
                    {
                        name = (string.IsNullOrWhiteSpace(attribute.Name) ? null : attribute.Name) ?? name;
                        transactionRequirement = attribute.TransactionRequirement;
                        isolationLevel = attribute.IsolationLevel;
                    }

                    if (!callbackTypes.ContainsKey(name))
                    {
                        callbackTypes.Add(name, new List<CallbackType>());
                    }

                    callbackTypes[name]
                        .Add(new CallbackType(type, name, transactionRequirement, isolationLevel));
                }
            }

            return callbackTypes;
        }

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool SupportsCallbacks => true;

        /// <inheritdoc />
        public override bool SupportsScripts => false;

        /// <inheritdoc />
        protected override bool FillBatch (IDbBatch<TConnection, TTransaction, TParameterTypes> batch, string name,
                                           string commandSeparator)
        {
            Dictionary<string, List<CallbackType>> callbackTypes = this.GetCallbackTypes();

            if (!callbackTypes.ContainsKey(name))
            {
                return false;
            }

            foreach (CallbackType callback in callbackTypes[name])
            {
                batch.AddCode(callback.CreateCallback(), callback.TransactionRequirement, callback.IsolationLevel);
            }

            return true;
        }

        /// <inheritdoc />
        protected override IEnumerable<string> GetNames ()
        {
            Dictionary<string, List<CallbackType>> callbackTypes = this.GetCallbackTypes();

            return callbackTypes.Keys;
        }

        #endregion




        #region Type: CallbackType

        private sealed class CallbackType
        {
            #region Instance Constructor/Destructor

            public CallbackType (Type type, string name, DbBatchTransactionRequirement transactionRequirement,
                                 IsolationLevel? isolationLevel)
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
                this.IsolationLevel = isolationLevel;
            }

            #endregion




            #region Instance Properties/Indexer

            public IsolationLevel? IsolationLevel { get; }

            public string Name { get; }

            public DbBatchTransactionRequirement TransactionRequirement { get; }

            public Type Type { get; }

            #endregion




            #region Instance Methods

            public CallbackBatchCommandDelegate<TConnection, TTransaction, TParameterTypes> CreateCallback () =>
                this.Target;

            private object Target (TConnection connection, TTransaction transaction,
                                   IDbBatchCommandParameterCollection<TParameterTypes> parameters, out string error,
                                   out Exception exception)
            {
                ICallbackBatch<TConnection, TTransaction, TParameterTypes> instance =
                    (ICallbackBatch<TConnection, TTransaction, TParameterTypes>)
                    Activator.CreateInstance(this.Type, false);

                return instance.Execute(connection, transaction, parameters, out error, out exception);
            }

            #endregion
        }

        #endregion
    }
}
