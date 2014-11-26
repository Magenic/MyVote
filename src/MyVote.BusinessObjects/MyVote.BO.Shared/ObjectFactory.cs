using System;
using System.Threading.Tasks;
using Csla;
using Csla.Core;
using Csla.Serialization.Mobile;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.BusinessObjects
{
#pragma warning disable 67
	internal sealed class ObjectFactory<T>
		: IObjectFactory<T>
		where T : class, IMobileObject
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

#if (!NETFX_CORE && !WINDOWS_PHONE)||ANDROID||IOS
		public T Create()
		{
			return DataPortal.Create<T>();
		}

		public T Create(object criteria)
		{
			return DataPortal.Create<T>(criteria);
		}
#endif

		public async Task<T> CreateAsync(object criteria)
		{
			return await DataPortal.CreateAsync<T>(criteria);
		}

		public async Task<T> CreateAsync()
		{
			return await DataPortal.CreateAsync<T>();
		}

		public T CreateChild()
		{
			return DataPortal.CreateChild<T>();
		}

		public T CreateChild(params object[] parameters)
		{
			return DataPortal.CreateChild<T>(parameters);
		}

		public event EventHandler<DataPortalResult<T>> CreateCompleted;

#if (!NETFX_CORE && !WINDOWS_PHONE)||ANDROID||IOS
		public void Delete(object criteria)
		{
			DataPortal.Delete<T>(criteria);
		}
#endif

		public Task DeleteAsync(object criteria)
		{
			return DataPortal.DeleteAsync<T>(criteria);
		}

		public event EventHandler<DataPortalResult<T>> DeleteCompleted;

#if (!NETFX_CORE && !WINDOWS_PHONE)||ANDROID||IOS
		public T Execute(T obj)
		{
			return DataPortal.Execute<T>(obj);
		}
#endif

		public async Task<T> ExecuteAsync(T command)
		{
			return await DataPortal.ExecuteAsync<T>(command);
		}

		public event EventHandler<DataPortalResult<T>> ExecuteCompleted;

#if !NETFX_CORE && !WINDOWS_PHONE && !ANDROID && !IOS
		public T Fetch()
		{
			return DataPortal.Fetch<T>();
		}

		public T Fetch(object criteria)
		{
			return DataPortal.Fetch<T>(criteria);
		}
#endif

		public async Task<T> FetchAsync(object criteria)
		{
			return await DataPortal.FetchAsync<T>(criteria);
		}

		public async Task<T> FetchAsync()
		{
			return await DataPortal.FetchAsync<T>();
		}

		public T FetchChild()
		{
			return DataPortal.FetchChild<T>();
		}

		public T FetchChild(params object[] parameters)
		{
			return DataPortal.FetchChild<T>(parameters);
		}

		public event EventHandler<DataPortalResult<T>> FetchCompleted;

		public ContextDictionary GlobalContext
		{
			get { return ApplicationContext.GlobalContext; }
		}

#if (!NETFX_CORE && !WINDOWS_PHONE)||ANDROID||IOS
		public T Update(T obj)
		{
			return DataPortal.Update<T>(obj);
		}
#endif

		public async Task<T> UpdateAsync(T obj)
		{
			return await DataPortal.UpdateAsync<T>(obj);
		}

		public event EventHandler<DataPortalResult<T>> UpdateCompleted;
	}
#pragma warning restore 67
}
