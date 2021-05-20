using System;
using System.Data;
using System.Data.Common;

using RI.DatabaseManager.Batches;




namespace RI.DatabaseManager.Builder.Options
{
    /// <summary>
    ///     Boilerplate implementation of <see cref="IDbManagerOptions" />, <see cref="ISupportDefaultDatabaseCleanup"/>, <see cref="ISupportDefaultDatabaseCreation"/>, <see cref="ISupportDefaultDatabaseVersioning"/>, <see cref="ISupportDefaultDatabaseUpgrading"/>.
    /// </summary>
    /// <remarks>
    ///     <note type="implement">
    ///         It is recommended that database options implementations use this base class as it already implements most of the database-independent logic.
    ///     </note>
    /// </remarks>
    /// <threadsafety static="false" instance="false" />
    public abstract class DbManagerOptionsBase<TOptions, TConnectionStringBuilder> : IDbManagerOptions, ISupportDefaultDatabaseCleanup, ISupportDefaultDatabaseCreation, ISupportDefaultDatabaseVersioning, ISupportDefaultDatabaseUpgrading
        where TOptions : DbManagerOptionsBase<TOptions, TConnectionStringBuilder>, new()
        where TConnectionStringBuilder : DbConnectionStringBuilder, new()
    {
        /// <summary>
        /// Creates a new instance of <see cref="DbManagerOptionsBase{TOptions,TConnectionStringBuilder}"/>.
        /// </summary>
        public DbManagerOptionsBase ()
        {
            this.ConnectionString = new TConnectionStringBuilder();
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbManagerOptionsBase{TOptions,TConnectionStringBuilder}"/>.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="connectionString" /> is an empty string. </exception>
        public DbManagerOptionsBase(string connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new ArgumentException("The string argument is empty.", nameof(connectionString));
            }

            this.ConnectionString = new TConnectionStringBuilder();
            this.ConnectionString.ConnectionString = connectionString;
        }

        /// <summary>
        /// Creates a new instance of <see cref="DbManagerOptionsBase{TOptions,TConnectionStringBuilder}"/>.
        /// </summary>
        /// <param name="connectionString"> The used connection string. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="connectionString" /> is null. </exception>
        public DbManagerOptionsBase(TConnectionStringBuilder connectionString)
        {
            if (connectionString == null)
            {
                throw new ArgumentNullException(nameof(connectionString));
            }

            this.ConnectionString = new TConnectionStringBuilder();
            this.ConnectionString.ConnectionString = connectionString.ConnectionString;
        }

        private TConnectionStringBuilder _connectionString;

        /// <summary>
        ///     Gets or sets the connection string builder.
        /// </summary>
        /// <value>
        ///     The connection string builder. Cannot be null.
        /// </value>
        /// <exception cref="ArgumentNullException"> <paramref name="value" /> is null. </exception>
        public virtual TConnectionStringBuilder ConnectionString
        {
            get => this._connectionString;
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                this._connectionString = value;
            }
        }

        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        /// <inheritdoc cref="ICloneable.Clone"/>
        public TOptions Clone ()
        {
            TOptions clone = new TOptions();

            clone.ConnectionString.ConnectionString = this.ConnectionString.ConnectionString;

            this.CustomClone(ref clone);

            return clone;
        }

        /// <summary>
        /// Clones the options implemented by derivations from <see cref="DbManagerOptionsBase{TOptions,TConnectionStringBuilder}"/>.
        /// </summary>
        /// <param name="clone">The clone to be filled.</param>
        /// <remarks>
        ///<para>
        /// The default implementation does nothing.
        /// </para>
        /// </remarks>
        protected void CustomClone (ref TOptions clone)
        {
        }

        /// <inheritdoc />
        public virtual string GetConnectionString () => this.ConnectionString.ConnectionString;

        /// <inheritdoc />
        public virtual string GetDefaultUpgradingBatchNameFormat () => @".+?(?&lt;sourceVersion&gt;\d{4}).*";

        /// <inheritdoc />
        public virtual string[] GetDefaultVersioningScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.DontCare;
            isolationLevel = null;
            return null;
        }

        /// <inheritdoc />
        public virtual string[] GetDefaultCreationScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.DontCare;
            isolationLevel = null;
            return null;
        }

        /// <inheritdoc />
        public virtual string[] GetDefaultCleanupScript (out DbBatchTransactionRequirement transactionRequirement, out IsolationLevel? isolationLevel)
        {
            transactionRequirement = DbBatchTransactionRequirement.DontCare;
            isolationLevel = null;
            return null;
        }
    }
}
