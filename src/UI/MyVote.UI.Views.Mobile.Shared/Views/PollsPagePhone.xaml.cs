using System;
using System.Linq;
using MvvmCross.Core.ViewModels;
using MvvmCross.Platform;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class PollsPagePhone : TabbedPage
	{
		public PollsPagePhone()
		{
            InitializeComponent();

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, true);

            Device.OnPlatform(
		        iOS: () => ToolbarItems.Add(new ToolbarItem("More", "", ActionsClick, ToolbarItemOrder.Primary, 1)),
                Android: () =>
                {
                    ToolbarItems.Add(new ToolbarItem("Edit Profile", "", () => { }, ToolbarItemOrder.Secondary, 1));
                    ToolbarItems.Add(new ToolbarItem("Logout", "", () => { }, ToolbarItemOrder.Secondary, 2));
                }
            );
        }

        protected override void OnBindingContextChanged()
	    {
	        if (Children != null && this.BindingContext != null)
	        {
	            var homePage = Children.Single(c => c.Title == "Home");
	            homePage.PropertyChanged += Child_PropertyChanged;
	            homePage.BindingContext = BindingContext;
	            var categoriesPage = Children.Single(c => c.Title == "Categories");
                categoriesPage.PropertyChanged += Child_PropertyChanged;
                categoriesPage.BindingContext = BindingContext;
                var searchPage = Children.Single(c => c.Title == "Search");
                searchPage.PropertyChanged += Child_PropertyChanged;
                searchPage.BindingContext = BindingContext;
                var addPollPage = Children.Single(c => c.Title == "New Poll");
                addPollPage.PropertyChanged += Child_PropertyChanged;
#if IOS
                homePage.Icon = "Home.png";
                categoriesPage.Icon = "Categories.png";
                searchPage.Icon = "Search.png";
                addPollPage.Icon = "New.png";

#endif
                SetupNewPollViewModel();

#if ANDROID
                SetupToolbar();
#endif
	        }
            else if (Children != null)
            {
                var addPollPage = Children.Single(c => c.Title == "New Poll");
                var addPollviewModel = addPollPage.BindingContext as AddPollPageViewModel;
                if (addPollviewModel != null)
                {
                    addPollviewModel.PollAdded -= AddPollviewModel_PollAdded;
                }
            }
	    }

	    private void SetupToolbar()
	    {
	        var viewModel = ((PollsPageViewModel) this.BindingContext);
	        var toolbar = this.ToolbarItems.Single(t => t.Text == "Edit Profile");
	        toolbar.Command = viewModel.EditProfile;
	        toolbar = this.ToolbarItems.Single(t => t.Text == "Logout");
	        toolbar.Command = viewModel.Logout;
	    }

	    private void Child_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsBusy")
			{
                this.IsBusy = ((Page)sender).IsBusy;
			}
        }

	    private void SetupNewPollViewModel()
	    {
            var addPollPage = Children.Single(c => c.Title == "New Poll");
            var viewModelLoader = Mvx.Resolve<IMvxViewModelLoader>();
            var addPollviewModel = (AddPollPageViewModel)viewModelLoader.LoadViewModel(new MvxViewModelRequest(typeof(AddPollPageViewModel), new MvxBundle(), new MvxBundle(), new MvxRequestedBy(MvxRequestedByType.UserAction)), null);
            addPollviewModel.PollAdded += AddPollviewModel_PollAdded;
            addPollPage.BindingContext = addPollviewModel;
	    }

	    private void AddPollviewModel_PollAdded(object sender, AddPollEventArgs addPollEventArgs)
	    {
            Device.BeginInvokeOnMainThread(() =>
            {
                var home = Children.Single(c => c.Title == "Home");
                this.SelectedItem = home;
            });
	    }

        public async void ActionsClick()
        {
            const string editProfile = "Edit Profile";
            const string logout = "Logout";

            var result = await this.DisplayActionSheet(null, null, null, editProfile, logout);
            var viewModel = ((PollsPageViewModel)this.BindingContext);

            switch (result)
            {
                case editProfile:
                    viewModel.EditProfile.Execute(null);
                    break;
                case logout:
                    viewModel.Logout.Execute(null);
                    break;
            }
        }
    }
}
