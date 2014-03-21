using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyVote.UI.Helpers
{
	public interface ISecondaryPinner
	{
		Task<bool> PinPoll(FrameworkElement anchorElement, int pollId, string question);
		Task<bool> UnpinPoll(FrameworkElement anchorElement, int pollId);
		bool IsPollPinned(int pollId);
	}
}
