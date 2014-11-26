using System;
using System.Linq;
using MyVote.BusinessObjects;
using MyVote.UI.Helpers;
using MyVote.UI.ViewModels;

using Xamarin.Forms;

namespace MyVote.UI.Forms
{
    public partial class Polls : ContentPageBase
    {
        public Polls()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            VMPageMappings.NavigationPage.Popped -= this.OnPopped;
            VMPageMappings.NavigationPage.Pushed -= this.OnPushed;
            VMPageMappings.NavigationPage.Pushed += this.OnPushed;
            VMPageMappings.NavigationPage.Popped += this.OnPopped;
        }

        private void OnPushed(object sender, NavigationEventArgs e)
        {
            if (VMPageMappings.NavigationPage.CurrentPage.GetType() != typeof(Polls))
            {
                VMPageMappings.NavigationPage.Popped -= this.OnPopped;
                VMPageMappings.NavigationPage.Pushed -= this.OnPushed;                
            }
        }

        private void OnPopped(object sender, NavigationEventArgs e)
        {
            if (VMPageMappings.NavigationPage.CurrentPage.GetType() == typeof(Login))
            {
                VMPageMappings.NavigationPage.Popped -= this.OnPopped;
                VMPageMappings.NavigationPage.Pushed -= this.OnPushed;
                ((PollsPageViewModel)this.BindingContext).DoLogout();                
            }
        }

        public async void SortClick(object sender, EventArgs e)
        {
            const string noneResult = "None";
            const string mostPopular = "Most Popular";
            const string recentlyAdded = "Recently Added";

            var result = await this.DisplayActionSheet(null, null, null, noneResult, mostPopular, recentlyAdded);
            var viewModel = ((PollsPageViewModel)this.BindingContext);

            switch (result)
            {
                case noneResult:
                    viewModel.SelectSearchOption(viewModel.SearchOptions.Last());
                    break;
                case mostPopular:
                    viewModel.SelectSearchOption(viewModel.SearchOptions.Single(so => so.QueryType == PollSearchResultsQueryType.MostPopular));
                    break;
                case recentlyAdded:
                    viewModel.SelectSearchOption(viewModel.SearchOptions.Single(so => so.QueryType == PollSearchResultsQueryType.Newest));
                    break;
            }
        }

        public async void ActionsClick(object sender, EventArgs e)
        {
            const string add = "Add";
            const string editProfile = "Edit Profile";
            const string logout = "Logout";

            var result = await this.DisplayActionSheet(null, null, null, add, editProfile, logout);
            var viewModel = ((PollsPageViewModel)this.BindingContext);

            switch (result)
            {
                case add:
                    viewModel.AddPoll.Execute(null);
                    break;
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