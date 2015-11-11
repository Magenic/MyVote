using System;
using System.IO;
using Autofac;
using Csla;
using Csla.Serialization.Mobile;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Core.Extensions;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

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
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				var formatter = new MobileFormatter();

				using (var stream = new MemoryStream())
				{
					formatter.Serialize(stream, responses);
					stream.Position = 0;
					var newResponses = formatter.Deserialize(stream) as PollSubmissionResponseCollection;
					Assert.AreEqual(1, newResponses.Count, newResponses.GetPropertyName(_ => _.Count));
					Assert.AreEqual(responses[0].OptionText, newResponses[0].OptionText, newResponses.GetPropertyName(_ => _[0].OptionText));
				}
			}
		}

		[TestMethod]
		public void Create()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());

				Assert.AreEqual(1, responses.Count, responses.GetPropertyName(_ => _.Count));
			}
		}

		[TestMethod]
		[ExpectedException(typeof(NotSupportedException))]
		public void Clear()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
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
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.Insert(1, Mock.Of<IPollSubmissionResponse>());
			}
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void Move()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.Move(0, 0);
			}
		}

		[TestMethod, ExpectedException(typeof(NotSupportedException))]
		public void Remove()
		{
			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				responses.RemoveAt(0);
			}
		}
	}
}
