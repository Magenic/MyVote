using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Routing;
using MyVote.Services.AppServer.Filters;
using Rocks;
using Rocks.Options;
using System;
using System.Collections.Generic;
using Xunit;

namespace MyVote.Services.AppServer.Tests.Filters
{
	public sealed class UnhandledExceptionFilterTests
	{
		[Fact]
		public void HandleOnException()
		{
			var action = new ActionContext()
			{
				HttpContext = Rock.Make<HttpContext>(
					new RockOptions(allowWarnings: AllowWarnings.Yes)),
				RouteData = new RouteData(),
				ActionDescriptor = new ActionDescriptor()
			};

			var context = new ExceptionContext(action, new List<IFilterMetadata>())
			{
				Exception = new Exception()
			};

			var filter = new UnhandledExceptionFilter();
			filter.OnException(context);

			var result = context.Result as ObjectResult;
			result.Value.Should().BeOfType<ExceptionResult>();
			result.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
		}
	}
}
