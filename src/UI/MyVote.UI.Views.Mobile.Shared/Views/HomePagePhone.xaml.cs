using System;
using MyVote.UI.ViewModels;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class HomePagePhone : ContentPageBase
	{
		public HomePagePhone()
		{
		    InitializeComponent();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            if (this.Resources != null)
            {
                if (this.Resources.Keys.Contains("vmViewPoll"))
                {
                    this.Resources.Remove("vmViewPoll");
                }
            }
            else
            {
                this.Resources = new ResourceDictionary();
            }

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, true);

            this.Resources.Add("vmViewPoll", ((PollsPageViewModel)this.BindingContext).ViewPoll);
            this.ApplyBindings();
        }
	}
}