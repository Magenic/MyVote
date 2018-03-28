// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;
using Autofac;
using MyVote.UI.Services;

namespace MyVote.UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : MyVotePage
    {
		private NavigatingViewModelBase ViewModel { get; set; }

        public LandingPage()
        {
            this.InitializeComponent();
            this.Loaded += LandingPage_Loaded;
        }
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is Uri)
			{
				Ioc.Container.Resolve<IMobileService>().ResumeWithUrl(e.Parameter as Uri);
			}
		}
		void LandingPage_Loaded(object sender, RoutedEventArgs e)
        {
            VisualStateManager.GoToState(this, "DoneLoading", true);

            if (this.DataContext != null)
            {
				this.ViewModel = (NavigatingViewModelBase)this.DataContext;
                this.ViewModel.PropertyChanged += viewModel_PropertyChanged;

                UpdateState();
            }
        }

        void viewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
                UpdateState();
        }

        private void UpdateState()
        {
            if (this.ViewModel == null)
                return;

            if (this.ViewModel.IsBusy)
                VisualStateManager.GoToState(this, "Busy", true);
            else
                VisualStateManager.GoToState(this, "Idle", true);
        }
    }
}
