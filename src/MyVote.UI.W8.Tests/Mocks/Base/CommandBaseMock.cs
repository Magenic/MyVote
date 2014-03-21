using MyVote.BusinessObjects.Core.Contracts;
using System;
using System.ComponentModel;

namespace MyVote.UI.W8.Tests.Mocks.Base
{
	public abstract class CommandBaseMock : ICommandBaseCore
	{
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
