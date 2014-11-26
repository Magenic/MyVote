using System;
using System.Collections.Generic;
using MyVote.UI.Forms;
using MyVote.UI.ViewModels;

using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    // Some like mapping the VM to pages by convension.  I don't.
    public class VMPageMappings : IVmPageMappings
    {
        public VMPageMappings()
        {
            Mapings = new Dictionary<Type, Type>
                {
                    {typeof(LandingPageViewModel), typeof(Login)},
                    {typeof(RegistrationPageViewModel), typeof(EditUser)},
					{typeof(PollsPageViewModel), typeof(Polls)},
					{typeof(ViewPollPageViewModel), typeof(ViewPoll)},
					{typeof(AddPollPageViewModel), typeof(AddPoll)},
					{typeof(PollResultsPageViewModel), typeof(PollResults)}
                };
        }

        public NavigationPage Navigation
        {
            get { return NavigationPage; }
        }

        public Dictionary<Type, Type> Mapings { get; private set; }

        public static NavigationPage NavigationPage { get; set; }
    }
}
