using Autofac;
using Csla;
using FluentAssertions;
using Moq;
using MyVote.Data.Entities;
using Spackle.Extensions;
using Xunit;

namespace MyVote.BusinessObjects.Net.Tests
{
	public sealed class PollDataResultTests
	{
		[Fact]
		public void Fetch()
		{
			var data = EntityCreator.Create<PollData>();

			var container = new ContainerBuilder();
			container.RegisterInstance(Mock.Of<IEntities>()).As<IEntities>();

			using (new ObjectActivator(container.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var result = DataPortal.FetchChild<PollDataResult>(data);

				result.PollOptionID.Should().Be(data.PollOptionID);
				result.ResponseCount.Should().Be(data.ResponseCount);
			}
		}
	}
}
