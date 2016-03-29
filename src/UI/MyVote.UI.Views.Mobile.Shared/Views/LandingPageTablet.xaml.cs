using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using MyVote.UI.Helpers;

namespace MyVote.UI.Views
{
	public partial class LandingPageTablet : ContentPageBase
	{
		public LandingPageTablet()
		{
            InitializeComponent();

			this.ResolveAutofacDependencies();

            Xamarin.Forms.NavigationPage.SetHasNavigationBar(this, false);

            //Issue with PCL, need to reference something in Xamarin.Forms.Labs or it gets compiled out
            var imageButton = btnMicrosoft;
		}
	}
}
