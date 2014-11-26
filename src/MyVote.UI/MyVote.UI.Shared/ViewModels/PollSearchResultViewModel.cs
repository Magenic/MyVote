using System.Threading.Tasks;
using Cirrious.MvvmCross.ViewModels;
using MyVote.BusinessObjects.Contracts;
using MyVote.UI.Contracts;
using MyVote.UI.NavigationCriteria;

namespace MyVote.UI.ViewModels
{
    public class PollSearchResultViewModel : PageViewModelBase, IPollSearchResultViewModel
    {
        private IPollSearchResult pollSearchResult;
        private readonly IObjectFactory<IPollSubmissionCommand> objectFactory;

#if __ANDROID__
        private string missingImage = "NoImage.png";
#elif __IOS__
        private string missingImage = "NoImage.png";
#endif
        
        public PollSearchResultViewModel(IPollSearchResult searchResult
            , IObjectFactory<IPollSubmissionCommand> objectFactory)
        {
            this.pollSearchResult = searchResult;
            this.objectFactory = objectFactory;
        }

        public System.Windows.Input.ICommand ViewPoll
        {
            get { return new MvxCommand(async () => await this.DoViewPollAsync()); }
        }

        private async Task DoViewPollAsync()
        {
            this.IsBusy = true;
            var identity = (IUserIdentity)Csla.ApplicationContext.User.Identity;
			var command = await this.objectFactory.CreateAsync();
			command.PollID = this.Id;
			command.UserID = identity.UserID.HasValue ? identity.UserID.Value : 0;
			command = await this.objectFactory.ExecuteAsync(command);
            this.IsBusy = false;

			if (command.Submission != null)
			{
                var criteria = new ViewPollPageNavigationCriteria
                {
                    PollId = this.Id
                };

                this.ShowViewModel<ViewPollPageViewModel>(criteria);
            }
			else
			{
                var navigationCriteria = new PollResultsPageNavigationCriteria
                {
                    PollId = this.Id
                };

                this.ShowViewModel<PollResultsPageViewModel>(navigationCriteria);
            }
        }

        public int Id
        {
            get
            {
                return this.pollSearchResult.Id;
            }
        }
 
        public string ImageLink
        {
            get
            {
                return this.pollSearchResult.ImageLink == string.Empty ?
                    missingImage :
                    this.pollSearchResult.ImageLink;
            }
        }

        public string Question
        {
            get
            {
                return this.pollSearchResult.Question;
            }
        }

        public int SubmissionCount
        {
            get
            {
                return this.pollSearchResult.SubmissionCount;
            }
        }
    }
}