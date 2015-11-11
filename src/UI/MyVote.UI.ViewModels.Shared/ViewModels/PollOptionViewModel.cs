using Cirrious.MvvmCross.ViewModels;
using MyVote.BusinessObjects.Contracts;

namespace MyVote.UI.ViewModels
{
    public sealed class PollOptionViewModel : MvxViewModel
    {
		private readonly IPoll poll;
		private readonly IObjectFactory<IPollOption> objectFactory;
		private readonly short optionPosition;

		private IPollOption pollOption;

		public PollOptionViewModel(
			IPoll poll,
			IObjectFactory<IPollOption> objectFactory,
			short optionPosition)
		{
			this.poll = poll;
			this.objectFactory = objectFactory;
			this.optionPosition = optionPosition;
		}

		private void CreatePollOption()
		{
			this.pollOption = this.objectFactory.CreateChild();
			this.pollOption.OptionText = this.optionText;
			this.pollOption.OptionPosition = this.optionPosition;
			this.poll.PollOptions.Add(this.pollOption);
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
					if (this.pollOption == null)
					{
						this.CreatePollOption();
					}
					else
					{
						this.pollOption.OptionText = value;
					}
				}
				else
				{
					if (this.pollOption != null)
					{
						this.poll.PollOptions.Remove(this.pollOption);
						this.pollOption = null;
					}
				}
			}
		}
    }
}
