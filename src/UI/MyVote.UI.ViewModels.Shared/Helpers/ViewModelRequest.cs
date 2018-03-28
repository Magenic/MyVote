using MyVote.UI.ViewModels;
using System;
namespace MyVote.UI.Helpers
{
    public class ViewModelRequest
    {
        public ViewModelRequest()
        {
        }

		public ViewModelRequest(Type viewModelType)
		{
			ViewModelType = viewModelType;
		}

        public Type ViewModelType { get; set; }

        public object Parameters { get; set; }
    }

	public class ViewModelRequest<TViewModel> : ViewModelRequest
		where TViewModel : IViewModel
	{
		public ViewModelRequest()
			: base(typeof(TViewModel))
		{
		}
	}
}
