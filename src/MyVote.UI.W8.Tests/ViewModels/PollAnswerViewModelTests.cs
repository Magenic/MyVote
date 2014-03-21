using System;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class PollAnswerViewModelTests
	{
		private PollMock Poll { get; set; }
		private ObjectFactoryMock<IPollOption> ObjectFactory { get; set; }

		private PollAnswerViewModel GetViewModel()
		{
			return new PollAnswerViewModel(Poll, ObjectFactory, 1);
		}

		[TestInitialize]
		public void Init()
		{
			Poll = new PollMock();
			ObjectFactory = new ObjectFactoryMock<IPollOption>();
		}

		[TestMethod]
		public void PollAnswer()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollAnswer = Guid.NewGuid().ToString();

			Poll.PollOptionsDelegate = () =>
				{
					return new BusinessList<IPollOption>();
				};

			var pollOption = new PollOptionMock();
			pollOption.IsChild = true;
			ObjectFactory.CreateChildDelegate = () =>
				{
					return pollOption;
				};

			// Act
			viewModel.PollAnswer = pollAnswer;

			// Assert
			Assert.AreEqual(pollAnswer, viewModel.PollAnswer, "PollAnswer");
			Assert.AreEqual(pollAnswer, pollOption.OptionText, "OptionText");
			Assert.AreEqual(1, (int)pollOption.OptionPosition, "OptionPosition");
		}

		[TestMethod]
		public void PollAnswerSetTwice()
		{
			// Arrange
			var viewModel = GetViewModel();

			var pollAnswer = Guid.NewGuid().ToString();

			Poll.PollOptionsDelegate = () =>
			{
				return new BusinessList<IPollOption>();
			};

			var pollOption = new PollOptionMock();
			pollOption.IsChild = true;
			ObjectFactory.CreateChildDelegate = () =>
			{
				return pollOption;
			};

			// Act
			viewModel.PollAnswer = Guid.NewGuid().ToString();
			viewModel.PollAnswer = pollAnswer;

			// Assert
			Assert.AreEqual(pollAnswer, viewModel.PollAnswer, "PollAnswer");
			Assert.AreEqual(pollAnswer, pollOption.OptionText, "OptionText");
			Assert.AreEqual(1, (int)pollOption.OptionPosition, "OptionPosition");
		}
	}
}
