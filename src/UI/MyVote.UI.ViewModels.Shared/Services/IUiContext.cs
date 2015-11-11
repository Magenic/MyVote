#if __ANDROID__
using Android.App;
#elif __IOS__
using UIKit;
#endif

namespace MyVote.UI.Services
{
    public interface IUiContext
    {
#if __ANDROID__
        Activity CurrentContext { get; set; }
#elif __IOS__
		UIViewController CurrentContext { get; set; }
#else
		object CurrentContext { get; set; }
#endif
    }
}
