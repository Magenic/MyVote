using MyVote.BusinessObjects.Contracts;
using System;
using MvvmCross.Core.ViewModels;

namespace MyVote.UI.ViewModels
{
	public sealed class PollDataResultViewModel : MvxViewModel
	{
		public PollDataResultViewModel(IPollDataResult pollDataResult, int totalResponses)
		{
			if (pollDataResult == null)
			{
				throw new ArgumentNullException("pollDataResult");
			}

			this.PollDataResult = pollDataResult;
			this.TotalResponses = totalResponses;
		}

		private IPollDataResult pollDataResult;
		public IPollDataResult PollDataResult
		{
			get { return this.pollDataResult; }
			private set
			{
				this.pollDataResult = value;
				this.RaisePropertyChanged(() => this.PollDataResult);
			}
		}

		private int totalResponses;
		public int TotalResponses
		{
			get { return this.totalResponses; }
			private set
			{
				this.totalResponses = value;
				this.RaisePropertyChanged(() => this.TotalResponses);
				this.RaisePropertyChanged(() => this.ResponsePercentage);
			}
		}

		public int ResponsePercentage
		{
			get
			{
				return (int)(((double)this.PollDataResult.ResponseCount / (double)this.TotalResponses) * 100);
			}
		}
    }
}
