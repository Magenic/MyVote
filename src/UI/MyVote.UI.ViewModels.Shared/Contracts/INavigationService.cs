using System;
using MyVote.UI.ViewModels;

namespace MyVote.UI.Contracts
{
    public interface INavigationService
    {
        void ShowViewModel<T>() where T : ViewModelBase;
        void ShowViewModel<T>(object parameters) where T : ViewModelBase;
        void Close();
        void ChangePresentation(object hint);
    }
}