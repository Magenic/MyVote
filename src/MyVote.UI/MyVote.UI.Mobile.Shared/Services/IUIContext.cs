#if __ANDROID__
using Android.App;
#elif __IOS__
using MonoTouch.UIKit;
#endif

namespace MyVote.UI.Services
{
    public interface IUIContext
    {
#if __ANDROID__
        Activity CurrentContext { get; set; }
#elif __IOS__
        UIViewController CurrentContext { get; set; }
#endif
    }
}