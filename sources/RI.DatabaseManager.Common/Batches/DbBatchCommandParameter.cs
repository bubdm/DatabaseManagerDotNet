using System;
using System.Data;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatchCommandParameter" /> and
    ///     <see cref="IDbBatchCommandParameter{TParameterTypes}" /> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatchCommandParameter <TParameterTypes> : IDbBatchCommandParameter<TParameterTypes>
        where TParameterTypes : Enum
    {
        #region Instance Constructor/Destructor

        /// <summary>
        ///     Creates a new instance of <see cref="DbBatchCommandParameter{TParameterTypes}" />.
        /// </summary>
        /// <param name="name"> The name of the parameter. </param>
        /// <param name="type"> The type of the parameter. </param>
        /// <param name="value"> The optional value of the parameter. The default value is null. </param>
        /// <exception cref="ArgumentNullException"> <paramref name="name" /> is null. </exception>
        /// <exception cref="ArgumentException"> <paramref name="name" /> is an empty string. </exception>
        public DbBatchCommandParameter (string name, TParameterTypes type, object value = null)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("The string argument is empty.", nameof(name));
            }

            this.Name = name;
            this.Type = type;
            this.Value = value;
        }

        #endregion




        #region Instance Methods

        /// <inheritdoc cref="ICloneable.Clone" />
        public DbBatchCommandParameter<TParameterTypes> Clone () =>
            new DbBatchCommandParameter<TParameterTypes>(this.Name, this.Type, this.Value);

        #endregion




        #region Overrides

        /// <inheritdoc />
        public override bool Equals (object obj) => this.Equals(obj as IDbBatchCommandParameter);

        /// <inheritdoc />
        public override int GetHashCode () => this.Name.GetHashCode();

        /// <inheritdoc />
        public override string ToString ()
        {
            return base.ToString();
        }

        #endregion




        #region Interface: ICloneable

        /// <inheritdoc />
        object ICloneable.Clone () => this.Clone();

        #endregion




        #region Interface: IDbBatchCommandParameter

        /// <inheritdoc />
        public string Name { get; }

        /// <inheritdoc />
        DbType IDbBatchCommandParameter.Type => (DbType)Enum.Parse(typeof(DbType), this.Type.ToString());

        /// <inheritdoc />
        public object Value { get; set; }

        #endregion




        #region Interface: IDbBatchCommandParameter<TParameterTypes>

        /// <inheritdoc />
        public TParameterTypes Type { get; }

        #endregion




        #region Interface: IEquatable<IDbBatchCommandParameter<TParameterTypes>>

        /// <inheritdoc />
        public bool Equals (IDbBatchCommandParameter<TParameterTypes> other) =>
            this.Equals(other as IDbBatchCommandParameter);

        #endregion




        #region Interface: IEquatable<IDbBatchCommandParameter>

        /// <inheritdoc />
        public bool Equals (IDbBatchCommandParameter other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Name.Equals(other.Name, StringComparison.InvariantCultureIgnoreCase);
        }

        #endregion
    }
}
