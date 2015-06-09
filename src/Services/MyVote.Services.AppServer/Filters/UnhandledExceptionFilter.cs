using System;
using System.Web.Http.Filters;

namespace MyVote.Services.AppServer.Filters
{
	public sealed class UnhandledExceptionFilter
		: ExceptionFilterAttribute
	{
		//private readonly ILogger logger;

		public UnhandledExceptionFilter(/*ILogger logger*/)
		{
			//if (logger == null)
			//{
			//	throw new ArgumentNullException("logger");
			//}

			//this.logger = logger;
		}

		public override void OnException(HttpActionExecutedContext context)
		{
			//this.logger.Error(context.Exception,
			//	"Unhandled Exception.");
			Console.WriteLine(context.Exception.ToString());
		}
	}
}
