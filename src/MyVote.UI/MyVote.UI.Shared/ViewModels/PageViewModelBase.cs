using Cirrious.MvvmCross.ViewModels;

namespace MyVote.UI.ViewModels
{
    public abstract class PageViewModelBase : MvxViewModel
    {

        protected PageViewModelBase()
        {
            
        }

		public void GoBack()
		{
            this.Close(this);
		}

		private bool isBusy;
		public bool IsBusy
		{
			get { return this.isBusy; }
			set
			{
				this.isBusy = value;
                this.RaisePropertyChanged(() => this.IsBusy);
			}
		}
	}
}