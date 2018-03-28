using Autofac;
using Csla;
using Csla.Core;
using Csla.Serialization.Mobile;
using FluentAssertions;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.Data.Entities;
using Rocks;
using Rocks.Options;
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

			var option = Rock.Create<IPollOption>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			option.Handle(nameof(IPollOption.IsChild), () => true);
			option.Handle(nameof(IPollOption.EditLevel), () => 0);
			option.Handle<int>(nameof(IPollOption.EditLevelAdded), _ => { });
			option.Handle(nameof(IPollOption.PollOptionID), () => optionId as int?);
			option.Handle(nameof(IPollOption.OptionPosition), () => optionPosition as short?);
			option.Handle(nameof(IPollOption.OptionText), () => optionText);
			option.Handle(_ => _.SetParent(Arg.IsAny<IParent>()));

			return new BusinessList<IPollOption>
			{
				option.Make()
			};
		}

		[Fact]
		public void EnsureCollectionCanBeSerialized()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

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

			pollSubmissionResponseFactory.Verify();
		}

		[Fact]
		public void Create()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());

				responses.Count.Should().Be(1);
			}

			pollSubmissionResponseFactory.Verify();
		}

		[Fact]
		public void Clear()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Clear()).ShouldThrow<NotSupportedException>();
			}

			pollSubmissionResponseFactory.Verify();
		}

		[Fact]
		public void Insert()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Insert(1, Rock.Make<IPollSubmissionResponse>())).ShouldThrow<NotSupportedException>();
			}

			pollSubmissionResponseFactory.Verify();
		}

		[Fact]
		public void Move()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.Move(0, 0)).ShouldThrow<NotSupportedException>();
			}

			pollSubmissionResponseFactory.Verify();
		}

		[Fact]
		public void Remove()
		{
			var pollSubmissionResponseFactory = Rock.Create<IObjectFactory<IPollSubmissionResponse>>(
				new RockOptions(allowWarnings: AllowWarnings.Yes));
			pollSubmissionResponseFactory.Handle<object[], IPollSubmissionResponse>(_ => _.CreateChild(Arg.IsAny<object[]>()),
				data => DataPortal.CreateChild<PollSubmissionResponse>(data[0] as IPollOption));

			var builder = new ContainerBuilder();
			builder.Register<IEntitiesContext>(_ => Rock.Make<IEntitiesContext>(
				new RockOptions(allowWarnings: AllowWarnings.Yes)));
			builder.Register<IObjectFactory<IPollSubmissionResponse>>(_ => pollSubmissionResponseFactory.Make());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var responses = DataPortal.CreateChild<PollSubmissionResponseCollection>(
					PollSubmissionResponseCollectionTests.CreateOptions());
				new Action(() => responses.RemoveAt(0)).ShouldThrow<NotSupportedException>();
			}

			pollSubmissionResponseFactory.Verify();
		}
	}
}
