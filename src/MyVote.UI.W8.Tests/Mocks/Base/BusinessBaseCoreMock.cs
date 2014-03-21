using Csla.Core;
using Csla.Rules;
using MyVote.BusinessObjects.Core.Contracts;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks.Base
{
	public abstract class BusinessBaseCoreMock : IBusinessBaseCore
	{

		public Func<BrokenRulesCollection> BrokenRulesCollectionDelegate { get; set; }
		public BrokenRulesCollection BrokenRulesCollection
		{
			get
			{
				if (BrokenRulesCollectionDelegate != null)
				{
					return BrokenRulesCollectionDelegate();
				}
				else
				{
					return null;
				}
			}
		}

		public virtual void BeginSave(object userState)
		{
			throw new NotImplementedException();
		}

		public virtual void BeginSave()
		{
			throw new NotImplementedException();
		}

		public Func<object> SaveAsyncForcedDelegate { get; set; }
		public Task<object> SaveAsync(bool forceUpdate)
		{
			if (SaveAsyncForcedDelegate != null)
			{
				return Task.Run<object>(SaveAsyncForcedDelegate);
			}
			else
			{
				return Task.Run<object>(() => { return this; });
			}
		}

		public Func<object> SaveAsyncDelegate { get; set; }
		public Task<object> SaveAsync()
		{
			if (SaveAsyncDelegate != null)
			{
				return Task.Run<object>(SaveAsyncDelegate);
			}
			else
			{
				return Task.Run<object>(() => { return this; });
			}
		}

		public virtual void SaveComplete(object newObject)
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

		public Action DeleteDelegate { get; set; }
		public virtual void Delete()
		{
			if (DeleteDelegate != null)
			{
				DeleteDelegate();
			}
		}

		public virtual void DeleteChild()
		{
			throw new NotImplementedException();
		}

		public int EditLevelAdded { get; set; }

		public virtual void SetParent(Csla.Core.IParent parent)
		{
			throw new NotImplementedException();
		}

		public virtual void ApplyEdit()
		{
			throw new NotImplementedException();
		}

		public virtual void BeginEdit()
		{
			throw new NotImplementedException();
		}

		public virtual void CancelEdit()
		{
			throw new NotImplementedException();
		}

		public virtual void AcceptChanges(int parentEditLevel, bool parentBindingEdit)
		{
			throw new NotImplementedException();
		}

		public virtual void CopyState(int parentEditLevel, bool parentBindingEdit)
		{
			throw new NotImplementedException();
		}

		public int EditLevel { get; set; }

		public virtual void UndoChanges(int parentEditLevel, bool parentBindingEdit)
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

		public virtual void EndEdit()
		{
			throw new NotImplementedException();
		}

		public virtual bool CanExecuteMethod(string methodName)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanReadProperty(string propertyName)
		{
			throw new NotImplementedException();
		}

		public virtual bool CanWriteProperty(string propertyName)
		{
			throw new NotImplementedException();
		}

		public virtual void ApplyEditChild(Csla.Core.IEditableBusinessObject child)
		{
			throw new NotImplementedException();
		}

		public Csla.Core.IParent Parent { get; set; }

		public virtual void RemoveChild(Csla.Core.IEditableBusinessObject child)
		{
			throw new NotImplementedException();
		}

		public virtual void AllRulesComplete()
		{
			throw new NotImplementedException();
		}

		public virtual void RuleComplete(string property)
		{
			throw new NotImplementedException();
		}

		public virtual void RuleComplete(Csla.Core.IPropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public virtual void RuleStart(Csla.Core.IPropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public virtual void CheckRules()
		{
			throw new NotImplementedException();
		}

		public virtual Csla.Rules.BrokenRulesCollection GetBrokenRules()
		{
			throw new NotImplementedException();
		}

		public virtual void ResumeRuleChecking()
		{
			throw new NotImplementedException();
		}

		public virtual void SuppressRuleChecking()
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

		public virtual void Deserialized()
		{
			throw new NotImplementedException();
		}

		public event EventHandler<System.ComponentModel.DataErrorsChangedEventArgs> ErrorsChanged;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaiseErrorsChanged(DataErrorsChangedEventArgs args)
		{
			if (ErrorsChanged != null)
			{
				ErrorsChanged(this, args);
			}
		}

		public virtual System.Collections.IEnumerable GetErrors(string propertyName)
		{
			throw new NotImplementedException();
		}

		public bool HasErrors { get; set; }

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
		public void RaisePropertyChanged(PropertyChangedEventArgs args)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, args);
			}
		}

		public virtual void GetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
		{
			throw new NotImplementedException();
		}

		public virtual void GetState(Csla.Serialization.Mobile.SerializationInfo info)
		{
			throw new NotImplementedException();
		}

		public virtual void SetChildren(Csla.Serialization.Mobile.SerializationInfo info, Csla.Serialization.Mobile.MobileFormatter formatter)
		{
			throw new NotImplementedException();
		}

		public virtual void SetState(Csla.Serialization.Mobile.SerializationInfo info)
		{
			throw new NotImplementedException();
		}

	 public bool CanExecuteMethod(IMemberInfo method)
	 {
		throw new NotImplementedException();
	 }

	 public bool CanReadProperty(IPropertyInfo property)
	 {
		throw new NotImplementedException();
	 }


	 public bool CanWriteProperty(IPropertyInfo property)
	 {
		throw new NotImplementedException();
	 }

	 public object Clone()
	 {
		 throw new NotImplementedException();
	 }
	}
}
