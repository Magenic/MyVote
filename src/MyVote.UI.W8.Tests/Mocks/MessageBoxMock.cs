using MyVote.UI.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyVote.UI.W8.Tests.Mocks
{
	public class MessageBoxMock : IMessageBox
	{
		public Func<string, bool?> ShowAsyncDelegate { get; set; }
		public Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			if (ShowAsyncDelegate != null)
			{
				return Task.FromResult(ShowAsyncDelegate(content));
			}
			else
			{
				return Task.FromResult((bool?)null);
			}
		}

		public Func<string, string, MessageBoxButtons, bool?> ShowAsyncWithTitleDelegate { get; set; }
		public Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			if (ShowAsyncWithTitleDelegate != null)
			{
				return Task.FromResult(ShowAsyncWithTitleDelegate(content, title, messageBoxButtons));
			}
			else
			{
				return Task.FromResult((bool?)null);
			}
		}
	}
}
