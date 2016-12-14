using FluentAssertions;
using MyVote.Services.AppServer.Filters;
using System;
using Xunit;

namespace MyVote.Services.AppServer.Tests.Filters
{
	public sealed class ExceptionResultTests
	{
		private const string message = "my message";

		[Fact]
		public void Create()
		{
			try
			{
				throw new ArgumentException(message);
			}
			catch(ArgumentException e)
			{
				var result = new ExceptionResult(e);
				result.Message.Should().Be(message);
				result.StackTrace.Should().NotBeNullOrWhiteSpace();
				result.Inner.Should().BeNull();
			}
		}

		[Fact]
		public void CreateWithInnerException()
		{
			try
			{
				throw new ArgumentException(message, 
					new ArgumentException());
			}
			catch (ArgumentException e)
			{
				var result = new ExceptionResult(e);
				result.Message.Should().Be(message);
				result.StackTrace.Should().NotBeNullOrWhiteSpace();
				result.Inner.Should().NotBeNull();
			}
		}

		[Fact]
		public void CreateWhenExceptionIsNull()
		{
			new Action(() => new ExceptionResult(null)).ShouldThrow<ArgumentNullException>();
		}
	}
}
