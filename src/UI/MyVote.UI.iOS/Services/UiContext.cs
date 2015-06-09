using UIKit;

namespace MyVote.UI.Services
{
	public sealed class UiContext : IUiContext
	{
		public UIViewController CurrentContext
		{
			get { return UIApplication.SharedApplication.KeyWindow.RootViewController; }
			set
			{
			}
		}
	}
}