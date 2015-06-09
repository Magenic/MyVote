using System;
using MyVote.UI.Controls;
using MyVote.UI.Helpers;
using Xamarin.Forms;

namespace MyVote.UI.Views
{
	public partial class RegistrationPage : ContentPageBase
	{
		public RegistrationPage()
		{
		    try
		    {
                InitializeComponent();

		    }
		    catch (Exception ex)
		    {
		        var a = ex;
		        throw;
		    }

			this.ResolveAutofacDependencies();
		}
	}
}
