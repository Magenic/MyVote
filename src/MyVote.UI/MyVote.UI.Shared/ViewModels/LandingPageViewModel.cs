using System.Collections.Generic;
using Cirrious.MvvmCross.ViewModels;
using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.Services;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin;

namespace MyVote.UI.ViewModels
{
	public sealed class LandingPageViewModel : PageViewModelBase
	{
		private readonly IMessageBox messageBox;
		private readonly IMobileService mobileService;
		private readonly IObjectFactory<IUserIdentity> objectFactory;
		private readonly IAppSettings appSettings;

        public LandingPageViewModel(
            IMessageBox messageBox,
            IMobileService mobileService,
            IObjectFactory<IUserIdentity> objectFactory,
            IAppSettings appSettings)
        {
            this.messageBox = messageBox;
            this.mobileService = mobileService;
            this.objectFactory = objectFactory;
            this.appSettings = appSettings;
        }

        public void Init(LandingPageNavigationCriteria criteria)
        {
            this.NavigationCriteria = criteria;
        }

	    public async override void Start()
    	{
	        base.Start();
			string profileId;
			if (this.appSettings.TryGetValue<string>(SettingsKeys.ProfileId, out profileId))
			{
			    try
			    {
			        await LoadIdentityAndGo(profileId);
			    }
			    catch (Exception ex)
			    {
                    this.messageBox.ShowAsync("There was an error loading your profile.", "Error");
                    Insights.Report(ex);
                }
			}
	    }

		public ICommand SignInWithTwitter
		{
            get
		    {
                return new MvxCommand(async () =>
                         await AuthenticateAndGo(AuthenticationProvider.Twitter)); 		        
		    }
		}

        public ICommand SignInWithFacebook
		{
            get
		    {
                return new MvxCommand(async () =>
                         await AuthenticateAndGo(AuthenticationProvider.Facebook)); 		        
		    }
		}

        public ICommand SignInWithMicrosoft
		{
            get
		    {
                return new MvxCommand(async () =>
                         await AuthenticateAndGo(AuthenticationProvider.Microsoft)); 		        
		    }
		}

		public ICommand SignInWithGoogle
		{
            get
            {
                return new MvxCommand(async () =>
                         await AuthenticateAndGo(AuthenticationProvider.Google));
            }
        }

		private async Task AuthenticateAndGo(AuthenticationProvider provider)
		{
			var result = await Authenticate(provider);

			if (result.Error != null)
			{
				if (result.Error.HResult != -2146233079) // Cancelled by user
				{
					await this.messageBox.ShowAsync("Error authenticating.", "Error");
				}
			}
			else
			{
				this.appSettings.Add(SettingsKeys.ProfileId, result.UserId);

				await LoadIdentityAndGo(result.UserId);
			}
		}

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
				Debug.WriteLine(ex.Message);
				identity = null;
                Insights.Report(ex);
			}
			IsBusy = false;

			if (identity == null)
			{
				await this.messageBox.ShowAsync("There was an error retrieving your profile.", "Error");
			}
			else if (identity.IsAuthenticated)
			{
                Insights.Identify(profileId, Insights.Traits.Name, identity.UserName);
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

					this.ShowViewModel<PollsPageViewModel>();
                    this.ShowViewModel<ViewPollPageViewModel>(criteria);
				}

                else if (this.NavigationCriteria != null && !string.IsNullOrEmpty(this.NavigationCriteria.SearchQuery))
                {
                    var criteria = new PollsPageSearchNavigationCriteria
                    {
                        SearchQuery = this.NavigationCriteria.SearchQuery
                    };

                    this.ShowViewModel<PollsPageViewModel>();
                    this.ShowViewModel<ViewPollPageViewModel>(criteria);
                }
                else
				{
					this.ShowViewModel<PollsPageViewModel>();
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

				this.ShowViewModel<RegistrationPageViewModel>(criteria);
			}
		}

		private async Task<AuthenticationResult> Authenticate(AuthenticationProvider provider)
		{
			var result = new AuthenticationResult();

			try
			{
				result.UserId = await this.mobileService.AuthenticateAsync(provider);
			}
			catch (InvalidOperationException ex)
			{
				result.Error = ex;
                Insights.Report(ex);
			}

			return result;
		}

		private LandingPageNavigationCriteria NavigationCriteria { get; set; }

		private class AuthenticationResult
		{
			public string UserId { get; set; }
			public Exception Error { get; set; }
		}
	}
}
