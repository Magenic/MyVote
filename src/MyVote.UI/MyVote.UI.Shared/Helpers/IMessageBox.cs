using System;
using System.Threading.Tasks;
#if __MOBILE__
using Xamarin.Forms;
#endif

namespace MyVote.UI.Helpers
{
	[Flags]
	public enum MessageBoxButtons
	{
		Ok = 1,
		Cancel = 2,
		OkCancel = Ok | Cancel,
#if NETFX_CORE
		Yes = 4,
		No = 8,
		YesNo = Yes | No
#endif // NETFX_CORE
	}

	public interface IMessageBox
	{
		Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
		Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
	}
}