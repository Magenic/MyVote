using Csla;
using MyVote.BusinessObjects.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class ObjectFactoryMock<T> : IObjectFactory<T>
	{
		public void BeginCreate(object criteria, object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginCreate(object criteria)
		{
			throw new NotImplementedException();
		}

		public void BeginCreate()
		{
			throw new NotImplementedException();
		}

		public void BeginDelete(object criteria, object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginDelete(object criteria)
		{
			throw new NotImplementedException();
		}

		public void BeginExecute(T command, object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginExecute(T command)
		{
			throw new NotImplementedException();
		}

		public void BeginFetch(object criteria, object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginFetch(object criteria)
		{
			throw new NotImplementedException();
		}

		public void BeginFetch()
		{
			throw new NotImplementedException();
		}

		public void BeginUpdate(T obj, object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginUpdate(T obj)
		{
			throw new NotImplementedException();
		}

		public Func<object, T> CreateAsyncWithCriteriaDelegate { get; set; }
		public Task<T> CreateAsync(object criteria)
		{
			if (CreateAsyncWithCriteriaDelegate != null)
			{
				return Task.FromResult<T>(CreateAsyncWithCriteriaDelegate(criteria));
			}
			else
			{
				return Task.FromResult<T>(default(T));
			}
		}

		public Func<T> CreateAsyncDelegate { get; set; }
		public Task<T> CreateAsync()
		{
			if (CreateAsyncDelegate != null)
			{
				return Task.FromResult<T>(CreateAsyncDelegate());
			}
			else
			{
				return Task.FromResult<T>(default(T));
			}
		}

		public event EventHandler<DataPortalResult<T>> CreateCompleted;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseCreateCompleted(DataPortalResult<T> result)
		{
			if (CreateCompleted != null)
			{
				CreateCompleted(this, result);
			}
		}

		public Task DeleteAsync(object criteria)
		{
			throw new NotImplementedException();
		}

		public event EventHandler<DataPortalResult<T>> DeleteCompleted;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseDeleteCompleted(DataPortalResult<T> result)
		{
			if (DeleteCompleted != null)
			{
				DeleteCompleted(this, result);
			}
		}

		public Func<T, T> ExecuteAsyncDelegate { get; set; }
		public Task<T> ExecuteAsync(T command)
		{
			if (ExecuteAsyncDelegate != null)
			{
				return Task.FromResult<T>(ExecuteAsyncDelegate(command));
			}
			else
			{
				return Task.FromResult<T>(command);
			}
		}

		public event EventHandler<DataPortalResult<T>> ExecuteCompleted;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseExecuteCompleted(DataPortalResult<T> result)
		{
			if (ExecuteCompleted != null)
			{
				ExecuteCompleted(this, result);
			}
		}

		public Func<object, T> FetchAsyncWithCriteriaDelegate { get; set; }
		public Task<T> FetchAsync(object criteria)
		{
			if (FetchAsyncWithCriteriaDelegate != null)
			{
				return Task.FromResult<T>(FetchAsyncWithCriteriaDelegate(criteria));
			}
			else
			{
				return Task.FromResult<T>(default(T));
			}
		}

		public Func<T> FetchAsyncDelegate { get; set; }
		public Task<T> FetchAsync()
		{
			if (FetchAsyncDelegate != null)
			{
				return Task.FromResult<T>(FetchAsyncDelegate());
			}
			else
			{
				return Task.FromResult<T>(default(T));
			}
		}

		public event EventHandler<DataPortalResult<T>> FetchCompleted;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseFetchCompleted(DataPortalResult<T> result)
		{
			if (FetchCompleted != null)
			{
				FetchCompleted(this, result);
			}
		}

		public Csla.Core.ContextDictionary GlobalContext
		{
			get { return null; }
		}

		public Task<T> UpdateAsync(T obj)
		{
			throw new NotImplementedException();
		}

		public event EventHandler<DataPortalResult<T>> UpdateCompleted;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseUpdatedCompleted(DataPortalResult<T> result)
		{
			if (UpdateCompleted != null)
			{
				UpdateCompleted(this, result);
			}
		}

		public Func<T> CreateChildDelegate { get; set; }
		public T CreateChild()
		{
			return this.CreateChildDelegate();
		}

		public T CreateChild(params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public T FetchChild()
		{
			throw new NotImplementedException();
		}

		public T FetchChild(params object[] parameters)
		{
			throw new NotImplementedException();
		}

		public Func<T> CreateChildAsyncDelegate { get; set; }
		public Task<T> CreateChildAsync()
		{
			if (CreateChildAsyncDelegate != null)
			{
				return Task.FromResult<T>(CreateChildAsyncDelegate());
			}
			else
			{
				return Task.FromResult<T>(default(T));
			}
		}
	}
}
