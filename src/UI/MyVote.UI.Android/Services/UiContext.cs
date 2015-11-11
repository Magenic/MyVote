using Android.App;

namespace MyVote.UI.Services
{
	public sealed class UiContext : IUiContext
	{
		private static Activity currentContext;

		public Activity CurrentContext
		{
			get
			{
				return currentContext;
			}
			set
			{
				currentContext = value;
			}
		}
	}
}