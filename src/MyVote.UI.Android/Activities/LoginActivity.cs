﻿﻿using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Csla;
using MyVote.BusinessObjects;
using System.Threading.Tasks;
using MyVote.UI.Services;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using Autofac;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Helpers;

namespace MyVote.UI.Droid
{
    [Activity(Label = "MyVote", MainLauncher = true, Icon = "@drawable/icon", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape )]
	public class LoginActivity : MyVoteActivity
    {
		//declare app constants
		public static bool LOG_OUT_OF_APP;

		private ContainerBuilder ContainerBuilder { get; set; }

		[Inject]
		public IObjectFactory<IUserIdentity> ObjectFactory { get; set;}

		[Inject]
		public IMobileService MobileServices { get; set; }

		private LandingPageNavigationCriteria NavigationCriteria { get; set; }

		//declare needed UI variables
		private Button twitterButton, facebookButton, microsoftButton, googleButton;

		protected override void OnCreate(Bundle bundle)
        {   
			MyVoteActivity.setLoadingMessage ("Validating user credentials...");
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.login);

			twitterButton = FindViewById<Button>(Resource.Id.TwitterButton);
			twitterButton.Click += Twitter_Clicked;

            facebookButton = FindViewById<Button>(Resource.Id.FacebookButton);
			facebookButton.Click += Facebook_Clicked;

            microsoftButton = FindViewById<Button>(Resource.Id.MicrosoftButton);
			microsoftButton.Click += Microsoft_Clicked;

            googleButton = FindViewById<Button>(Resource.Id.GoogleButton);
			googleButton.Click += Google_Clicked;

        }

        protected async override void OnStart()
        {
            base.OnStart();
            if (!string.IsNullOrEmpty(this.GetLastUserId()))
            {
                await this.LoadIdentityAndGo(this.GetLastUserId());
            }
        }

        protected override void OnResume()
        {
            base.OnResume();
			if (LOG_OUT_OF_APP) {
				this.Finish();
			}
        }

		protected override void OnPause(){
			base.OnPause ();
			LOG_OUT_OF_APP = false;
		}

        void Twitter_Clicked (object sender, EventArgs e)
        {
			this.AwaitAuthenticateAndGo (AuthenticationProvider.Twitter);
        }

		void Facebook_Clicked (object sender, EventArgs e)
		{
			this.AwaitAuthenticateAndGo (AuthenticationProvider.Facebook);
		}

		void Microsoft_Clicked (object sender, EventArgs e)
		{
			this.AwaitAuthenticateAndGo (AuthenticationProvider.Microsoft);
		}

		void Google_Clicked (object sender, EventArgs e)
		{
			this.AwaitAuthenticateAndGo (AuthenticationProvider.Google);
		}

		private void AwaitAuthenticateAndGo(AuthenticationProvider provider)
		{
			var t = this.AuthenticateAndGo (provider);
			if (t.Exception != null)
				throw t.Exception;
		}

		private async Task AuthenticateAndGo(AuthenticationProvider provider)
		{
			var result = await Authenticate(provider);

			if (result.Error != null)
			{
				this.MessageBox.Show(this, "There was an error authenticating.", "Error");
			}
			else
			{
				this.SetLastUserId(result.UserId);
				await LoadIdentityAndGo(result.UserId);
			}
		}

		private async Task LoadIdentityAndGo(string profileId)
		{
			IUserIdentity identity = null;
			IsBusy = true;
            try
            {
                //identity = (this.objectFactory as IObjectFactory<IUserIdentity>).Fetch(profileId);
                identity = await this.ObjectFactory.FetchAsync(profileId);

                var principal = new CslaPrincipalCore(identity);
                Csla.ApplicationContext.User = principal;
            }
            catch (DataPortalException ex)
            {
                Console.WriteLine(ex.Message);
                identity = null;
                MessageBox.Show(this, ex.Message);
            }

			IsBusy = false;
			if (identity == null)
			{
				this.MessageBox.Show(this, "There was an error retrieving your profile.", "Error");
			}

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

                    //this.Navigation.NavigateToViewModel<PollsPageViewModel>();
                    //this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }

                else if (this.NavigationCriteria != null && !string.IsNullOrEmpty(this.NavigationCriteria.SearchQuery))
                {
                    var criteria = new PollsPageSearchNavigationCriteria
                    {
                        SearchQuery = this.NavigationCriteria.SearchQuery
                    };

                    //this.Navigation.NavigateToViewModel<PollsPageViewModel>();
                    //this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
                }
                else
                {
                    //this.Navigation.NavigateToViewModel<PollsPageViewModel>();
					var polls = new Intent (this, typeof(PollsActivity));
					StartActivity (polls);                
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

				var registration = new Intent (this, typeof(RegistrationActivity));
				registration.PutExtra("NavigationCriteria_PollId", criteria.PollId ?? 0);
				registration.PutExtra ("NavigationCriteria_ProfileId", criteria.ProfileId);
				StartActivity (registration);

                //this.Navigation.NavigateToViewModel<RegistrationPageViewModel>(criteria);
                
            }

		}

		private async Task<AuthenticationResult> Authenticate(AuthenticationProvider provider)
		{
			var result = new AuthenticationResult();

			try
			{
				result.UserId = await this.MobileServices.AuthenticateAsync(this, provider);
			}
			catch (InvalidOperationException ex)
			{
				result.Error = ex;
			}

			return result;
		}

		private class AuthenticationResult
		{
			public string UserId { get; set; }
			public Exception Error { get; set; }
		}
    }
}

