using Android.App;

namespace MyVote.UI.Services
{
    public class UIContext : IUIContext
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