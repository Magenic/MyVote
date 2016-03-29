using Autofac;
using Csla;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;
using System;
using System.IO;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
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

		[TestMethod]
		public void EnsureCollectionCanBeSerialized()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
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
					Assert.AreEqual(1, newResponses.Count, nameof(newResponses.Count));
					Assert.AreEqual(responses[0].OptionText, newResponses[0].OptionText, nameof(IPollSubmissionResponse.OptionText));
				}
			}
		}

		[TestMethod]
		public void Create()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>(MockBehavior.Strict);
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());

				Assert.AreEqual(1, responses.Count, nameof(responses.Count));
			}

			pollSubmissionResponseFactory.VerifyAll();
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Clear()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.Clear();
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Insert()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.Insert(1, Mock.Of<IPollSubmissionResponse>());
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Move()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.Move(0, 0);
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Remove()
		{
			var pollSubmissionResponseFactory = new Mock<IObjectFactory<IPollSubmissionResponse>>();
			pollSubmissionResponseFactory.Setup(_ => _.CreateChild(It.IsAny<object[]>()))
				.Returns<object[]>(_ => DataPortal.CreateChild<PollSubmissionResponse>(_[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Object);

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.RemoveAt(0);
			}
		}
	}
}
