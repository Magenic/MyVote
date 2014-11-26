using System;
using System.Windows.Input;

using Cirrious.MvvmCross.ViewModels;

using Csla;
using MyVote.BusinessObjects.Contracts;
using MyVote.BusinessObjects.Core;
using MyVote.UI.Helpers;
using MyVote.UI.NavigationCriteria;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin;

namespace MyVote.UI.ViewModels
{
    public sealed class RegistrationPageViewModel : PageViewModelBase
    {
        private readonly IObjectFactory<IUser> objectFactory;
        private readonly IObjectFactory<IUserIdentity> userIdentityObjectFactory;
        private readonly IMessageBox messageBox;

        public RegistrationPageViewModel(
            IObjectFactory<IUser> objectFactory,
            IObjectFactory<IUserIdentity> userIdentityObjectFactory,
            IMessageBox messageBox)
        {
            this.objectFactory = objectFactory;
            this.userIdentityObjectFactory = userIdentityObjectFactory;
            this.messageBox = messageBox;

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
                System.Diagnostics.Debug.WriteLine(ex);
                hasError = true;
                Insights.Report(ex);
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
                    //var criteria = new ViewPollPageNavigationCriteria { PollId = this.NavigationCriteria.PollId.Value };
                    this.Close(this);
                    //this.ShowViewModel<ViewPollPageViewModel>(criteria);
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

        public void Init(RegistrationPageNavigationCriteria criteria)
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
#if WINDOWS_PHONE
				this.messageBox.Show("There was an error creating the user: " + ex.Message, "Error");
#else
                this.messageBox.ShowAsync("There was an error creating the user: " + ex.Message, "Error");
#endif // WINDOWS_PHONE
                Insights.Report(ex);
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
