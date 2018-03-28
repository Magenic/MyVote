using MyVote.BusinessObjects.Contracts;
using System;

namespace MyVote.UI.ViewModels
{
	public sealed class PollDataResultViewModel : ViewModelBase
	{
		public PollDataResultViewModel(IPollDataResult pollDataResult, 
                                       int totalResponses)
		{
            this.PollDataResult = pollDataResult ?? throw new ArgumentNullException(nameof(pollDataResult));
			this.TotalResponses = totalResponses;
		}

		private IPollDataResult pollDataResult;
		public IPollDataResult PollDataResult
		{
			get { return this.pollDataResult; }
			private set
			{
				this.pollDataResult = value;
                this.RaisePropertyChanged(nameof(PollDataResult));
			}
		}

		private int totalResponses;
		public int TotalResponses
		{
			get { return this.totalResponses; }
			private set
			{
				this.totalResponses = value;
                this.RaisePropertyChanged(nameof(TotalResponses));
                this.RaisePropertyChanged(nameof(ResponsePercentage));
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
