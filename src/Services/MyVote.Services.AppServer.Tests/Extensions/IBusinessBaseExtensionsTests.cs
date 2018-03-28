using Csla;
using Csla.Rules;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Rocks;
using Rocks.Options;
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
			var businessBase = Rock.Create<IBusinessBase>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			businessBase.Handle(nameof(IBusinessBase.IsSavable), () => true);
			businessBase.Handle(_ => _.SaveAsync()).Returns(Task.FromResult<object>(new object()));

			var result = await businessBase.Make().PersistAsync();

			result.Should().BeOfType<NoContentResult>();
			businessBase.Verify();
		}

		[Fact]
		public async Task PersistAsyncWithDifferentResult()
		{
			var businessBase = Rock.Create<IBusinessBase>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			businessBase.Handle(nameof(IBusinessBase.IsSavable), () => true);
			businessBase.Handle(_ => _.SaveAsync()).Returns(Task.FromResult<object>(new object()));

			var result = await businessBase.Make().PersistAsync(
				() => new OkResult());

			result.Should().BeOfType<OkResult>();
			businessBase.Verify();
		}

		[Fact]
		public async Task PersistAsyncWhenObjectIsNotSavable()
		{
			var brokenRules = new BrokenRulesCollection();
			var businessBase = Rock.Create<IBusinessBase>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			businessBase.Handle(nameof(IBusinessBase.IsSavable), () => false);
			businessBase.Handle(nameof(IBusinessBase.IsDirty), () => true);
			businessBase.Handle(_ => _.GetBrokenRules()).Returns(brokenRules);

			var result = await businessBase.Make().PersistAsync();

			result.Should().BeOfType<BadRequestObjectResult>();
			businessBase.Verify();
		}
	}
}
