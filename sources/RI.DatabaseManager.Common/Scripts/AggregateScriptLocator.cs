using System;
using System.Collections;
using System.Collections.Generic;

using RI.DatabaseManager.Manager;




namespace RI.DatabaseManager.Scripts
{
	/// <summary>
	///     Implements a database script locator which combines multiple script locators.
	/// </summary>
	/// <remarks>
	///     <para>
	///         See <see cref="IDatabaseScriptLocator" /> for more details.
	///     </para>
	///     <note type="note">
	///         <see cref="IDatabaseScriptLocator.BatchSeparator" /> is ignored as the values of the individual script locators are used.
	///     </note>
	/// </remarks>
	/// <threadsafety static="true" instance="true" />
	public sealed class AggregateScriptLocator : IDatabaseScriptLocator, ICollection<IDatabaseScriptLocator>, ICollection
	{
		#region Instance Constructor/Destructor

		/// <summary>
		///     Creates a new instance of <see cref="AggregateScriptLocator" />.
		/// </summary>
		public AggregateScriptLocator ()
			: this((IEnumerable<IDatabaseScriptLocator>)null)
		{
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateScriptLocator" />.
		/// </summary>
		/// <param name="scriptLocators"> The sequence of script locators which are aggregated. </param>
		/// <remarks>
		///     <para>
		///         <paramref name="scriptLocators" /> is enumerated only once.
		///     </para>
		/// </remarks>
		public AggregateScriptLocator (IEnumerable<IDatabaseScriptLocator> scriptLocators)
		{
			this.SyncRoot = new object();

			this.ScriptLocators = new HashSet<IDatabaseScriptLocator>();

			if (scriptLocators != null)
			{
				foreach (IDatabaseScriptLocator scriptLocator in scriptLocators)
				{
					this.Add(scriptLocator);
				}
			}
		}

		/// <summary>
		///     Creates a new instance of <see cref="AggregateScriptLocator" />.
		/// </summary>
		/// <param name="scriptLocators"> The array of script locators which are aggregated. </param>
		public AggregateScriptLocator (params IDatabaseScriptLocator[] scriptLocators)
			: this((IEnumerable<IDatabaseScriptLocator>)scriptLocators)
		{
		}

		#endregion




		#region Instance Properties/Indexer

		private HashSet<IDatabaseScriptLocator> ScriptLocators { get; }

		#endregion




		#region Interface: ICollection

		/// <inheritdoc />
		bool ICollection.IsSynchronized => ((ISynchronizable)this).IsSynchronized;

		/// <inheritdoc />
		void ICollection.CopyTo (Array array, int index)
		{
			lock (this.SyncRoot)
			{
				int i1 = 0;
				foreach (IDatabaseScriptLocator item in this)
				{
					array.SetValue(item, index + i1);
					i1++;
				}
			}
		}

		#endregion




		#region Interface: ICollection<IDatabaseScriptLocator>

		/// <inheritdoc />
		public int Count
		{
			get
			{
				lock (this.SyncRoot)
				{
					return this.ScriptLocators.Count;
				}
			}
		}

		/// <inheritdoc />
		bool ICollection<IDatabaseScriptLocator>.IsReadOnly => false;

		/// <inheritdoc />
		public void Add (IDatabaseScriptLocator item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			lock (this.SyncRoot)
			{
				this.ScriptLocators.Add(item);
			}
		}

		/// <inheritdoc />
		public void Clear ()
		{
			lock (this.SyncRoot)
			{
				this.ScriptLocators.Clear();
			}
		}

		/// <inheritdoc />
		public bool Contains (IDatabaseScriptLocator item)
		{
			lock (this.SyncRoot)
			{
				return this.ScriptLocators.Contains(item);
			}
		}

		/// <inheritdoc />
		void ICollection<IDatabaseScriptLocator>.CopyTo (IDatabaseScriptLocator[] array, int arrayIndex)
		{
			lock (this.SyncRoot)
			{
				this.ScriptLocators.CopyTo(array, arrayIndex);
			}
		}

		/// <inheritdoc />
		IEnumerator IEnumerable.GetEnumerator ()
		{
			return this.GetEnumerator();
		}

		/// <inheritdoc />
		public IEnumerator<IDatabaseScriptLocator> GetEnumerator ()
		{
			lock (this.SyncRoot)
			{
				return this.ScriptLocators.GetEnumerator();
			}
		}

		/// <inheritdoc />
		public bool Remove (IDatabaseScriptLocator item)
		{
			if (item == null)
			{
				throw new ArgumentNullException(nameof(item));
			}

			lock (this.SyncRoot)
			{
				return this.ScriptLocators.Remove(item);
			}
		}

		#endregion




		#region Interface: IDatabaseScriptLocator

		/// <inheritdoc />
		string IDatabaseScriptLocator.BatchSeparator { get; set; }

		/// <inheritdoc />
		public List<string> GetScriptBatch (IDbManager manager, string name, bool preprocess)
		{
			if (manager == null)
			{
				throw new ArgumentNullException(nameof(manager));
			}

			if (name == null)
			{
				throw new ArgumentNullException(nameof(name));
			}

			if (name.IsNullOrEmptyOrWhitespace())
			{
				throw new EmptyStringArgumentException(nameof(name));
			}

			lock (this.SyncRoot)
			{
				List<string> batches = new List<string>();

				foreach (IDatabaseScriptLocator scriptLocator in this.ScriptLocators)
				{
					List<string> currentBatches = scriptLocator.GetScriptBatch(manager, name, preprocess);
					if (currentBatches == null)
					{
						return null;
					}

					batches.AddRange(currentBatches);
				}

				return batches;
			}
		}

		#endregion
	}
}
