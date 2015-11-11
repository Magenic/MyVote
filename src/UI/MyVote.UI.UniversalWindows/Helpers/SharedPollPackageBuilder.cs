using MyVote.UI.Models;
using System;
using System.Text;
using Windows.ApplicationModel.DataTransfer;

namespace MyVote.UI.Helpers
{
	public static class SharedPollPackageBuilder
	{
		public static void Build(SharedPoll poll, DataPackage dataPackage)
		{
			if (poll == null)
			{
				throw new ArgumentNullException("poll");
			}
			if (dataPackage == null)
			{
				throw new ArgumentNullException("dataPackage");
			}

			var url = string.Format("myvote://poll/{0}", poll.PollId);

			// Set as many data types as we can.
			dataPackage.Properties.Title = string.Format("MyVote Poll: {0}", poll.Question);

			// Add an HTML version.
			var htmlBuilder = new StringBuilder();
			htmlBuilder.AppendFormat("<h1>{0}</h1>", poll.Question);
			htmlBuilder.AppendFormat("<p>{0}</p>", poll.Description);
			htmlBuilder.Append("<ol>");
			foreach (var option in poll.Options)
			{
				htmlBuilder.AppendFormat("<li>{0}</li>", option);
			}
			htmlBuilder.Append("</ol>");
			htmlBuilder.AppendFormat("<p><a href='{0}'>Cast your vote!</a></p>", url);
			var html = HtmlFormatHelper.CreateHtmlFormat(htmlBuilder.ToString());
			dataPackage.SetHtmlFormat(html);

			// Add a text only version
			var text = string.Format("{0} Cast your vote! {1}", poll.Question, url);
			dataPackage.SetText(text);

			// Add a Uri
			dataPackage.SetApplicationLink(new Uri(url, UriKind.Absolute));
		}
	}
}
