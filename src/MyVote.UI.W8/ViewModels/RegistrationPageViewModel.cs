using Caliburn.Micro;
using Csla;
using MyVote.BusinessObjects;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using MyVote.UI.ViewModels;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

namespace MyVote.UI.ViewModels
{
	public sealed class RegistrationPageViewModel : PageViewModelBase
	{
		private readonly IObjectFactory<IUser> objectFactory;
		private readonly IObjectFactory<IUserIdentity> userIdentityObjectFactory;
		private readonly IMessageBox messageBox;

		public RegistrationPageViewModel(
			INavigation navigation,
			IObjectFactory<IUser> objectFactory,
			IObjectFactory<IUserIdentity> userIdentityObjectFactory,
			IMessageBox messageBox)
			: base(navigation)
		{
			this.objectFactory = objectFactory;
			this.userIdentityObjectFactory = userIdentityObjectFactory;
			this.messageBox = messageBox;

			GenderOptions = new ObservableCollection<SelectOptionViewModel<string>>();
		}

		public async Task Submit()
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
				System.Diagnostics.Debug.WriteLine(ex);
				hasError = true;
			}
			this.IsBusy = false;

			if (!hasError)
			{
				// Always navigate to the Polls page first.
				this.Navigation.NavigateToViewModel<PollsPageViewModel>();

				// If there is a PollId, the user is coming in from a URI link.
				// Navigate to the View Poll page, leaving the Polls page in the back
				// stack so the user can back up to it.
				if (this.NavigationCriteria.PollId.HasValue)
				{
					var criteria = new ViewPollPageNavigationCriteria
					{
						PollId = this.NavigationCriteria.PollId.Value
					};

					this.Navigation.NavigateToViewModel<ViewPollPageViewModel>(criteria);
				}
			}
			else
			{
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error saving your profile. Please try again.", "Error");
#else
				await this.messageBox.ShowAsync("There was an error saving your profile. Please try again.", "Error");
#endif // WINDOWS_PHONE
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1804:RemoveUnusedLocals", MessageId = "result")]
		protected override void OnInitialize()
		{
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

			// OnInitialize isn't async, so we have to go old school.
			var task = this.CreateUserAsync();

			var awaiter = task.GetAwaiter();
			awaiter.OnCompleted(() =>
			{
				if (task.Exception != null)
				{
#if WINDOWS_PHONE
					this.messageBox.Show("There was an error creating the user.", "Error");
#else
					this.messageBox.ShowAsync("There was an error creating the user.", "Error");
#endif // WINDOWS_PHONE
				}
			});
		}

		public async Task CreateUserAsync()
		{
			this.User = await this.objectFactory.CreateAsync(NavigationCriteria.ProfileId);
		}

		protected override void DeserializeParameter(string parameter)
		{
			this.NavigationCriteria = Serializer.Deserialize<RegistrationPageNavigationCriteria>(parameter);
		}

		private void UserPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			NotifyOfPropertyChange(() => this.CanSave);
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
				NotifyOfPropertyChange(() => this.User);
			}
		}

		public bool CanSave
		{
			get { return this.User != null ? this.User.IsSavable : false; }
		}
	}
}
