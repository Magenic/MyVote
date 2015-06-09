using System;
using System.Collections.Generic;
using System.Text;

namespace MyVote.UI.Helpers
{
    public sealed class Logger : ILogger
    {
		public void Log(Exception exception)
		{
			System.Diagnostics.Debug.WriteLine(exception);

#if MOBILE
			Xamarin.Insights.Report(exception);
#endif // MOBILE
		}
	}
}
