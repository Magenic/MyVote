using Autofac;
using Csla;
using Csla.Serialization.Mobile;
using FluentAssertions;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.IO;
using Xunit;

namespace MyVote.BusinessObjects.Tests
{
	public sealed class PollSubmissionResponseCollectionTests
	{
		private static BusinessList<IPollOption> CreateOptions()
		{
			var generator = new RandomObjectGenerator();
			var optionId = generator.Generate<int>();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();

			var option = new Mock<IPollOption>(MockBehavior.Loose);
			option.Setup(_ => _.IsChild).Returns(true);
			option.Setup(_ => _.PollOptionID).Returns(optionId);
			option.Setup(_ => _.OptionPosition).Returns(optionPosition);
			option.Setup(_ => _.OptionText).Returns(optionText);

			var options = new BusinessList<IPollOption>();
			options.Add(option.Object);

			return options;
		}

		[Fact]
		public void EnsureCollectionCanBeSerialized()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				var formatter = new MobileFormatter();

				using (var stream = new MemoryStream())
				{
					formatter.Serialize(stream, responses);
					stream.Position = 0;
					var newResponses = formatter.Deserialize(stream) as PollSubmissionResponseCollection;
					newResponses.Count.Should().Be(1);
					newResponses[0].OptionText.Should().Be(responses[0].OptionText);
				}
			}
		}

		[Fact]
		public void Create()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>(MockBehavior.Strict);
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());

				responses.Count.Should().Be(1);
			}

			pollSubmissionResponseFactory.VerifyAll();
		}

		[Fact]
		public void Clear()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Clear()).ShouldThrow<NotSupportedException>();
			}
		}

		[Fact]
		public void Insert()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Insert(1, Mock.Of<IPollSubmissionResponse>())).ShouldThrow<NotSupportedException>();
			}
		}

		[Fact]
		public void Move()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Move(0, 0)).ShouldThrow<NotSupportedException>();
			}
		}

		[Fact]
		public void Remove()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Mock.Of<IEntitiesContext>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.RemoveAt(0)).ShouldThrow<NotSupportedException>();
			}
		}
	}
}
