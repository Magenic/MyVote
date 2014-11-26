using System;
using System.Threading.Tasks;
#if !__ANDROID__
using Windows.UI.Popups;
#else
using Android.App;
using Android.Content;
#endif

namespace MyVote.UI.Helpers
{
	public sealed class MessageBox : IMessageBox
	{
#if WINDOWS_PHONE
		public bool? Show(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			return Show(content, string.Empty, messageBoxButtons);
		}

		public bool? Show(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			var result = System.Windows.MessageBox.Show(content, title, ConvertButtons(messageBoxButtons));

			if (result == System.Windows.MessageBoxResult.OK)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		private static System.Windows.MessageBoxButton ConvertButtons(MessageBoxButtons messageBoxButtons)
		{
			switch (messageBoxButtons)
			{
				case MessageBoxButtons.Cancel:
				case MessageBoxButtons.OkCancel:
					return System.Windows.MessageBoxButton.OKCancel;

				case MessageBoxButtons.Ok:
				default:
					return System.Windows.MessageBoxButton.OK;
			}
		}

#elif __ANDROID__
		public void Show(Context context, string content)
		{
			var dialog = new ProgressDialog(context);
			dialog.SetMessage(content);
			dialog.SetCancelable(false);
			dialog.Show();
			System.Threading.Thread.Sleep (2000);
			dialog.Hide ();
		}

		public void Show(Context context, string content, string title)
		{
			var dialog = new ProgressDialog(context);
			dialog.SetMessage(content);
			dialog.SetTitle (title);
			dialog.SetCancelable(false);
			dialog.Show();
			System.Threading.Thread.Sleep (2000);
			dialog.Hide ();
		}

        public void ShowYesNo(Context context, string message, Action onSuccess, Action onNo)
        {
            AlertDialog.Builder builder = new AlertDialog.Builder(context);
            builder.SetMessage("Are you sure?")
                .SetPositiveButton("Yes", (a,b)=> onSuccess() )
                .SetNegativeButton("No", (a,b)=> onNo() ).Show();
        }
#else
		public async Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			var messageDialog = new MessageDialog(content);
			bool? result = null;
			MessageBox.AddButtons(messageDialog, messageBoxButtons,
				new UICommandInvokedHandler((cmd) => result = true),
				new UICommandInvokedHandler((cmd) => result = false));

			await messageDialog.ShowAsync();

			return result;
		}

		public async Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			var messageDialog = new MessageDialog(content, title);
			bool? result = null;
			MessageBox.AddButtons(messageDialog, messageBoxButtons,
				new UICommandInvokedHandler((cmd) => result = true),
				new UICommandInvokedHandler((cmd) => result = false));

			await messageDialog.ShowAsync();

			return result;
		}

		private static void AddButtons(
			MessageDialog messageDialog,
			MessageBoxButtons messageBoxButtons,
			UICommandInvokedHandler positiveHandler,
			UICommandInvokedHandler negativeHandler)
		{
			if ((messageBoxButtons & MessageBoxButtons.Ok) == MessageBoxButtons.Ok)
			{
				messageDialog.Commands.Add(new UICommand("OK", positiveHandler));
			}

			if ((messageBoxButtons & MessageBoxButtons.Cancel) == MessageBoxButtons.Cancel)
			{
				messageDialog.Commands.Add(new UICommand("Cancel", negativeHandler));
			}

			if ((messageBoxButtons & MessageBoxButtons.Yes) == MessageBoxButtons.Yes)
			{
				messageDialog.Commands.Add(new UICommand("Yes", positiveHandler));
			}

			if ((messageBoxButtons & MessageBoxButtons.No) == MessageBoxButtons.No)
			{
				messageDialog.Commands.Add(new UICommand("No", negativeHandler));
			}
		}
#endif // WINDOWS_PHONE
	}
}
