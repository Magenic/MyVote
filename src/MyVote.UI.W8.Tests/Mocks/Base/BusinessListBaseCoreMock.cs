using Csla.Core;
using MyVote.BusinessObjects.Core.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks.Base
{
	[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix")]
	public class BusinessListBaseCoreMock<T> : List<T>, IBusinessListBaseCore<T>
	{
		public object GetDeletedList()
		{
			throw new NotImplementedException();
		}

		public void RemoveChild(Csla.Core.IEditableBusinessObject child)
		{
			throw new NotImplementedException();
		}

		public void SetParent(Csla.Core.IParent parent)
		{
			throw new NotImplementedException();
		}

		public void ApplyEdit()
		{
			throw new NotImplementedException();
		}

		public void BeginEdit()
		{
			throw new NotImplementedException();
		}

		public void CancelEdit()
		{
			throw new NotImplementedException();
		}

		public bool IsChild { get; set; }

		public bool IsDeleted { get; set; }

		public bool IsDirty { get; set; }

		public bool IsNew { get; set; }

		public bool IsSavable { get; set; }

		public bool IsSelfDirty { get; set; }

		public bool IsSelfValid { get; set; }

		public bool IsValid { get; set; }

		public event Csla.Core.BusyChangedEventHandler BusyChanged;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseBusyChanged(BusyChangedEventArgs args)
		{
			if (BusyChanged != null)
			{
				BusyChanged(this, args);
			}
		}

		public bool IsBusy { get; set; }

		public bool IsSelfBusy { get; set; }

		public event EventHandler<Csla.Core.ErrorEventArgs> UnhandledAsyncException;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseUnhandledAsyncException(ErrorEventArgs args)
		{
			if (UnhandledAsyncException != null)
			{
				UnhandledAsyncException(this, args);
			}
		}

		public void AcceptChanges(int parentEditLevel, bool parentBindingEdit)
		{
			throw new NotImplementedException();
		}

		public void CopyState(int parentEditLevel, bool parentBindingEdit)
		{
			throw new NotImplementedException();
		}

		public int EditLevel { get; set; }

		public void UndoChanges(int parentEditLevel, bool parentBindingEdit)
		{
			throw new NotImplementedException();
		}

		public void BeginSave(object userState)
		{
			throw new NotImplementedException();
		}

		public void BeginSave()
		{
			throw new NotImplementedException();
		}

		public Task<object> SaveAsync(bool forceUpdate)
		{
			throw new NotImplementedException();
		}

		public Task<object> SaveAsync()
		{
			throw new NotImplementedException();
		}

		public void SaveComplete(object newObject)
		{
			throw new NotImplementedException();
		}

		public event EventHandler<Csla.Core.SavedEventArgs> Saved;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseSaved(SavedEventArgs args)
		{
			if (Saved != null)
			{
				Saved(this, args);
			}
		}

		public void ApplyEditChild(Csla.Core.IEditableBusinessObject child)
		{
			throw new NotImplementedException();
		}

		public Csla.Core.IParent Parent { get; set; }

		public event EventHandler<Csla.Core.RemovingItemEventArgs> RemovingItem;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseRemovingItem(RemovingItemEventArgs args)
		{
			if (RemovingItem != null)
			{
				RemovingItem(this, args);
			}
		}

		public void Deserialized()
		{
			throw new NotImplementedException();
		}

		public event EventHandler<Csla.Core.ChildChangedEventArgs> ChildChanged;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseChildChanged(ChildChangedEventArgs args)
		{
			if (ChildChanged != null)
			{
				ChildChanged(this, args);
			}
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
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			if (CollectionChanged != null)
			{
				CollectionChanged(this, args);
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, args);
			}
		}
	}
}
