using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using MyVote.UI.ViewModels;
using MyVote.UI.W8.Tests.Mocks;

namespace MyVote.UI.W8.Tests.ViewModels
{
	[TestClass]
	public sealed class PollCommentViewModelTests
	{
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private PollCommentMock PollComment { get; set; }

		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
		private PollCommentViewModel GetViewModel(int? parentCommandId, Func<int, string, Task> submitCommentCallback, bool isNested)
		{
			return new PollCommentViewModel(parentCommandId, PollComment, submitCommentCallback, isNested);
		}

		[TestInitialize]
		public void Init()
		{
			PollComment = new PollCommentMock();
		}

		//[TestMethod]
		//public void Create()
		//{
		//	// Arrange
		//	var parentCommentId = new Random().Next();
		//	var isNested = true;
		//	Func<int, string, Task> submitCommentCallback = (pollCommentId, commentText) =>
		//		{
		//			return Task.FromResult(0);
		//		};

		//	// Act
		//	var viewModel = GetViewModel(parentCommentId, submitCommentCallback, isNested);

		//	// Assert
		//	Assert.AreSame(submitCommentCallback, viewModel
		//}
	}
}
