using System;
using System.Diagnostics.CodeAnalysis;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class UserIdentityMock
		: IUserIdentity
	{
		public string ProfileID { get; set; }

		public string AuthenticationType { get; set; }

		public bool IsAuthenticated { get; set; }

		public string Name { get; set; }

		public string UserName { get; set; }

		public Func<string, bool> IsInRoleDelegate { get; set; }
		public bool IsInRole(string role)
		{
			if (IsInRoleDelegate != null)
			{
				return IsInRoleDelegate(role);
			}
			else
			{
				return false;
			}
		}

		public int? UserID { get; set; }

		public bool CanReadProperty(string propertyName)
		{
			throw new NotImplementedException();
		}

		public void Deserialized()
		{
			throw new NotImplementedException();
		}

		public bool CanExecuteMethod(Csla.Core.IMemberInfo method)
		{
			throw new NotImplementedException();
		}

		public bool CanExecuteMethod(string methodName)
		{
			throw new NotImplementedException();
		}

		public bool CanReadProperty(Csla.Core.IPropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public bool CanWriteProperty(Csla.Core.IPropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public bool CanWriteProperty(string propertyName)
		{
			throw new NotImplementedException();
		}

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

		public void AllRulesComplete()
		{
			throw new NotImplementedException();
		}

		public void RuleComplete(string property)
		{
			throw new NotImplementedException();
		}

		public void RuleComplete(Csla.Core.IPropertyInfo property)
		{
			throw new NotImplementedException();
		}

		public void RuleStart(Csla.Core.IPropertyInfo property)
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

#pragma warning disable 67
		public event Csla.Core.BusyChangedEventHandler BusyChanged;
		public event EventHandler<Csla.Core.ErrorEventArgs> UnhandledAsyncException;
		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
#pragma warning restore 67
	}
}
