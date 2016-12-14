using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyVote.Services.AppServer.Filters
{
	public sealed class UnhandledExceptionFilter
		: IExceptionFilter
	{
		public void OnException(ExceptionContext context)
		{
			var exception = context.Exception;

			context.Result = new ObjectResult(
				new ExceptionResult(exception))
			{
				StatusCode = StatusCodes.Status500InternalServerError
			};
		}
	}
}
