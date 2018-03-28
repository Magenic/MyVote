using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;

namespace MyVote.Data.Entities
{
	/// <summary>
	/// Lifted from: http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext/.
	/// Useful in testing/mocking scenarios.
	/// </summary>
	public class InMemoryDbSet<T> : DbSet<T>, IQueryable<T>
		 where T : class
	{
		private ObservableCollection<T> data;
		private IQueryable query;

		public InMemoryDbSet()
			: base()
		{
			this.data = new ObservableCollection<T>();
			this.query = this.data.AsQueryable();
		}

		public override T Find(params object[] keyValues)
		{
			throw new NotImplementedException("Derive from InMemoryDbSet<T> and override Find");
		}

		public override EntityEntry<T> Add(T entity)
		{
			this.data.Add(entity);
			return default(EntityEntry<T>);
		}

		public override EntityEntry<T> Remove(T entity)
		{
			this.data.Remove(entity);
			return default(EntityEntry<T>);
		}

		public override EntityEntry<T> Attach(T entity)
		{
			this.data.Add(entity);
			return default(EntityEntry<T>);
		}

		public T Detach(T item)
		{
			this.data.Remove(item);
			return item;
		}

		public T Create()
		{
			return Activator.CreateInstance<T>();
		}

		public TDerivedEntity Create<TDerivedEntity>() where TDerivedEntity : class, T
		{
			return Activator.CreateInstance<TDerivedEntity>();
		}

		Type IQueryable.ElementType
		{
			get { return this.query.ElementType; }
		}

		Expression IQueryable.Expression
		{
			get { return this.query.Expression; }
		}

		IQueryProvider IQueryable.Provider
		{
			get { return this.query.Provider; }
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			return this.data.GetEnumerator();
		}

		public ObservableCollection<T> LocalData => this.data;

	}
}
