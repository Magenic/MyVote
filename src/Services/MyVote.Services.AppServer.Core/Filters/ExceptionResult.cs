using System;

namespace MyVote.Services.AppServer.Filters
{
	internal sealed class ExceptionResult
	{
		public ExceptionResult(Exception exception)
		{
			if (exception == null)
			{
				throw new ArgumentNullException(nameof(exception));
			}

			this.Message = exception.Message;
			this.StackTrace = exception.StackTrace;

			if(exception.InnerException != null)
			{
				this.Inner = new ExceptionResult(exception.InnerException);
			}
		}

		public ExceptionResult Inner { get; }
		public string Message { get; }
		public string StackTrace { get; }
	}
}
