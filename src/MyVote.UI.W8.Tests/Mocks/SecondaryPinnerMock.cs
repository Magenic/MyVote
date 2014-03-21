using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class SecondaryPinnerMock : ISecondaryPinner
	{
		public Func<FrameworkElement, int, string, bool> PinPollDelegate { get; set; }
		public async Task<bool> PinPoll(FrameworkElement anchorElement, int pollId, string question)
		{
			if (PinPollDelegate != null)
			{
				return await Task.FromResult<bool>(PinPollDelegate(anchorElement, pollId, question));
			}
			else
			{
				return await Task.FromResult<bool>(false);
			}
		}

		public Func<FrameworkElement, int, bool> UnpinPollDelegate { get; set; }
		public async Task<bool> UnpinPoll(FrameworkElement anchorElement, int pollId)
		{
			if (UnpinPollDelegate != null)
			{
				return await Task.FromResult<bool>(UnpinPollDelegate(anchorElement, pollId));
			}
			else
			{
				return await Task.FromResult<bool>(false);
			}
		}

		public Func<int, bool> IsPollPinnedDelegate { get; set; }
		public bool IsPollPinned(int pollId)
		{
			if (IsPollPinnedDelegate != null)
			{
				return IsPollPinnedDelegate(pollId);
			}
			else
			{
				return false;
			}
		}
	}
}
