using MyVote.UI.ViewModels;
using Xamarin.Forms;
using System;
#if ANDROID
using Xamarin.Forms.Platform.Android;
using Android.Support.Design.Widget;
#endif

namespace MyVote.UI.Views
{
	public partial class HomePagePhone : ContentPageBase
	{
        public HomePagePhone()
        {
            InitializeComponent();

#if ANDROID
            NativeViewWrapper actionButtonView = null;
            actionButtonView = (NativeViewWrapper)fabParent.Content;
            if (actionButtonView != null)
            {
                var actionButton = (FloatingActionButton)actionButtonView.NativeView;
                actionButton.SetImageResource(Resource.Drawable.ic_add_white_24dp);
            }
#endif
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

        private void fab_click(object sender, EventArgs e)
        {
            ((PollsPageViewModel)BindingContext).AddPoll.Execute(null);
        }
	}
}