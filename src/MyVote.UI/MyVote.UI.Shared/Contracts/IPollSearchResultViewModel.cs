using System.Windows.Input;

namespace MyVote.UI.Contracts
{
    public interface IPollSearchResultViewModel
    {
        int Id { get; }
        string ImageLink { get; }
        string Question { get; }
        int SubmissionCount { get; }
        ICommand ViewPoll { get; }
    }
}