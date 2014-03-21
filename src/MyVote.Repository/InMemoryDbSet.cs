using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;

namespace MyVote.Repository
{
	/// <summary>
	/// Lifted from: http://romiller.com/2012/02/14/testing-with-a-fake-dbcontext/.
	/// Useful in testing/mocking scenarios.
	/// </summary>
	public class InMemoryDbSet<T> : IDbSet<T>
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

		public virtual T Find(params object[] keyValues)
		{
			throw new NotImplementedException("Derive from FakeDbSet<T> and override Find");
		}

		public T Add(T item)
		{
			this.data.Add(item);
			return item;
		}

		public T Remove(T item)
		{
			this.data.Remove(item);
			return item;
		}

		public T Attach(T item)
		{
			this.data.Add(item);
			return item;
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

		public ObservableCollection<T> Local
		{
			get { return this.data; }
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
	}
}
