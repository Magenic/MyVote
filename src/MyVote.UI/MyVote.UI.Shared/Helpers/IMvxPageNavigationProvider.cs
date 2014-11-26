using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public interface IMvxPageNavigationProvider
    {
        void Push(Page page);
        void Pop();
    }
}
