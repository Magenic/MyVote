using MyVote.UI.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace MyVote.UI.Views
{
	public class MyVotePage : Page
    {
        public MyVotePage()
        {
        }

		protected override void OnNavigatedTo(NavigationEventArgs e)
		{
			base.OnNavigatedTo(e);

			if (e.Parameter is ViewModelBase)
			{
				DataContext = e.Parameter;
			}
		}
	}
}