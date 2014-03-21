using System.ComponentModel;
using Csla;

namespace MyVote.BusinessObjects.Core.Contracts
{
	public interface IBusinessBaseCore
		: IBusinessBase, INotifyPropertyChanged { }
}
