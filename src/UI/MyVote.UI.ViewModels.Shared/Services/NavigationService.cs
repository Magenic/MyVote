using System;
using MyVote.UI.Contracts;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;

namespace MyVote.UI.Services
{
    public class NavigationService : INavigationService
    {
        private readonly IViewPresenter viewPresenter;

        public NavigationService(IViewPresenter viewPresenter)
        {
            this.viewPresenter = viewPresenter;
        }

        public void ChangePresentation(object hint)
        {
            viewPresenter.ChangePresentation(hint);
        }

        public void Close()
        {
            viewPresenter.Close();
        }

        public void ShowViewModel<T>() where T : ViewModelBase
        {
            var request = new ViewModelRequest
            {
                ViewModelType = typeof(T)
            };
            viewPresenter.ShowAsync(request);
        }

        public void ShowViewModel<T>(object parameters) where T : ViewModelBase
        {
            var request = new ViewModelRequest
            {
                ViewModelType = typeof(T),
                Parameters = parameters
            };
            viewPresenter.ShowAsync(request);
        }
    }
}
