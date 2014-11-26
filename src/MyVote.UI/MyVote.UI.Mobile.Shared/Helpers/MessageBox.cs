using Xamarin.Forms;
using System.Threading.Tasks;

namespace MyVote.UI.Helpers
{
	public sealed class MessageBox : IMessageBox
	{
	    private Page currentPage;

	    public MessageBox(Page page)
	    {
	        this.currentPage = page;
	    }

        public async Task<bool?> ShowAsync(string content, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
        {
            return await ShowAsync(content, string.Empty, messageBoxButtons);
        }

        public async Task<bool?> ShowAsync(string content, string title, MessageBoxButtons messageBoxButtons = MessageBoxButtons.Ok)
        {
            return await currentPage.DisplayAlert(
                title,
                content,
                messageBoxButtons == MessageBoxButtons.Cancel ? string.Empty : "Ok",
                messageBoxButtons == MessageBoxButtons.Ok ? string.Empty : "Cancel");
        }
    }
}