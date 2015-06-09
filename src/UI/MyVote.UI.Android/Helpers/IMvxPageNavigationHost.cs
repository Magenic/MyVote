// NOTE: Created from Cheesebaron's MVVM Cross/Xamarin.Forms example at:
// https://github.com/Cheesebaron/Xam.Forms.Mvx

namespace MyVote.UI.Helpers
{
	public interface IMvxPageNavigationHost
	{
		IMvxPageNavigationProvider NavigationProvider { get; set; } 
	}
}