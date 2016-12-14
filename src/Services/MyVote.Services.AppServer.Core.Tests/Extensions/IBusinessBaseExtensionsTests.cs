using Csla;
using Csla.Rules;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Threading.Tasks;
using Xunit;
using static MyVote.Services.AppServer.Extensions.IBusinessBaseExtensions;

namespace MyVote.Services.AppServer.Tests.Extensions
{
	public sealed class IBusinessBaseExtensionsTests
	{
		[Fact]
		public async Task PersistAsync()
		{
			var businessBase = new Mock<IBusinessBase>(MockBehavior.Strict);
			businessBase.SetupGet(_ => _.IsSavable).Returns(true);
			businessBase.Setup(_ => _.SaveAsync()).Returns(Task.FromResult<object>(new object()));

			var result = await businessBase.Object.PersistAsync();

			result.Should().BeOfType<NoContentResult>();
			businessBase.VerifyAll();
		}

		[Fact]
		public async Task PersistAsyncWithDifferentResult()
		{
			var businessBase = new Mock<IBusinessBase>(MockBehavior.Strict);
			businessBase.SetupGet(_ => _.IsSavable).Returns(true);
			businessBase.Setup(_ => _.SaveAsync()).Returns(Task.FromResult<object>(new object()));

			var result = await businessBase.Object.PersistAsync(
				() => new OkResult());

			result.Should().BeOfType<OkResult>();
			businessBase.VerifyAll();
		}

		[Fact]
		public async Task PersistAsyncWhenObjectIsNotSavable()
		{
			var brokenRules = new BrokenRulesCollection();
			var businessBase = new Mock<IBusinessBase>(MockBehavior.Strict);
			businessBase.SetupGet(_ => _.IsSavable).Returns(false);
			businessBase.SetupGet(_ => _.IsDirty).Returns(true);
			businessBase.Setup(_ => _.GetBrokenRules()).Returns(brokenRules);

			var result = await businessBase.Object.PersistAsync();

			result.Should().BeOfType<BadRequestObjectResult>();
			businessBase.VerifyAll();
		}
	}
}
