using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Services;
using MyVote.UI.Helpers;

namespace MyVote.Android.Tests
{
    [Activity(Label = "MyVote.Android.Tests", MainLauncher = true, Icon = "@drawable/icon")]
    public class TestActivity : Activity
    {
        int count = 1;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);



            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            // Get our button from the layout resource,
            // and attach an event to it
            Button button = FindViewById<Button>(Resource.Id.MyButton);

            button.Click += delegate { button.Text = string.Format("{0} clicks!", count++); };

			this.mobileService = new MobileService ();

			SignInWithTwitter ();
        }

        public async Task SignInWithTwitter()
        {
            await AuthenticateAndGo(AuthenticationProvider.Twitter);
        }

        public async Task SignInWithFacebook()
        {
            await AuthenticateAndGo(AuthenticationProvider.Facebook);
        }

        public async Task SignInWithMicrosoft()
        {
            await AuthenticateAndGo(AuthenticationProvider.Microsoft);
        }

        public async Task SignInWithGoogle()
        {
            await AuthenticateAndGo(AuthenticationProvider.Google);
        }

        protected void DeserializeParameter(string parameter)
        {
            //this.NavigationCriteria = Serializer.Deserialize<LandingPageNavigationCriteria>(parameter);
        }

        private async Task AuthenticateAndGo(AuthenticationProvider provider)
        {
            var result = await Authenticate(provider);

            if (result.Error != null)
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error authenticating.", "Error");
#else
                //await this.messageBox.ShowAsync("Error authenticating.", "Error");
#endif // WINDOWS_PHONE
            }
            else
            {
                //this.appSettings.Add(SettingsKeys.ProfileId, result.UserId);

                await LoadIdentityAndGo(result.UserId);
            }
        }

		private bool IsBusy {get;set;}
		private  IObjectFactory<IUserIdentity> objectFactory;
		private  IMobileService mobileService;

        private async Task LoadIdentityAndGo(string profileId)
        {
            IUserIdentity identity = null;
            IsBusy = true;
            try
            {
                identity = await this.objectFactory.FetchAsync(profileId);

                var principal = new CslaPrincipalCore(identity);
                Csla.ApplicationContext.User = principal;
            }
            catch (DataPortalException ex)
            {
                Console.WriteLine(ex.Message);
                identity = null;
            }
            IsBusy = false;

            if (identity == null)
            {
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error retrieving your profile.", "Error");
#else
                //await this.messageBox.ShowAsync("There was an error retrieving your profile.", "Error");
#endif // WINDOWS_PHONE
            }
			/*
            else if (identity.IsAuthenticated)
            {
                // If there is a PollId, the user is coming in from a URI.
                // Navigate to the Polls page, which adds it to the back stack.
                // Then, navigate to the View Poll page. This allows the user to be able
                // to back out of the View Poll page and land on the Polls page, rather than
                // leaving the app.

                if (this.NavigationCriteria != null && this.NavigationCriteria.PollId.HasValue)
                {
                    var criteria = new ViewPollPageNavigationCriteria
                    {
                        PollId = this.NavigationCriteria.PollId.Value
                    };

                    this.Navigation.NavigateToViewModel<PollsPageViewModel>();
                    this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }

                else if (this.NavigationCriteria != null && !string.IsNullOrEmpty(this.NavigationCriteria.SearchQuery))
                {
                    var criteria = new PollsPageSearchNavigationCriteria
                    {
                        SearchQuery = this.NavigationCriteria.SearchQuery
                    };

                    this.Navigation.NavigateToViewModel<PollsPageViewModel>();
                    this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }
                else
                {
                    this.Navigation.NavigateToViewModel<PollsPageViewModel>();
                }
            }
            else
            {
                var criteria = new RegistrationPageNavigationCriteria
                {
                    ProfileId = profileId
                };

                if (this.NavigationCriteria != null)
                {
                    criteria.PollId = this.NavigationCriteria.PollId;
                }

                this.Navigation.NavigateToViewModel<RegistrationPageViewModel>(criteria);
            }
			*/
        }

        private async Task<AuthenticationResult> Authenticate(AuthenticationProvider provider)
        {
            var result = new AuthenticationResult();

            try
            {
                result.UserId = await this.mobileService.AuthenticateAsync(this, provider);
            }
            catch (InvalidOperationException ex)
            {
                result.Error = ex;
            }

            return result;
        }

        //private LandingPageNavigationCriteria NavigationCriteria { get; set; }

        private class AuthenticationResult
        {
            public string UserId { get; set; }
            public Exception Error { get; set; }
        }
    }
}

