using MyVote.UI.ViewModels;
using MyVote.UI.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MyVote.UI.Helpers
{
    public sealed class VmPageMappings : IVmPageMappings
    {
		public VmPageMappings()
        {
            this.Mappings = new Dictionary<Type, Type>
                {
					{typeof(RegistrationPageViewModel), typeof(RegistrationPage)},
					{typeof(ViewPollPageViewModel), typeof(ViewPollPage)},
					{typeof(AddPollPageViewModel), typeof(AddPollPage)},
					{typeof(PollResultsPageViewModel), typeof(PollResultsPage)}
                };
		    if (Device.Idiom == TargetIdiom.Tablet)
		    {
                this.Mappings.Add(typeof(LandingPageViewModel), typeof(LandingPageTablet));		        
                this.Mappings.Add(typeof(PollsPageViewModel), typeof(PollsPageTablet));		        
		    }
		    else
		    {
                this.Mappings.Add(typeof(LandingPageViewModel), typeof(LandingPagePhone));
                this.Mappings.Add(typeof(PollsPageViewModel), typeof(PollsPagePhone));
            }
        }

		public Dictionary<Type, Type> Mappings { get; private set; }

#if MOBILE
        public NavigationPage Navigation
        {
            get { return navigationPage; }
        }

        private static NavigationPage navigationPage;
        public static NavigationPage NavigationPage
        {
            set
            {
                navigationPage = value; 
                navigationPage.Pushed += NavigationPageOnPushed;
            }
            get { return navigationPage; }
        }

        private static void NavigationPageOnPushed(object sender, NavigationEventArgs navigationEventArgs)
        {
            var page = navigationEventArgs.Page;
            var vm = page.BindingContext;

            MessagingCenter.Send<VmPageMappings>(new VmPageMappings(), string.Format(Constants.Navigation.PageNavigated, vm.GetType()));
        }
#endif // MOBILE
    }
}
