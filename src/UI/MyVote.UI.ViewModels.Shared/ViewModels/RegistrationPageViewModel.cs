using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using MvvmCross.Core.ViewModels;

namespace MyVote.UI.ViewModels
{
	public sealed class RegistrationPageViewModel : ViewModelBase<RegistrationPageNavigationCriteria>
    {
		private readonly IObjectFactory<IUser> objectFactory;
		private readonly IObjectFactory<IUserIdentity> userIdentityObjectFactory;
		private readonly IMessageBox messageBox;
		private readonly ILogger logger;

		public RegistrationPageViewModel(
			IObjectFactory<IUser> objectFactory,
			IObjectFactory<IUserIdentity> userIdentityObjectFactory,
			IMessageBox messageBox,
			ILogger logger)
		{
			this.objectFactory = objectFactory;
			this.userIdentityObjectFactory = userIdentityObjectFactory;
			this.messageBox = messageBox;
			this.logger = logger;

			GenderOptions = new ObservableCollection<SelectOptionViewModel<string>>();
		}

		public ICommand Submit
		{
			get
			{
				return new MvxCommand(async () => await SubmitHandlerAsync());
			}
		}

		private async Task SubmitHandlerAsync()
		{
			this.IsBusy = true;

			var hasError = false;
			try
			{
				this.User = await this.User.SaveAsync() as IUser;

				var identity = await this.userIdentityObjectFactory.FetchAsync(this.User.ProfileID);

				var principal = new CslaPrincipalCore(identity);
				Csla.ApplicationContext.User = principal;
			}
			catch (DataPortalException ex)
			{
				this.logger.Log(ex);
				hasError = true;
			}
			this.IsBusy = false;

			if (!hasError)
			{
				// Always navigate to the Polls page first.

				this.ShowViewModel<PollsPageViewModel>();

				// If there is a PollId, the user is coming in from a URI link.
				// Navigate to the View Poll page, leaving the Polls page in the back
				// stack so the user can back up to it.
				if (this.NavigationCriteria.PollId.HasValue)
				{
					this.Close(this);
				}
#if MOBILE
                ChangePresentation(new ClearBackstackHint());
#endif
            }
            else
			{
				await this.messageBox.ShowAsync("There was an error saving your profile. Please try again.", "Error");
			}
		}

		public override void RealInit(RegistrationPageNavigationCriteria criteria)
		{
			this.NavigationCriteria = criteria;
		}

		public async override void Start()
		{
			base.Start();
			GenderOptions.Add(new SelectOptionViewModel<string>
			{
				Display = "Male",
				Value = "M"
			});
			GenderOptions.Add(new SelectOptionViewModel<string>
			{
				Display = "Female",
				Value = "F"
			});

			try
			{
				if (this.NavigationCriteria.ExistingUser)
				{
					await this.LoadUserAsync(this.NavigationCriteria.ProfileId);
				}
				else
				{
					await this.CreateUserAsync();
				}
			}
			catch (Exception ex)
			{
				this.messageBox.ShowAsync("There was an error creating the user: " + ex.Message, "Error");
				this.logger.Log(ex);
			}
		}

		public async Task CreateUserAsync()
		{
			this.User = await this.objectFactory.CreateAsync(NavigationCriteria.ProfileId);
		}

		public async Task LoadUserAsync(string profileId)
		{
			this.User = await this.objectFactory.FetchAsync(profileId);
		}

		private void UserPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			this.RaisePropertyChanged(() => this.CanSave);
		}

		private RegistrationPageNavigationCriteria NavigationCriteria { get; set; }

		public ObservableCollection<SelectOptionViewModel<string>> GenderOptions { get; private set; }

		private IUser user;
		public IUser User
		{
			get { return this.user; }
			private set
			{
				if (this.user != null)
				{
					this.user.PropertyChanged -= UserPropertyChanged;
				}
				this.user = value;
				if (value != null)
				{
					value.PropertyChanged += UserPropertyChanged;
				}
				this.RaisePropertyChanged(() => this.User);
			}
		}

		public bool CanSave
		{
			get { return this.User != null ? this.User.IsSavable : false; }
		}
    }
}
