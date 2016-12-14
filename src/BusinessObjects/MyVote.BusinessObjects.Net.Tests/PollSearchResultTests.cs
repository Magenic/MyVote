using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollSearchResultTests
	{
		[Fact]
		public void Fetch()
		{
			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			var data = EntityCreator.Create<PollSearchResultsData>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollSearchResult>(data);

				result.Id.Should().Be(data.Id);
				result.ImageLink.Should().Be(data.ImageLink);
				result.Question.Should().Be(data.Question);
			}
		}
	}
}
