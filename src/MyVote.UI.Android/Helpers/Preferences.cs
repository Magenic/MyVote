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
using Android.Preferences;

namespace MyVote.UI.Droid
{
	public static class Preferences
	{
		public static string GetLastUserId(this Context context)
		{
			var prefs = PreferenceManager.GetDefaultSharedPreferences (context); 
			return prefs.GetString("LastUser", null);
		}


		public static void SetLastUserId(this Context context, string lastUser)
		{
			var prefs = PreferenceManager.GetDefaultSharedPreferences (context); 
			var editor = prefs.Edit ();
			editor.PutString ("LastUser", lastUser);
			editor.Commit();
		}

	}
}

