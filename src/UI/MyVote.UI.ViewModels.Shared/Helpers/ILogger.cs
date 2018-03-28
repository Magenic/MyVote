using System;

namespace MyVote.UI.Helpers
{
    public interface ILogger
    {
		void Log(Exception exception);
        void Information(string message, string key);
    }
}
