using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MyVote.UI.Droid
{
	[Actionbar]
    [Activity (Label = "EditUserActivity", ScreenOrientation = Android.Content.PM.ScreenOrientation.Landscape)]			
	public class EditUserActivity : MyVoteActivity
	{

		protected override void OnCreate (Bundle bundle)
		{
			MyVoteActivity.setLoadingMessage ("Loading Edit User interface...");
			base.OnCreate (bundle);

			SetContentView(Resource.Layout.edituser);

			// Create your application here
		}
	}
}

