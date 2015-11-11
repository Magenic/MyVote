// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

using MyVote.UI.ViewModels;
using Windows.UI.Xaml;

namespace MyVote.UI.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LandingPage : MyVotePage
    {
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
                ((ViewModelBase)this.ViewModel).PropertyChanged += viewModel_PropertyChanged;

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

            if (((ViewModelBase)this.ViewModel).IsBusy)
                VisualStateManager.GoToState(this, "Busy", true);
            else
                VisualStateManager.GoToState(this, "Idle", true);
        }
    }
}
