using System;
using System.Threading.Tasks;
using Csla;
using Csla.Core;
using Csla.Serialization.Mobile;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Extensions;
using System.Reflection;
using Csla.Reflection;

namespace MyVote.BusinessObjects
{
#pragma warning disable 67
    [Serializable]
    internal sealed class ObjectFactory<T>
            : IObjectFactory<T>
        where T : class, IMobileObject
	{
		private readonly Type concreteType;

		public ObjectFactory()
		{
			this.concreteType = typeof(T).GetConcreteType();
		}

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

		public T Create()
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Create), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as T;
        }

		public T Create(object criteria)
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Create), new[] { typeof(object) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria }) as T;
    	}


		public async Task<T> CreateAsync(object criteria)
		{            
            var task = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.CreateAsync), new[] { typeof(object) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria }) as Task;

			await task;

			return task.HandleTask<T>();
		}

		public async Task<T> CreateAsync()
		{
			var task = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.CreateAsync), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as Task;

			await task;

			return task.HandleTask<T>();
		}

		public T CreateChild()
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.CreateChild), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as T;
		}

		public T CreateChild(params object[] parameters)
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.CreateChild), new[] { typeof(object[]) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { parameters }) as T;
		}

		public event EventHandler<DataPortalResult<T>> CreateCompleted;

		public void Delete(object criteria)
		{
			typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Delete), new[] { this.concreteType })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria });
    }

		public Task DeleteAsync(object criteria)
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.DeleteAsync), new [] { typeof(object) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria }) as Task;
		}

		public event EventHandler<DataPortalResult<T>> DeleteCompleted;

		public T Execute(T obj)
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Execute))
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { obj }) as T;
        }

		public async Task<T> ExecuteAsync(T command)
		{
			var task = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.ExecuteAsync))
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { command }) as Task;

			await task;

			return task.HandleTask<T>();
		}

		public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

        public T Fetch()
        {
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Fetch), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as T;
        }

        public T Fetch(object criteria)
        {
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Fetch), new[] { typeof(object) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria }) as T;
        }

        public async Task<T> FetchAsync(object criteria)
		{
            var result = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.FetchAsync), new[] { typeof(object) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { criteria }) as Task;

			await result;

			return result.HandleTask<T>();
		}

		public async Task<T> FetchAsync()
		{
			var task = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.FetchAsync), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as Task;

			await task;

			return task.HandleTask<T>();
		}

		public T FetchChild()
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.FetchChild), Type.EmptyTypes)
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, null) as T;
		}

		public T FetchChild(params object[] parameters)
		{
			return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.FetchChild), new[] { typeof(object[]) })
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { parameters }) as T;
		}

		public event EventHandler<DataPortalResult<T>> FetchCompleted;

		public ContextDictionary GlobalContext
		{
			get { return ApplicationContext.GlobalContext; }
		}

        public T Update(T obj)
        {
            return typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.Update))
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { obj }) as T;
        }

        public async Task<T> UpdateAsync(T obj)
		{
			var task = typeof(DataPortal)
				.GetTypeInfo()
				.GetMethod(nameof(DataPortal.UpdateAsync))
				.MakeGenericMethod(this.concreteType)
				.Invoke(null, new object[] { obj }) as Task;

			await task;

			return task.HandleTask<T>();
		}

		public event EventHandler<DataPortalResult<T>> UpdateCompleted;
	}
#pragma warning restore 67
}
