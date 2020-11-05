using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;




namespace RI.DatabaseManager.Batches
{
    /// <summary>
    ///     Default implementation of <see cref="IDbBatchCommandParameterCollection" /> and <see cref="IDbBatchCommandParameterCollection{TParameterTypes}"/> suitable for most scenarios.
    /// </summary>
    /// <typeparam name="TParameterTypes"> The database command parameter type. </typeparam>
    /// <threadsafety static="false" instance="false" />
    public sealed class DbBatchCommandParameterCollection<TParameterTypes> : KeyedCollection<string, IDbBatchCommandParameter<TParameterTypes>>, IDbBatchCommandParameterCollection<TParameterTypes>
        where TParameterTypes : Enum
    {
        private HashSet<IDbBatchCommandParameter> InternalSet { get; } = new HashSet<IDbBatchCommandParameter>();

        /// <inheritdoc />
        protected override string GetKeyForItem (IDbBatchCommandParameter<TParameterTypes> item) => item.Name;

        /// <inheritdoc />
        void ICollection<IDbBatchCommandParameter>.Add (IDbBatchCommandParameter item)
        {
            this.InternalSet.Add(item);
        }

        /// <inheritdoc />
        public void ExceptWith (IEnumerable<IDbBatchCommandParameter> other)
        {
            this.InternalSet.ExceptWith(other);
        }

        /// <inheritdoc />
        public void IntersectWith (IEnumerable<IDbBatchCommandParameter> other)
        {
            this.InternalSet.IntersectWith(other);
        }

        /// <inheritdoc />
        public bool IsProperSubsetOf (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.IsProperSubsetOf(other);
        }

        /// <inheritdoc />
        public bool IsProperSupersetOf (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.IsProperSupersetOf(other);
        }

        /// <inheritdoc />
        public bool IsSubsetOf (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.IsSubsetOf(other);
        }

        /// <inheritdoc />
        public bool IsSupersetOf (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.IsSupersetOf(other);
        }

        /// <inheritdoc />
        public bool Overlaps (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.Overlaps(other);
        }

        /// <inheritdoc />
        public bool SetEquals (IEnumerable<IDbBatchCommandParameter> other)
        {
            return this.InternalSet.SetEquals(other);
        }

        /// <inheritdoc />
        public void SymmetricExceptWith (IEnumerable<IDbBatchCommandParameter> other)
        {
            this.InternalSet.SymmetricExceptWith(other);
        }

        /// <inheritdoc />
        public void UnionWith (IEnumerable<IDbBatchCommandParameter> other)
        {
            this.InternalSet.UnionWith(other);
        }

        /// <inheritdoc />
        bool ISet<IDbBatchCommandParameter>.Add (IDbBatchCommandParameter item)
        {
            return this.InternalSet.Add(item);
        }

        /// <inheritdoc />
        public bool Contains (IDbBatchCommandParameter item)
        {
            return this.InternalSet.Contains(item);
        }

        /// <inheritdoc />
        public void CopyTo (IDbBatchCommandParameter[] array, int arrayIndex)
        {
            this.InternalSet.CopyTo(array, arrayIndex);
        }

        /// <inheritdoc />
        public bool Remove (IDbBatchCommandParameter item)
        {
            return this.InternalSet.Remove(item);
        }

        /// <inheritdoc />
        public new bool Add(IDbBatchCommandParameter<TParameterTypes> item)
        {
            return this.InternalSet.Add(item);
        }

        /// <inheritdoc />
        public void ExceptWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            this.InternalSet.ExceptWith(other);
        }

        /// <inheritdoc />
        public void IntersectWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            this.InternalSet.IntersectWith(other);
        }

        /// <inheritdoc />
        public bool IsProperSubsetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.IsProperSubsetOf(other);
        }

        /// <inheritdoc />
        public bool IsProperSupersetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.IsProperSupersetOf(other);
        }

        /// <inheritdoc />
        public bool IsSubsetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.IsSubsetOf(other);
        }

        /// <inheritdoc />
        public bool IsSupersetOf (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.IsSupersetOf(other);
        }

        /// <inheritdoc />
        public bool Overlaps (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.Overlaps(other);
        }

        /// <inheritdoc />
        public bool SetEquals (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            return this.InternalSet.SetEquals(other);
        }

        /// <inheritdoc />
        public void SymmetricExceptWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            this.InternalSet.SymmetricExceptWith(other);
        }

        /// <inheritdoc />
        public void UnionWith (IEnumerable<IDbBatchCommandParameter<TParameterTypes>> other)
        {
            this.InternalSet.UnionWith(other);
        }

        /// <inheritdoc />
        public new IEnumerator<IDbBatchCommandParameter> GetEnumerator ()
        {
            return this.InternalSet.GetEnumerator();
        }

        /// <inheritdoc />
        public bool IsReadOnly => ((ICollection<IDbBatchCommandParameter>)this.InternalSet).IsReadOnly;
    }
}
