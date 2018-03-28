using System;

namespace MyVote.UI.Helpers
{
    public sealed class Logger : ILogger
    {
        public void Information(string message, string key)
        {
#if MOBILE
            Xamarin.Insights.Track("", key, message);
#endif // MOBILE
        }

        public void Log(Exception exception)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine(exception);
#endif // DEBUG
#if MOBILE
            Xamarin.Insights.Report(exception);
#endif // MOBILE
		}
	}
}
