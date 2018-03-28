using System.Threading.Tasks;
using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
	public class MessageBox : IMessageBox
	{
		private Page currentPage;

		public MessageBox(Page page)
		{
			this.currentPage = page;
		}

		public async Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			return await this.ShowAsync(content, string.Empty, messageBoxButtons);
		}

		public async Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
		{
			return await currentPage.DisplayAlert(
				title,
				content,
				messageBoxButtons == MessageBoxButtons.Cancel ? "Ok1" : "Ok",
				messageBoxButtons == MessageBoxButtons.Ok ? "Cancel1" : "Cancel");
		}
	}
}
