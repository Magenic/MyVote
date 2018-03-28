using System;
using MyVote.UI.ViewModels;

namespace MyVote.UI.Helpers
{
    public interface IViewModelLoader
    {
		TViewModel LoadViewModel<TViewModel>(object parameter = null, IBundle savedState = null)
			where TViewModel : IViewModel;

        IViewModel LoadViewModel(Type viewModelType, object parameter = null, IBundle savedState = null);
	}
}