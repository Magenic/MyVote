
namespace MyVote.UI.NavigationCriteria
{
    public sealed class RegistrationPageNavigationCriteria
    {
        public string ProfileId { get; set; }
        public int? PollId { get; set; }
        public bool ExistingUser { get; set; }
    }
}