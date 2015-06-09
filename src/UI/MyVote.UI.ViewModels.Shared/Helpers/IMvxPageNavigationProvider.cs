#if MOBILE
using Xamarin.Forms;
#else
using Windows.UI.Xaml.Controls;
#endif // MOBILE

namespace MyVote.UI.Helpers
{
    public interface IMvxPageNavigationProvider
    {
		void Push(Page page);
		void Pop();
    }
}
