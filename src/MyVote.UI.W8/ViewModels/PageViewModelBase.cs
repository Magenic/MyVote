using Caliburn.Micro;
using MyVote.UI.Helpers;

namespace MyVote.UI.ViewModels
{
	public abstract class PageViewModelBase : Screen
	{
		protected PageViewModelBase(INavigation navigation)
		{
			this.navigation = navigation;
		}

		public void GoBack()
		{
			this.Navigation.GoBack();
		}

		protected virtual void DeserializeParameter(string value)
		{
		}

		private readonly INavigation navigation;
		protected INavigation Navigation
		{
			get { return navigation; }
		}

		public bool CanGoBack
		{
			get { return navigation.CanGoBack; }
		}

		private bool isBusy;
		public bool IsBusy
		{
			get { return this.isBusy; }
			set
			{
				this.isBusy = value;
				NotifyOfPropertyChange(() => this.IsBusy);
			}
		}

		private string parameter;
		public string Parameter
		{
			get { return this.parameter; }
			set
			{
				this.parameter = value;
				NotifyOfPropertyChange(() => this.Parameter);

				if (value != null)
				{
					this.DeserializeParameter(value);
				}
			}
		}
	}
}
