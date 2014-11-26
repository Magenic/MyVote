// NOTE: Created from Cheesebaron's MVVM Cross/Xamarin.Forms example at:
// https://github.com/Cheesebaron/Xam.Forms.Mvx

using MyVote.UI.Helpers;

namespace MyVote.UI.MvxDroidAdaptation
{
    public interface IMvxPageNavigationHost 
    { 
        IMvxPageNavigationProvider NavigationProvider { get; set; } 
    } 
 }