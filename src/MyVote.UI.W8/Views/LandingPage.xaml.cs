using MyVote.UI.ViewModels;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MyVote.UI.Views
{
	/// <summary>
	/// An empty page that can be used on its own or navigated to within a Frame.
	/// </summary>
	public sealed partial class LandingPage : ResizablePage
	{
		LandingPageViewModel viewModel;

		public LandingPage()
		{
			this.InitializeComponent();

			this.Loaded += LandingPage_Loaded;
		}

		void LandingPage_Loaded(object sender, RoutedEventArgs e)
		{
			VisualStateManager.GoToState(this, "DoneLoading", true);

			if (this.DataContext != null)
			{
				viewModel = this.DataContext as LandingPageViewModel;
				viewModel.PropertyChanged += viewModel_PropertyChanged;

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
			var isSnapped = DetermineVisualState() == NarrowState;

			if (viewModel == null)
				return;

			if (viewModel.IsBusy)
                VisualStateManager.GoToState(this, string.Format("{0}Busy", isSnapped ? NarrowState : string.Empty), true);
			else
                VisualStateManager.GoToState(this, string.Format("{0}Idle", isSnapped ? NarrowState : string.Empty), true);
		}

		/// <summary>
		/// Invoked when this page is about to be displayed in a Frame.
		/// </summary>
		/// <param name="e">Event data that describes how this page was reached.  The Parameter
		/// property is typically used to configure the page.</param>
		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
		}
	}

}
