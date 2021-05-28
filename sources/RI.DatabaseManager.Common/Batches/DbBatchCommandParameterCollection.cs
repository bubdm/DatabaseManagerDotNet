using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatchCommandParameterCollection" /> and <see cref="IDbBatchCommandParameterCollection{TParameterTypes}"/> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatchCommandParameterCollection<TParameterTypes> : IDbBatchCommandParameterCollection<TParameterTypes>
        where TParameterTypes : Enum
    {
        private HashSet<IDbBatchCommandParameter<TParameterTypes>> InternalSet { get; } = new HashSet<IDbBatchCommandParameter<TParameterTypes>>();

        /// <inheritdoc />
        public bool Add(IDbBatchCommandParameter<TParameterTypes> item) => this.InternalSet.Add(item);

        /// <inheritdoc />
        public bool Contains (IDbBatchCommandParameter<TParameterTypes> item) => this.InternalSet.Contains(item);

        /// <inheritdoc />
        public void CopyTo (IDbBatchCommandParameter<TParameterTypes>[] array, int arrayIndex) => this.InternalSet.CopyTo(array, arrayIndex);

        /// <inheritdoc />
        public bool Remove (IDbBatchCommandParameter<TParameterTypes> item) => this.InternalSet.Remove(item);

        /// <inheritdoc />
        public int Count => this.InternalSet.Count;

        /// <inheritdoc />
        public void ExceptWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.ExceptWith(other);

        /// <inheritdoc />
        public void IntersectWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.IntersectWith(other);

        /// <inheritdoc />
        public bool IsProperSubsetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.IsProperSubsetOf(other);

        /// <inheritdoc />
        public bool IsProperSupersetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.IsProperSupersetOf(other);

        /// <inheritdoc />
        public bool IsSubsetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.IsSubsetOf(other);

        /// <inheritdoc />
        public bool IsSupersetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.IsSupersetOf(other);

        /// <inheritdoc />
        public bool Overlaps (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.Overlaps(other);

        /// <inheritdoc />
        public bool SetEquals (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.SetEquals(other);

        /// <inheritdoc />
        public void SymmetricExceptWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.SymmetricExceptWith(other);

        /// <inheritdoc />
        public void UnionWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other) => this.InternalSet.UnionWith(other);

        /// <inheritdoc />
        public bool IsReadOnly => ((ICollection<IDbBatchCommandParameter<TParameterTypes>>)this.InternalSet).IsReadOnly;

        /// <inheritdoc />
        public void AddRange (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> parameters)
        {
            if (parameters == null)
            {
                return;
            }

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in parameters)
            {
                this.InternalSet.Add(parameter);
            }
        }

        /// <inheritdoc />
        public IDbBatchCommandParameter<TParameterTypes> Add (string name, TParameterTypes type, object value = null)
        {
            this.Remove(name);

            DbBatchCommandParameter<TParameterTypes> parameter = new DbBatchCommandParameter<TParameterTypes>(name, type, value);

            this.InternalSet.Add(parameter);

            return parameter;
        }

        /// <inheritdoc cref="ICloneable.Clone"/>
        public DbBatchCommandParameterCollection<TParameterTypes> Clone ()
        {
            DbBatchCommandParameterCollection<TParameterTypes> clone = new DbBatchCommandParameterCollection<TParameterTypes>();

            foreach (IDbBatchCommandParameter<TParameterTypes> parameter in this.InternalSet)
            {
                clone.Add(parameter);
            }

            return clone;
        }

        /// <inheritdoc />
        object ICloneable.Clone() => this.Clone();

        /// <inheritdoc />
        public bool Contains (string name) => this.InternalSet.Any(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

        /// <inheritdoc />
        public bool Remove (string name)
        {
            IDbBatchCommandParameter<TParameterTypes> parameter = this.InternalSet.FirstOrDefault(x => x.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase));

            if (parameter == null)
            {
                return false;
            }

            return this.InternalSet.Remove(parameter);
        }

        /// <inheritdoc />
        public IEnumerable<IDbBatchCommandParameter> GetAll () =>
            this.InternalSet.Cast<IDbBatchCommandParameter>()
                .ToList();

        /// <inheritdoc />
        void IDbBatchCommandParameterCollection.ClearParameters () => this.Clear();

        /// <inheritdoc />
        void ICollection<IDbBatchCommandParameter<TParameterTypes>>.Add (IDbBatchCommandParameter<TParameterTypes> item) => this.InternalSet.Add(item);

        /// <inheritdoc />
        public void Clear () => this.InternalSet.Clear();

        /// <inheritdoc />
        public IEnumerator<IDbBatchCommandParameter<TParameterTypes>> GetEnumerator () => this.InternalSet.GetEnumerator();

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator () => this.InternalSet.GetEnumerator();
    }
}
