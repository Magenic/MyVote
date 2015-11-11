using Autofac;
using Csla;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MyVote.BusinessObjects.Contracts;
using MyVote.Core.Extensions;
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

			using (new ObjectActivator(builder.Build()).Bind(() => ApplicationContext.DataPortalActivator))
			{
				var response = DataPortal.CreateChild<PollSubmissionResponse>(option.Object);

				Assert.AreEqual(optionId, response.PollOptionID, response.GetPropertyName(_ => _.PollOptionID));
				Assert.AreEqual(optionPosition, response.OptionPosition, response.GetPropertyName(_ => _.OptionPosition));
				Assert.AreEqual(optionText, response.OptionText, response.GetPropertyName(_ => _.OptionText));
				Assert.IsFalse(response.IsOptionSelected, response.GetPropertyName(_ => _.IsOptionSelected));
				Assert.IsNull(response.PollResponseID, response.GetPropertyName(_ => _.PollResponseID));
			}

			option.VerifyAll();
		}
	}
}
