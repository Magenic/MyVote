using MvvmCross.Core.ViewModels;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.UI.ViewModels
{
    public sealed class PollOptionViewModel : MvxViewModel
    {
		private readonly IPoll poll;
		private readonly IObjectFactory<IPollOption> objectFactory;
		private readonly short optionPosition;

        public IPollOption PollOption { get; private set; }

		public PollOptionViewModel(
			IPoll poll,
			IObjectFactory<IPollOption> objectFactory,
			short optionPosition)
		{
			this.poll = poll;
			this.objectFactory = objectFactory;
			this.optionPosition = optionPosition;
            if (optionPosition >= 0 && optionPosition <= 1)
            {
                CreatePollOption();
                poll.PollOptions.Add(PollOption);
            }
		}

		private void CreatePollOption()
		{
			PollOption = this.objectFactory.CreateChild();
			PollOption.OptionText = this.optionText;
			PollOption.OptionPosition = this.optionPosition;
		}

		private string optionText;
		public string OptionText
		{
			get { return this.optionText; }
			set
			{
				this.optionText = value;
				this.RaisePropertyChanged(() => OptionText);
				if (!string.IsNullOrEmpty(value))
				{
					if (PollOption == null)
					{
						this.CreatePollOption();
                        this.poll.PollOptions.Add(PollOption);
					}
					PollOption.OptionText = value;
				}
				else
				{
					if (PollOption != null && optionPosition >= 2)
					{
						this.poll.PollOptions.Remove(PollOption);
						PollOption = null;
					}
				}
			}
		}
    }
}
