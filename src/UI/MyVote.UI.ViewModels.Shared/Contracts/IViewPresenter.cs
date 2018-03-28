using System;
using System.Threading.Tasks;
using MyVote.UI.Helpers;

namespace MyVote.UI.Contracts
{
    public interface IViewPresenter
    {
        Task ShowAsync(ViewModelRequest request);
        void Close();
        void ChangePresentation(object hint);
    }
}