using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Core.Contracts;

namespace MyVote.UI.W8.Tests.Mocks.Base
{
#pragma warning disable 67
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public sealed class ReadOnlyListBaseCoreMock<T>
		: IReadOnlyListBaseCore<T>
	{
		private List<T> items;

		public ReadOnlyListBaseCoreMock(List<T> items)
		{
			this.items = items;
		}

		public event EventHandler<Csla.Core.RemovingItemEventArgs> RemovingItem;

		public event Csla.Core.BusyChangedEventHandler BusyChanged;

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public bool IsBusy
		{
			get { throw new NotImplementedException(); }
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public bool IsSelfBusy
		{
			get { throw new NotImplementedException(); }
		}

		public event EventHandler<Csla.Core.ErrorEventArgs> UnhandledAsyncException;

		public event EventHandler<Csla.Core.ChildChangedEventArgs> ChildChanged;

		public void Deserialized()
		{
			throw new NotImplementedException();
		}

		public void GetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
		{
			throw new NotImplementedException();
		}

		public void GetState(Csla.Serialization.Mobile.SerializationInfo info)
		{
			throw new NotImplementedException();
		}

		public void SetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
		{
			throw new NotImplementedException();
		}

		public void SetState(Csla.Serialization.Mobile.SerializationInfo info)
		{
			throw new NotImplementedException();
		}

		public event System.Collections.Specialized.NotifyCollectionChangedEventHandler CollectionChanged;

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		public int IndexOf(T item)
		{
			throw new NotImplementedException();
		}

		public void Insert(int index, T item)
		{
			throw new NotImplementedException();
		}

		public void RemoveAt(int index)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public T this[int index]
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}

		public void Add(T item)
		{
			throw new NotImplementedException();
		}

		public void Clear()
		{
			throw new NotImplementedException();
		}

		public bool Contains(T item)
		{
			throw new NotImplementedException();
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			throw new NotImplementedException();
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public int Count
		{
			get { throw new NotImplementedException(); }
		}

		[SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations")]
		public bool IsReadOnly
		{
			get { throw new NotImplementedException(); }
		}

		public bool Remove(T item)
		{
			throw new NotImplementedException();
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.items.GetEnumerator();
		}

		System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
		{
			return this.items.GetEnumerator();
		}
	}
#pragma warning restore 67
}
