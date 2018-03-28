using System;
using System.Windows.Input;
using MyVote.UI.Contracts;
using MyVote.UI.Helpers;

namespace MyVote.UI.ViewModels
{
    public class NavigatingViewModelBase : ViewModelBase
    {
        protected INavigationService navigationService;

        public NavigatingViewModelBase(INavigationService navigationService)
        {
            this.navigationService = navigationService;
        }

        public ICommand GoBack
        {
            get { return new Command(() => navigationService.Close()); }
        }

        private bool isBusy;
        public bool IsBusy
        {
            get { return this.isBusy; }
            set
            {
                this.isBusy = value;
                this.RaisePropertyChanged(nameof(IsBusy));
            }
        }

        private bool isEnabled;

        public bool IsEnabled
        {
            get { return this.isEnabled; }
            set
            {
                this.isEnabled = value;
                this.RaisePropertyChanged(nameof(IsEnabled));
            }
        }
    }
}