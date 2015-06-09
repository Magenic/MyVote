using System;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MyVote.Services.AppServer.Controllers
{
	public static class Extensions
	{
		public static HttpResponseException ToHttpResponseException(this Exception ex, HttpRequestMessage request)
		{
			return new HttpResponseException(
			  new HttpResponseMessage
			  {
				  StatusCode = HttpStatusCode.BadRequest,
				  ReasonPhrase = ex.Message.Replace(Environment.NewLine, " "),
				  Content = new StringContent(ex.ToString()),
				  RequestMessage = request
			  });
		}
	}
}