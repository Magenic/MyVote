using MonoTouch.UIKit;

namespace MyVote.UI.Services
{
    public class UIContext : IUIContext
    {
        private static UIViewController currentContext;

        public UIViewController CurrentContext
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