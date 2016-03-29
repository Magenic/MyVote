using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Data.Entities;
using Spackle;
using Spackle.Extensions;

namespace MyVote.BusinessObjects.Net.Tests
{
	[TestClass]
	public sealed class PollSubmissionResponseTests
	{
		[TestMethod]
		public void Create()
		{
			var generator = new RandomObjectGenerator();
			var optionId = generator.Generate<int>();
			var optionPosition = generator.Generate<short>();
			var optionText = generator.Generate<string>();

			var option = new Mock<IPollOption>(MockBehavior.Strict);
			option.Setup(_ => _.PollOptionID).Returns(optionId);
			option.Setup(_ => _.OptionPosition).Returns(optionPosition);
			option.Setup(_ => _.OptionText).Returns(optionText);

			var builder = new ContainerBuilder();
			builder.Register<IEntities>(_ => Mock.Of<IEntities>());

			using (new ObjectActivator(builder.Build(), new ActivatorCallContext())
				.Bind(() => ApplicationContext.DataPortalActivator))
			{
				var response = DataPortal.CreateChild<PollSubmissionResponse>(option.Object);

				Assert.AreEqual(optionId, response.PollOptionID, nameof(response.PollOptionID));
				Assert.AreEqual(optionPosition, response.OptionPosition, nameof(response.OptionPosition));
				Assert.AreEqual(optionText, response.OptionText, nameof(response.OptionText));
				Assert.IsFalse(response.IsOptionSelected, nameof(response.IsOptionSelected));
				Assert.IsNull(response.PollResponseID, nameof(response.PollResponseID));
			}

			option.VerifyAll();
		}
	}
}
