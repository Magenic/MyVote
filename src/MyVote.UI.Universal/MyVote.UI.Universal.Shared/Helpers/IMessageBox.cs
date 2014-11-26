using System;
using System.Threading.Tasks;
#if ANDROID
using Android.Content;
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
#if WINDOWS_PHONE
		bool? Show(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
		bool? Show(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
#elif ANDROID
		void Show(Context context, string content);
		void Show(Context context, string content, string title);
        void ShowYesNo(Context context, string message, Action onSuccess, Action onNo);
#else
		Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
		Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok);
#endif // WINDOWS_PHONE
	}
}
